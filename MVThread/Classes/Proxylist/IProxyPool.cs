﻿using System.Collections.Generic;

namespace MVThread
{
    public interface IProxyPool
    {
        List<Proxy> Proxies { get; }
        int Count { get; }
        bool IsEmpty { get; }
        void SetProxylist(List<Proxy> proxies, bool join);
        Proxy Get();
    }
}
