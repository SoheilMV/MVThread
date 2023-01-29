using System;

namespace MVThread
{
    public sealed class ExceptionEventArgs : EventArgs
    {
        public string Location { get; internal set; }
        public Exception Exception { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
