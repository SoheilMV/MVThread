namespace MVThread
{
    public interface IWordList
    {
        int Position { get; }
        int Count { get; }
        bool HasNext { get; }
        string GetData();
    }
}
