using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVThread.Proxylist
{
    internal class ProxyPool : IProxyPool
    {
        private Random _random = new Random(Guid.NewGuid().GetHashCode());

        public List<Proxy> Proxies { get; private set; }
        public int Count { get { return Proxies.Count; } }
        public bool Less { get { return Proxies.Count == 0; } }


        public ProxyPool()
        {
            Proxies = new List<Proxy>();
        }

        public void SetProxylist(List<Proxy> proxies, bool join)
        {
            if (join)
                Proxies.AddRange(proxies);
            else
                Proxies = proxies;
        }

        public Proxy Get()
        {
            if (!Less)
            {
                int num = _random.Next(Count);
                return Proxies[num];
            }
            else
                return null;
        }
    }
}
