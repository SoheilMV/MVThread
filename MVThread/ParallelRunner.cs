using System.Diagnostics;

namespace MVThread
{
    public class ParallelRunner : TaskRunner
    {
        #region Fields (private)

        private SynchronizationContext _syncContext;
        private List<ParallelLoopState> _parallelList;

        #endregion

        #region Properties (public)

        public override int Active => _parallelList.Where(p => p != null && !p.IsStopped).ToList().Count;
        public override RunnerType RunnerType => RunnerType.Parallel;

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

        public ParallelRunner(bool useAsync = true)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _parallelList = new List<ParallelLoopState>();
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

                _parallelList.Clear();

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

                Task.Factory.StartNew(() =>
                {
                    var options = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _bot,
                        TaskScheduler = TaskScheduler.Default,
                    };

                    var result = Parallel.For(0, _bot, options, (long index, ParallelLoopState state) =>
                    {
                        _parallelList.Add(state);
                        Config(_cts.Token, state);
                    });

                    _parallelList.Clear();

                    CompletedParallel(result);
                });
            }
        }

        #endregion

        #region Methods (private)

        private void Config(CancellationToken token, ParallelLoopState state)
        {
            while (_wordlist.HasNext && !_theEnd)
            {
                int retry = 0;
                string data = _wordlist.GetData();
                string id = GetID();

                if (!_storage.ContainsID(id))
                    _storage.Add(id);

                Retry:

                if (token.IsCancellationRequested)
                {
                    state.Stop();
                    break;
                }

                try
                {
                    using (ProxyDetail proxyDetail = new ProxyDetail(_proxyManage, token))
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

        private void CompletedParallel(ParallelLoopResult result)
        {
            if (result.IsCompleted)
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
            else
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
        }

        #endregion
    }
}
