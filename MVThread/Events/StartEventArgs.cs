using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Events
{
    public sealed class StartEventArgs : EventArgs
    {
        public int Bot { get; internal set; }
    }
}
