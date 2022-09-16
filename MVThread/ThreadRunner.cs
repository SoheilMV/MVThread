using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

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
        public override event EventHandler<ExceptionEventArgs> OnException;

        #endregion

        #region Constractor

        public ThreadRunner()
        {
            _threadList = new List<Thread>();
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

        public override void Start(int bot)
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

        #endregion

        #region Methods (private)

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

        #endregion
    }
}
