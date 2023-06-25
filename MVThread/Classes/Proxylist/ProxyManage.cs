namespace MVThread
{
    public class ProxyManage : IProxyInfo
    {
        private readonly object _obj = new object();
        private SecureRandom _random;
        private List<Uri> _used;
        private List<Uri> _bads;
        private List<Uri> _bans;

        public IProxyPool ProxyPool { get; private set; }
        public int Count => ProxyPool.Count;
        public int InUse => _used.Count;
        public int Bads => _bads.Count;
        public int Bans => _bans.Count;
        public int Goods => Count - Bads - Bans;
        public bool IsEmpty => Count == 0;
        public bool Freez { get; set; }

        public ProxyManage()
        {
            _random = new SecureRandom();
            _used = new List<Uri>();
            _bads = new List<Uri>();
            _bans = new List<Uri>();
            ProxyPool = new ProxyPool();
            Freez = false;
        }

        public Proxy Get()
        {
            lock (_obj)
            {
                if (!IsEmpty)
                {
                    if (Count > 0 && Goods <= InUse)
                        ClearAll();

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
                if (!Freez)
                    _bads.Add(proxy!.Address);
            }
        }

        public void AddBan(Proxy proxy)
        {
            lock (_obj)
            {
                if (!Freez)
                    _bans.Add(proxy!.Address);
            }
        }

        #region Methods (internal)

        internal void Unuse(Proxy proxy)
        {
            lock (_obj)
            {
                if (!Freez)
                    _used.Remove(proxy!.Address);
            }
        }

        internal void ClearAll()
        {
            ClearBads();
            ClearBans();
        }

        #endregion

        #region Methods (private)

        private void Use(Proxy proxy)
        {
            lock (_obj)
            {
                if (!Freez)
                    _used.Add(proxy!.Address);
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

        #endregion
    }
}
