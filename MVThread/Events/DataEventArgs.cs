using MVThread.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Events
{
    public sealed class DataEventArgs : EventArgs
    {
        public string Data { get; internal set; }
        public ISave Save { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
