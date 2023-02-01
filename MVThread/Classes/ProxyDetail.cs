using System;
using System.Threading;

namespace MVThread
{
    public class ProxyDetail : IDisposable
    {
        private readonly object _obj = new object();
        private ProxyManage _proxyManage;

        public Proxy Proxy { get; private set; }
        public int Goods => _proxyManage != null ? _proxyManage.GoodsCount : 0;
        public int InUse => _proxyManage != null ? _proxyManage.InUse : 0;
        public int Bads => _proxyManage != null ? _proxyManage.BadsCount : 0;
        public int Bans => _proxyManage != null ? _proxyManage.BansCount : 0;
        public int Count => _proxyManage != null ? _proxyManage.Count : 0;
        public bool IsProxyless => _proxyManage != null ? _proxyManage.ProxyPool.IsEmpty : true;

        public ProxyDetail()
        {
            _proxyManage = null;
            Proxy = null;
        }

        public ProxyDetail(ProxyManage proxyManage, CancellationToken cancellationToken)
        {

            _proxyManage = proxyManage;

            Retry:
            Proxy = null;
            if (_proxyManage != null)
            {
                if (!IsProxyless)
                {
                    Proxy = _proxyManage?.Get();
                    if (Proxy == null)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            Thread.Sleep(10);
                            goto Retry;
                        }
                    }
                }
            }
        }

        public void Bad()
        {
            if (Proxy != null)
                _proxyManage?.AddBad(Proxy);
        }

        public void Ban()
        {
            if (Proxy != null)
                _proxyManage?.AddBan(Proxy);
        }

        public void Dispose()
        {
            if (Proxy != null)
                _proxyManage?.Unuse(Proxy);
        }
    }
}
