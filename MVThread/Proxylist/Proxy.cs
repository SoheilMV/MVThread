using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Proxylist
{
    public class Proxy
    {
        public string Address { get; internal set; }

        public ProxyType Type { get; internal set; }
    }
}
