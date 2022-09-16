﻿using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MVThread
{
    public class TaskRunner : IRunner
    {
        #region Fields (private)

        private List<Task> _taskList;

        #endregion

        #region Fields (protected)

        protected CancellationTokenSource _cts;
        protected DataPool _datapool;
        protected Log _log;
        protected Save _save;
        protected ProxyPool _proxylist;
        protected Stopwatch _stopwatch;
        protected IWordList _wordlist;
        protected RunnerStatus _runnerStatus;
        protected bool _theEnd;
        protected bool _run;
        protected int _position;
        protected int _bot;

        #endregion

        #region Properties (public)

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
        public virtual int Active { get { return _taskList.Where(t => !t.IsCompleted).ToList().Count; } }
        public string Elapsed
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        #endregion

        #region Events (public)

        public virtual event EventHandler<StartEventArgs> OnStarted;
        public virtual event EventHandler<StopEventArgs> OnStopped;
        public virtual event EventHandler<EventArgs> OnCompleted;
        public virtual event Config OnConfig;
        public virtual event EventHandler<ExceptionEventArgs> OnException;

        #endregion

        #region Constractor

        public TaskRunner()
        {
            _taskList = new List<Task>();
            _datapool = new DataPool();
            _log = new Log();
            _save = new Save();
            _proxylist = new ProxyPool();
            _stopwatch = new Stopwatch();
            _wordlist = null;
            _runnerStatus = RunnerStatus.Idle;
            _theEnd = false;
            _run = false;
            _position = 0;
            _bot = 0;
        }

        #endregion

        #region Methods (public)

        public void SetWordlist(IEnumerable<string> combolist, int position = 0)
        {
            if (combolist == null)
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

        public IEnumerable<string> GetProxylist(string address)
        {
            if (System.IO.File.Exists(address))
            {
                try
                {
                    return System.IO.File.ReadAllLines(address);
                }
                catch
                {
                    throw new Exception("Cannot access your file.");
                }
            }
            if (Regex.IsMatch(address, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
            {
                try
                {
                    string input = HttpWebRequest(address);
                    MatchCollection mc = new Regex("\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\:[0-9]{1,5}\\b").Matches(input);
                    List<string> proxylist = new List<string>();
                    foreach (object prx in mc)
                    {
                        Match match = (Match)prx;
                        proxylist.Add(match.ToString());
                    }
                    return proxylist;
                }
                catch
                {
                    throw new Exception("Please enter valid proxy link and try.");
                }
            }
            else
                throw new Exception("The URL format is incorrect.");
        }

        public virtual void Start(int bot)
        {
            if (!_run)
            {
                if (_wordlist == null)
                    throw new Exception("Wordlist is null.");
                if (!_wordlist.HasNext)
                    throw new Exception("Wordlist is null.");

                _theEnd = false;
                _run = true;

                _taskList.Clear();

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
                    Task task = Task.Factory.StartNew(() => { Config(_cts.Token); }, _cts.Token);
                    _taskList.Add(task);
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

        #endregion

        #region Methods (private)

        private void Config(CancellationToken ct)
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

        private string HttpWebRequest(string url)
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

        #endregion
    }
}
