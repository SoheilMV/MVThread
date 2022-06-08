using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Proxylist
{
    public interface IProxyPool
    {
        List<Proxy> Proxies { get; }
        int Count { get; }
        bool Less { get; }
        void SetProxylist(List<Proxy> proxies, bool join);
        Proxy Get();
    }
}
