using System;
using System.Threading;

namespace MVThread
{
    public class ProxyDetail : IDisposable
    {
        private ProxyManage _proxyManage;

        public Proxy Proxy { get; private set; }
        public int Goods => _proxyManage.Goods;
        public int InUse => _proxyManage.InUse;
        public int Bads => _proxyManage.Bads;
        public int Bans => _proxyManage.Bans;
        public int Count => _proxyManage.Count;
        public bool IsProxyless => _proxyManage.IsEmpty;

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

        public void AddToBad()
        {
            if (Proxy != null)
                _proxyManage?.AddBad(Proxy);
        }

        public void AddToBan()
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
