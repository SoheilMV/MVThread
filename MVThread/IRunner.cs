using MVThread.Events;
using MVThread.Proxylist;
using MVThread.Wordlist;
using System;
using System.Collections.Generic;

namespace MVThread
{
    public interface IRunner
    {
        event EventHandler<StartEventArgs> OnStarted;
        event EventHandler<StopEventArgs> OnStopped;
        event EventHandler<EventArgs> OnCompleted;
        event ResolveConfig OnConfig;
        event EventHandler<ExceptionEventArgs> OnException;
        bool IsRunning { get; }
        bool IsCompleted { get; }
        RunnerStatus RunnerStatus { get; }
        IProxyPool ProxyPool { get; }
        Progress Progress { get; }
        int CPM { get; }
        int Active { get; }
        string Elapsed { get; }
        string LogAddress { get; set; }
        void SetWordlist(IEnumerable<string> combolist, int position = 0);
        void SetWordlist(IEnumerable<string> userlist, IEnumerable<string> passlist, ComboType type, int position = 0);
        void SetProxylist(IEnumerable<string> proxylist, ProxyType type, bool join = false);
        void SetProxylist(string url, ProxyType type, bool join = false);
        void Start(int bot);
        void Stop();
    }
}
