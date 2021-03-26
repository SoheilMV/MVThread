using MVThread.Events;
using MVThread.File;
using MVThread.Wordlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MVThread
{
    public class Runner : IRunner
    {
        private List<Thread> _threadList = new List<Thread>();
        private Log _log = new Log();
        private Save _save = new Save();
        private IWordList _wordlist = null;
        private RunnerStatus _status;
        private bool _run = false;
        private bool _compelete = false;

        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<StopEventArgs> OnStopped;
        public event EventHandler<EventArgs> OnCompleted;
        public event EventHandler<DataEventArgs> OnConfig;
        public event EventHandler<ExceptionEventArgs> OnException;

        public bool IsCompleted { get { return _compelete; } }
        public RunnerStatus Status { get { return _status; } }
        public string LogAddress { get { return _log.Address; } set { _log.Address = value; } }


        public Runner()
        {
            _status = RunnerStatus.Idle;
        }

        public void SetWordlist(List<string> combolist, int position = 0)
        {
            if(combolist == null)
                throw new ArgumentNullException("Wordlist is null.");
            if (combolist.Count == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (position < 0 || position > combolist.Count - 1)
                throw new ArgumentNullException($"Position 0-{combolist.Count - 1}");

            _wordlist = new ComboList(combolist, position);
        }

        public void SetWordlist(List<string> userlist, List<string> passlist, ComboType type, int position = 0)
        {
            if (userlist == null || passlist == null)
                throw new ArgumentNullException("Wordlist is null.");
            if (userlist.Count == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (passlist.Count == 0)
                throw new ArgumentNullException("Wordlist is null.");
            if (position < 0 || position > (userlist.Count * passlist.Count) - 1)
                throw new ArgumentNullException($"Position 0-{(userlist.Count * passlist.Count) - 1}");

            _wordlist = new CredentialsList(userlist, passlist, type, position);
        }

        public void Start(int bot)
        {
            if (!_run)
            {
                if (_wordlist == null)
                    throw new Exception("Wordlist is null.");
                if (!_wordlist.HasNext)
                    throw new Exception("Wordlist is null.");

                _run = true;
                _compelete = false;
                _threadList.Clear();

                _status = RunnerStatus.Started;
                try { OnStarted?.Invoke(this, new EventArgs()); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnStarted", Exception = ex, Log = _log }); }

                for (int i = 0; i < bot; i++)
                {
                    int num = i;
                    Thread thread = new Thread(() => { Config(num); }) { IsBackground = true, Name = $"ID{num}" };
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
                _run = false;
                _compelete = false;
                _status = RunnerStatus.Stopped;
                for (int i = 0; i < _threadList.Count; i++)
                {
                    _threadList[i].Abort();
                }
                try { OnStopped?.Invoke(this, new StopEventArgs() { WordList = _wordlist }); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnStopped", Exception = ex, Log = _log }); }
            }
        }


        private void Config(int num)
        {
            while (_wordlist.HasNext)
            {
                string data = _wordlist.GetData();
                try { OnConfig?.Invoke(this, new DataEventArgs() { Data = data, Save = _save, Log = _log }); } catch (Exception ex) { if (ex.Message != "Thread was being aborted.") OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnConfig", Exception = ex, Log = _log }); }
            }
            _threadList[num] = null;
        }

        private void CompletedTask()
        {
            int position = 0;
            while (_run)
            {
                try
                {
                    var activeThread = _threadList.Where(t => t != null).ToList();
                    if (activeThread.Count == 0)
                    {
                        _run = false;
                        _compelete = true;
                        _status = RunnerStatus.Completed;
                        try { OnCompleted?.Invoke(this, new EventArgs()); } catch (Exception ex) { OnException?.Invoke(this, new ExceptionEventArgs() { Location = "OnCompleted", Exception = ex, Log = _log }); }
                    }
                }
                catch
                {
                }
                Thread.Sleep(100);
            }
        }
    }
}
