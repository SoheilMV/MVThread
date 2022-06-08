using MVThread.File;
using MVThread.Wordlist;
using System;

namespace MVThread.Events
{
    public sealed class StopEventArgs : EventArgs
    {
        public IWordList WordList { get; internal set; }
        public ISave Save { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
