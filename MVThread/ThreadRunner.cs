using MVThread.Datas;
using MVThread.Events;
using MVThread.File;
using MVThread.Proxylist;
using MVThread.Wordlist;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace MVThread
{
    public class ThreadRunner : IRunner
    {
        private CancellationTokenSource _cts;
        private List<Thread> _threadList = new List<Thread>();
        private DataPool _datapool = new DataPool();
        private Log _log = new Log();
        private Save _save = new Save();
        private ProxyPool _proxylist = new ProxyPool();
        private Stopwatch _stopwatch = new Stopwatch();
        private IWordList _wordlist = null;
        private RunnerStatus _runnerStatus;
        private bool _theEnd = false;
        private bool _run = false;
        private int _position = 0;
        private int _bot = 0;

        public event EventHandler<StartEventArgs> OnStarted;
        public event EventHandler<StopEventArgs> OnStopped;
        public event EventHandler<EventArgs> OnCompleted;
        public event ResolveConfig OnConfig;
        public event EventHandler<ExceptionEventArgs> OnException;

        public bool IsRunning { get { return RunnerStatus == RunnerStatus.Started; } }
        public bool IsCompleted { get { return RunnerStatus == RunnerStatus.Completed; } }
        public RunnerStatus RunnerStatus { get { return _runnerStatus; } }
        public IProxyPool ProxyPool { get { return _proxylist; } }
        public string LogAddress { get { return _log.Address; } set { _log.Address = value; } }
        public Progress Progress
        {
            get
            {
                return _wordlist == null ? new Progress(0, 0) : new Progress(_wordlist.Count, _wordlist.Position);
            }
        }
        public int CPM { get { return _datapool.CPM; } }
        public int Active { get { return _threadList.Where(t => t != null).ToList().Count; } }
        public string Elapsed
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        public ThreadRunner()
        {
            _runnerStatus = RunnerStatus.Idle;
        }

        public void SetWordlist(IEnumerable<string> combolist, int position = 0)
        {
            if(combolist == null)
                throw new ArgumentNullException("Wordlist is null.");
            if (combolist.Count() == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (position < 0 || position > combolist.Count() - 1)
                throw new ArgumentNullException($"Position 0-{combolist.Count() - 1}");

            _position = position;
            _wordlist = new ComboList(combolist, position);
        }

        public void SetWordlist(IEnumerable<string> userlist, IEnumerable<string> passlist, ComboType type, int position = 0)
        {
            if (userlist == null || passlist == null)
                throw new ArgumentNullException("Wordlist is null.");
            if (userlist.Count() == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (passlist.Count() == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (position < 0 || position > (userlist.Count() * passlist.Count()) - 1)
                throw new ArgumentNullException($"Position 0-{(userlist.Count() * passlist.Count()) - 1}");

            _position = position;
            _wordlist = new CredentialsList(userlist, passlist, type, position);
        }

        public void SetProxylist(IEnumerable<string> proxylist, ProxyType type, bool join = false)
        {
            if (proxylist == null)
                throw new ArgumentNullException("Proxylist is null.");
            if (proxylist.Count() == 0)
                throw new ArgumentNullException("Proxylist is null.");

            List<Proxy> list = new List<Proxy>();
            foreach (var item in proxylist)
            {
                Proxy proxy = new Proxy()
                {
                    Address = item,
                    Type = type
                };
                list.Add(proxy);
            }
            _proxylist.SetProxylist(list, join);
        }

        public void SetProxylist(string url, ProxyType type, bool join = false)
        {
            if (Regex.IsMatch(url, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
            {
                try
                {
                    string input = webrequest(url);
                    MatchCollection mc = new Regex("\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\:[0-9]{1,5}\\b").Matches(input);
                    List<string> proxylist = new List<string>();
                    foreach (object prx in mc)
                    {
                        Match match = (Match)prx;
                        proxylist.Add(match.ToString());
                    }

                    List<Proxy> list = new List<Proxy>();
                    foreach (var item in proxylist)
                    {
                        Proxy proxy = new Proxy()
                        {
                            Address = item,
                            Type = type
                        };
                        list.Add(proxy);
                    }
                    _proxylist.SetProxylist(list, join);
                }
                catch
                {
                    throw new Exception("Please enter valid proxy link and try.");
                }
            }
            else
                throw new Exception("The URL format is incorrect.");
        }

        public void Start(int bot)
        {
            if (!_run)
            {
                if (_wordlist == null)
                    throw new Exception("Wordlist is null.");
                if (!_wordlist.HasNext)
                    throw new Exception("Wordlist is null.");

                _theEnd = false;
                _run = true;

                _threadList.Clear();

                _bot = bot;
                if (_bot > _wordlist.Count - _position)
                {
                    _bot = _wordlist.Count - _position;
                }

                _runnerStatus = RunnerStatus.Started;
                try { OnStarted?.Invoke(this, new StartEventArgs() { Bot = _bot }); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnStarted", Exception = ex, Log = _log }); }

                _stopwatch.Restart();
                _stopwatch.Start();

                _cts = new CancellationTokenSource();

                for (int i = 0; i < _bot; i++)
                {
                    int num = i;
                    Thread thread = new Thread(() => { Config(num, _cts.Token); }) { IsBackground = true, Name = $"ID{num}" };
                    thread.Start();
                    _threadList.Add(thread);
                }

                new Thread(() => { CompletedTask(); }) { IsBackground = true }.Start();
            }
        }

        public void Stop()
        {
            if (_run)
            {
                _runnerStatus = RunnerStatus.Stopped;
                _cts.Cancel();
            }
        }


        private void Config(int num, CancellationToken ct)
        {
            while (_wordlist.HasNext && !_theEnd)
            {
                int retry = 0;
                string data = _wordlist.GetData();

                Retry:

                if (ct.IsCancellationRequested)
                    break;

                try
                {
                    Proxy proxy = null;
                    if (!_proxylist.Less)
                    {
                        proxy = _proxylist.Get();
                        if (proxy == null)
                        {
                            goto Retry;
                        }
                    }

                    Status? status = Status.OK;
                    status = OnConfig?.Invoke(this, new DataEventArgs()
                    {
                        Retry = retry,
                        Data = data,
                        Proxy = proxy,
                        Save = _save,
                        Log = _log
                    });

                    switch (status)
                    {
                        case Status.OK:
                            _datapool.Add();
                            break;
                        case Status.Retry:
                            retry++;
                            goto Retry;
                        case Status.TheEnd:
                            _theEnd = true;
                            break;
                        default:
                            _datapool.Add();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, new ExceptionEventArgs()
                    {
                        Location = "OnConfig",
                        Exception = ex,
                        Log = _log
                    });
                }
            }

            _threadList[num] = null;
        }

        private void CompletedTask()
        {
            while (_run)
            {
                try
                {
                    var activeThread = Active;
                    if (activeThread == 0 && _runnerStatus == RunnerStatus.Stopped)
                    {
                        _run = false;
                        _stopwatch.Stop();
                        _datapool.Clear();
                        _cts.Dispose();
                        try { OnStopped?.Invoke(this, new StopEventArgs() { WordList = _wordlist, Save = _save, Log = _log }); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnStopped", Exception = ex, Log = _log }); }
                    }
                    else if (activeThread == 0)
                    {
                        _run = false;
                        _stopwatch.Stop();
                        _datapool.Clear();
                        _cts.Dispose();
                        _runnerStatus = RunnerStatus.Completed;
                        try { OnCompleted?.Invoke(this, new EventArgs()); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnCompleted", Exception = ex, Log = _log }); }
                    }
                }
                catch
                {
                }
                Thread.Sleep(100);
            }
        }

        private string webrequest(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certification, chain, sslPolicyErrors) => true;
            try
            {
                var req = WebRequest.Create(url);
                using (var res = (HttpWebResponse)req.GetResponse())
                {
                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                using (var res = (HttpWebResponse)ex.Response)
                {
                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
