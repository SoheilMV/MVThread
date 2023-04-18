using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Security.Authentication;

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
        protected ProxyManage _proxyManage;
        protected Stopwatch _stopwatch;
        protected IWordList _wordlist;
        protected RunnerStatus _runnerStatus;
        protected bool _theEnd;
        protected bool _run;
        protected int _position;
        protected int _bot;
        protected bool _useAsync = false;
        protected ParametersStorage _storage;

        #endregion

        #region Properties (public)

        public bool IsRunning => RunnerStatus == RunnerStatus.Started;
        public bool IsCompleted => RunnerStatus == RunnerStatus.Completed;
        public RunnerStatus RunnerStatus => _runnerStatus;
        public IProxyInfo ProxyInfo => _proxyManage;
        public string LogAddress { get { return _log.Address; } set { _log.Address = value; } }
        public Progress Progress => _wordlist == null ? new Progress(0, 0) : new Progress(_wordlist.Count, _wordlist.Position);
        public int CPM => _datapool.CPM;
        public virtual int Active => _taskList.Where(t => !t.IsCompleted).ToList().Count;
        public string Elapsed
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
        }
        public bool UseAsync { get { return _useAsync; } set { _useAsync = value; } }

        #endregion

        #region Events (public)

        public virtual event EventHandler<StartEventArgs> OnStarted;
        public virtual event EventHandler<StopEventArgs> OnStopped;
        public virtual event EventHandler<EventArgs> OnCompleted;
        public virtual event Config OnConfig;
        public virtual event ConfigAsync OnConfigAsync;
        public virtual event EventHandler<ExceptionEventArgs> OnException;

        #endregion

        #region Constractor

        public TaskRunner(bool useAsync = true)
        {
            _taskList = new List<Task>();
            _datapool = new DataPool();
            _log = new Log();
            _save = new Save();
            _proxyManage = new ProxyManage();
            _stopwatch = new Stopwatch();
            _wordlist = null;
            _runnerStatus = RunnerStatus.Idle;
            _theEnd = false;
            _run = false;
            _position = 0;
            _bot = 0;
            _useAsync = useAsync;
            _storage = new ParametersStorage();
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
            foreach (var address in proxylist)
            {
                if (Regex.IsMatch(address, @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(\d+)(?::(\w+))?(?::(\w+))?$")) //https://regex101.com/r/ZmltXj/2
                {
                    Proxy proxy = new Proxy(type, address);
                    list.Add(proxy);
                }
            }
            _proxyManage.ClearAll();
            _proxyManage.ProxyPool.SetProxylist(list, join);
        }

        public async Task<IEnumerable<string>> GetProxylistAsync(string address)
        {
            if (File.Exists(address))
            {
                try
                {
                    return await File.ReadAllLinesAsync(address);
                }
                catch
                {
                    throw new Exception("Cannot access your file.");
                }
            }
            else if (Regex.IsMatch(address, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
            {
                try
                {
                    string input = await HttpRequest(address);
                    MatchCollection mc = new Regex(@"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(\d+)(?::(\w+))?(?::(\w+))?$").Matches(input); //https://regex101.com/r/ZmltXj/2
                    List<string> result = new List<string>();
                    foreach (object prx in mc)
                    {
                        Match match = (Match)prx;
                        result.Add(match.ToString());
                    }
                    return result;
                }
                catch
                {
                    throw new Exception("Please enter valid proxy link and try.");
                }
            }
            else
                throw new Exception("The address format is incorrect.");
        }

        public virtual void Start(int bot)
        {
            if (!_run)
            {
                if (_wordlist == null)
                    throw new Exception("Wordlist is null.");
                if (!_wordlist.HasNext)
                    throw new Exception("Wordlist is null.");
                if (UseAsync && OnConfigAsync == null)
                    throw new Exception("OnConfigAsync event cannot be null.");
                if (!UseAsync && OnConfig == null)
                    throw new Exception("OnConfig event cannot be null.");

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

                if (!_proxyManage.IsEmpty && _proxyManage.Count < _bot * 2)
                    _proxyManage.Freez = true;
                else
                    _proxyManage.Freez = false;

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
                string id = GetID();

                if (!_storage.ContainsID(id))
                    _storage.Add(id);

                Retry:

                if (ct.IsCancellationRequested)
                    break;

                try
                {
                    using (ProxyDetail proxyDetail = new ProxyDetail(_proxyManage, ct))
                    {
                        Status? status = Status.OK;
                        if (_useAsync)
                        {
                            status = OnConfigAsync?.Invoke(this, new DataEventArgs()
                            {
                                BotID = id,
                                Parameters = _storage[id],
                                Retry = retry,
                                Data = data,
                                ProxyDetail = proxyDetail,
                                Save = _save,
                                Log = _log
                            }).Result;
                        }
                        else
                        {
                            status = OnConfig?.Invoke(this, new DataEventArgs()
                            {
                                BotID = id,
                                Parameters = _storage[id],
                                Retry = retry,
                                Data = data,
                                ProxyDetail = proxyDetail,
                                Save = _save,
                                Log = _log
                            });
                        }

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

                if (_storage.ContainsID(id))
                    _storage.Remove(id);
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

        private async Task<string> HttpRequest(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certification, chain, sslPolicyErrors) => true;
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.All;
                httpClientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                httpClientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.ExpectContinue = true;
                    using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        var response = await httpClient.SendAsync(req);
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        #endregion

        #region Methods (protected)

        protected string GetID()
        {
            SecureRandom random = new SecureRandom();
            byte[] buffer = random.NextBytes(32);
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buffer);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        #endregion
    }
}
