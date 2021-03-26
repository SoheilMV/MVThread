using MVThread.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Events
{
    public sealed class ExceptionEventArgs : EventArgs
    {
        public string Location { get; internal set; }
        public Exception Exception { get; internal set; }
        public ILog Log { get; internal set; }
    }
}
