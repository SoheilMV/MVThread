using System;

namespace MVThread
{
    public sealed class StartEventArgs : EventArgs
    {
        public int Bot { get; internal set; }
    }
}
