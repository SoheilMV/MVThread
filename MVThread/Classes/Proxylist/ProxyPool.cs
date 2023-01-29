using System;
using System.Collections.Generic;

namespace MVThread
{
    public class ProxyPool : IProxyPool
    {
        private readonly SecureRandom _random;

        public List<Proxy> Proxies { get; private set; }
        public int Count { get { return Proxies.Count; } }
        public bool IsEmpty { get { return Proxies.Count == 0; } }

        public ProxyPool()
        {
            _random = new SecureRandom();
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
            if (!IsEmpty)
            {
                int num = _random.Next(Count);
                return Proxies[num];
            }
            else
                return null;
        }
    }
}
