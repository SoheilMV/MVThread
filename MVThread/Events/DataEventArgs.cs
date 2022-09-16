using System;

namespace MVThread
{
    public sealed class DataEventArgs : EventArgs
    {
        public int Retry { get; internal set; }
        public string Data { get; internal set; }
        public Proxy Proxy { get; internal set; }
        public ISave Save { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
