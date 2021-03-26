using MVThread.Events;
using MVThread.Wordlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread
{
    public interface IRunner
    {
        event EventHandler<EventArgs> OnStarted;
        event EventHandler<StopEventArgs> OnStopped;
        event EventHandler<EventArgs> OnCompleted;
        event EventHandler<DataEventArgs> OnConfig;
        event EventHandler<ExceptionEventArgs> OnException;
        bool IsCompleted { get; }
        RunnerStatus Status { get; }
        string LogAddress { get; set; }
        void SetWordlist(List<string> combolist, int position = 0);
        void SetWordlist(List<string> userlist, List<string> passlist, ComboType type, int position = 0);
        void Start(int bot);
        void Stop();
    }
}
