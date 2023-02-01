using System;
using System.Linq;
using System.Collections.Generic;

namespace MVThread
{
    public class ProxyManage : IProxyInfo
    {
        private readonly object _obj = new object();
        private bool _freez = false;
        private SecureRandom _random;
        private List<Uri> _used;
        private List<Uri> _bads;
        private List<Uri> _bans;

        public IProxyPool ProxyPool { get; private set; }
        public int Count => ProxyPool != null ? ProxyPool.Count : 0;
        public int InUse => _used.Count;
        public int BadsCount => _bads.Count;
        public int BansCount => _bans.Count;
        public int GoodsCount => Count - BadsCount - BansCount;
        public bool IsEmpty => Count == 0;

        public ProxyManage(IProxyPool proxies, bool freez)
        {
            ProxyPool = proxies;
            _freez = freez;
            _random = new SecureRandom();
            _used = new List<Uri>();
            _bads = new List<Uri>();
            _bans = new List<Uri>();
        }

        public Proxy Get()
        {
            lock (_obj)
            {
                if (!IsEmpty)
                {
                    if (Count > 0 && GoodsCount <= InUse)
                    {
                        ClearBads();
                        ClearBans();
                    }

                    var Proxies = ProxyPool.Proxies.SkipWhile(proxy => _bads.Contains(proxy.Address) || _bans.Contains(proxy.Address) || _used.Contains(proxy.Address)).ToList();
                    int num = _random.Next(Proxies.Count);
                    var prx = Proxies[num];
                    Use(prx);
                    return prx;
                }
                else
                    return null;
            }
        }

        public void AddBad(Proxy proxy)
        {
            lock (_obj)
            {
                if (!_freez)
                    _bads.Add(proxy?.Address);
            }
        }

        public void AddBan(Proxy proxy)
        {
            lock (_obj)
            {
                if (!_freez)
                    _bans.Add(proxy?.Address);
            }
        }

        internal void Unuse(Proxy proxy)
        {
            lock (_obj)
            {
                if (!_freez)
                    _used.Remove(proxy?.Address);
            }
        }

        private void Use(Proxy proxy)
        {
            lock (_obj)
            {
                if (!_freez)
                    _used.Add(proxy?.Address);
            }
        }

        private void ClearBads()
        {
            lock (_obj)
            {
                _bads.Clear();
            }
        }

        private void ClearBans()
        {
            lock (_obj)
            {
                _bans.Clear();
            }
        }
    }
}
