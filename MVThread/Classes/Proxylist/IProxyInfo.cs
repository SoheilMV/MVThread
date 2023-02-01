using System;
using System.Collections.Generic;
using System.Text;

namespace MVThread
{
    public interface IProxyInfo
    {
        int Count { get; }
        int InUse { get; }
        int BadsCount { get; }
        int BansCount { get; }
        int GoodsCount { get; }
        bool IsEmpty { get; }
    }
}
