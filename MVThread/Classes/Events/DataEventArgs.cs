using System;
using System.Collections.Generic;

namespace MVThread
{
    public sealed class DataEventArgs : EventArgs
    {
        public string BotID { get; internal set; }
        public Parameters Parameters { get; internal set; }
        public int Retry { get; internal set; }
        public string Data { get; internal set; }
        public ProxyDetail ProxyDetail { get; internal set; }
        public ISave Save { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
