using MVThread.Wordlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Events
{
    public sealed class StopEventArgs : EventArgs
    {
        public IWordList WordList { get; internal set; }
    }
}
