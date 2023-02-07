namespace MVThread
{
    public interface IProxyInfo
    {
        int Count { get; }
        int InUse { get; }
        int Goods { get; }
        int Bads { get; }
        int Bans { get; }
        bool IsEmpty { get; }
    }
}
