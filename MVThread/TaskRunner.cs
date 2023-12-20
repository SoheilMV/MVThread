using System.Net;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Text.RegularExpressions;

namespace MVThread
{
    public class TaskRunner : IRunner
    {
        #region Fields (private)

        private SynchronizationContext _syncContext;
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
        protected ParametersStorage _storage;
        protected bool _theEnd;
        protected bool _run;
        protected int _position;
        protected int _bot;
        protected bool _useAsync = false;

        #endregion

        #region Properties (public)

        public bool IsRunning => RunnerStatus == RunnerStatus.Started;
        public bool IsCompleted => RunnerStatus == RunnerStatus.Completed;
        public bool IsStopped => RunnerStatus == RunnerStatus.Stopped;
        public RunnerStatus RunnerStatus => _runnerStatus;
        public IProxyInfo ProxyInfo => _proxyManage;
        public string LogAddress { get { return _log.Address; } set { _log.Address = value; } }
        public Progress Progress => _wordlist == null || _wordlist.Position == 0 ? new Progress(0, 0) : IsCompleted || IsStopped ? new Progress(_wordlist.Count, _wordlist.Count) : new Progress(_wordlist.Count, _wordlist.Position - 1);
        public int CPM => _datapool.CPM;
        public virtual int Active => _taskList.Where(t => !t.IsCompleted).ToList().Count;
        public string Elapsed
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                return string.Format(Constant.ElapsedFormat, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
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
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
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
                throw new ArgumentNullException(Constant.Wordlist_ArgumentNullException);
            if (combolist.Count() == 0)
                throw new ArgumentNullException(Constant.Wordlist_ArgumentNullException);
            if (position < 0 || position > combolist.Count() - 1)
                throw new ArgumentNullException(Constant.Wordlist_Position_ArgumentNullException.Replace("<Count>", (combolist.Count() - 1).ToString()));

            _position = position;
            _wordlist = new ComboList(combolist, position);
        }

        public void SetWordlist(IEnumerable<string> userlist, IEnumerable<string> passlist, ComboType type, int position = 0)
        {
            if (userlist == null || passlist == null)
                throw new ArgumentNullException(Constant.Wordlist_ArgumentNullException);
            if (userlist.Count() == 0)
                throw new ArgumentNullException(Constant.Wordlist_ArgumentNullException);
            if (passlist.Count() == 0)
                throw new ArgumentNullException(Constant.Wordlist_ArgumentNullException);
            if (position < 0 || position > (userlist.Count() * passlist.Count()) - 1)
                throw new ArgumentNullException(Constant.Wordlist_Position_ArgumentNullException.Replace("<Count>", ((userlist.Count() * passlist.Count()) - 1).ToString()));

            _position = position;
            _wordlist = new CredentialsList(userlist, passlist, type, position);
        }

        public void SetProxylist(IEnumerable<string> proxylist, ProxyType type, bool join = false)
        {
            if (proxylist == null)
                throw new ArgumentNullException(Constant.Proxylist_ArgumentNullException);
            if (proxylist.Count() == 0)
                throw new ArgumentNullException(Constant.Proxylist_ArgumentNullException);

            List<Proxy> list = new List<Proxy>();
            foreach (var address in proxylist)
            {
                if (Regex.IsMatch(address, Constant.ProxyPattern,RegexOptions.Compiled))
                {
                    Proxy proxy = new Proxy(type, address);
                    list.Add(proxy);
                }
            }
            _proxyManage.ClearAll();
            _proxyManage.ProxyPool.SetProxylist(list, join);
        }

        public async Task<IEnumerable<string>> GetProxylistAsync(string address, IWebProxy? proxy = default)
        {
            string input = string.Empty;

            if (File.Exists(address))
            {
                try
                {
                    using (var stream = new FileStream(address, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            input = await reader.ReadToEndAsync();
                        }
                    }
                }
                catch
                {
                    throw new Exception(Constant.Proxylist_FileException);
                }
            }
            else if (Regex.IsMatch(address, Constant.UrlPattern, RegexOptions.Compiled))
            {
                try
                {
                    input = await HttpRequest(address, proxy);
                }
                catch
                {
                    throw new Exception(Constant.Proxylist_LinkException);
                }
            }
            else
                throw new Exception(Constant.Proxylist_AddressException);

            return ProxyFinder(input);
        }

        public IEnumerable<string> GetProxylist(string address, IWebProxy? proxy = default)
        {
            return GetProxylistAsync(address, proxy).Result;
        }

        public virtual void Start(int bot)
        {
            if (!_run)
            {
                if (_wordlist == null)
                    throw new Exception(Constant.Wordlist_ArgumentNullException);
                if (!_wordlist.HasNext)
                    throw new Exception(Constant.Wordlist_ArgumentNullException);
                if (UseAsync && OnConfigAsync == null)
                    throw new Exception(Constant.OnConfigAsync_Exception);
                if (!UseAsync && OnConfig == null)
                    throw new Exception(Constant.OnConfig_Exception);

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
                        ConfigStatus? status = ConfigStatus.OK;
                        if (_useAsync)
                        {
                            status = OnConfigAsync?.Invoke(this, new DataEventArgs()
                            {
                                BotID = id,
                                Parameters = _storage[id]!,
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
                                Parameters = _storage[id]!,
                                Retry = retry,
                                Data = data,
                                ProxyDetail = proxyDetail,
                                Save = _save,
                                Log = _log
                            });
                        }

                        switch (status)
                        {
                            case ConfigStatus.OK:
                                _datapool.Add();
                                break;
                            case ConfigStatus.Retry:
                                retry++;
                                goto Retry;
                            case ConfigStatus.TheEnd:
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

                    retry++;
                    goto Retry;
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
                        _syncContext.Post(_ =>
                        {
                            try
                            {
                                OnStopped?.Invoke(this, new StopEventArgs() { WordList = _wordlist, Save = _save, Log = _log });
                            }
                            catch (Exception ex)
                            {
                                OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnStopped", Exception = ex, Log = _log });
                            }
                        }, null);
                    }
                    else if (activeThread == 0)
                    {
                        _run = false;
                        _stopwatch.Stop();
                        _datapool.Clear();
                        _cts.Dispose();
                        _runnerStatus = RunnerStatus.Completed;
                        _syncContext.Post(_ =>
                        {
                            try
                            {
                                OnCompleted?.Invoke(this, new EventArgs());
                            }
                            catch (Exception ex)
                            {
                                OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnCompleted", Exception = ex, Log = _log });
                            }
                        }, null);
                    }
                }
                catch
                {
                }
                Thread.Sleep(100);
            }
        }

        private async Task<string> HttpRequest(string url, IWebProxy? webProxy)
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.All;
                httpClientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                httpClientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                if (webProxy != null)
                    httpClientHandler.Proxy = webProxy;
                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    //httpClient.DefaultRequestHeaders.ExpectContinue = false;
                    using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        var response = await httpClient.SendAsync(req);
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        private IEnumerable<string> ProxyFinder(string input)
        {
            MatchCollection proxies = new Regex(Constant.ProxyPattern, RegexOptions.Compiled).Matches(input);
            List<string> result = new List<string>();
            foreach (object proxy in proxies)
            {
                Match match = (Match)proxy;
                result.Add(match.ToString());
            }
            return result;
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
