using System.Net;

namespace MVThread
{
    public interface IRunner
    {
        event EventHandler<StartEventArgs> OnStarted;
        event EventHandler<StopEventArgs> OnStopped;
        event EventHandler<EventArgs> OnCompleted;
        event Config OnConfig;
        event ConfigAsync OnConfigAsync;
        event EventHandler<ExceptionEventArgs> OnException;
        bool IsRunning { get; }
        bool IsCompleted { get; }
        RunnerStatus RunnerStatus { get; }
        IProxyInfo ProxyInfo { get; }
        Progress Progress { get; }
        int CPM { get; }
        int Active { get; }
        string Elapsed { get; }
        bool UseAsync { get; set; }
        string LogAddress { get; set; }
        void SetWordlist(IEnumerable<string> combolist, int position = 0);
        void SetWordlist(IEnumerable<string> userlist, IEnumerable<string> passlist, ComboType type, int position = 0);
        void SetProxylist(IEnumerable<string> proxylist, ProxyType type, bool join = false);
        Task<IEnumerable<string>> GetProxylistAsync(string address, IWebProxy? proxy = default);
        IEnumerable<string> GetProxylist(string address, IWebProxy? proxy = default);
        void Start(int bot);
        void Stop();
    }
}
