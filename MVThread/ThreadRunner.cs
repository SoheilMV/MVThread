using System.Diagnostics;

namespace MVThread
{
    public class ThreadRunner : TaskRunner
    {
        #region Fields (private)

        private List<Thread> _threadList;

        #endregion

        #region Properties (public)

        public override int Active { get { return _threadList.Where(t => t != null).ToList().Count; } }

        #endregion

        #region Events (public)

        public override event EventHandler<StartEventArgs> OnStarted;
        public override event EventHandler<StopEventArgs> OnStopped;
        public override event EventHandler<EventArgs> OnCompleted;
        public override event Config OnConfig;
        public override event ConfigAsync OnConfigAsync;
        public override event EventHandler<ExceptionEventArgs> OnException;

        #endregion

        #region Constractor

        public ThreadRunner(bool useAsync = true)
        {
            _threadList = new List<Thread>();
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

        public override void Start(int bot)
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

                if (!_proxyManage.IsEmpty && _proxyManage.Count < _bot * 2)
                    _proxyManage.Freez = true;
                else
                    _proxyManage.Freez = false;

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

        #endregion

        #region Methods (private)

        private void Config(int num, CancellationToken ct)
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
                }

                if (_storage.ContainsID(id))
                    _storage.Remove(id);
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

        #endregion
    }
}
