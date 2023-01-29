namespace MVThread
{
    public interface ISave
    {
        void Write(string file, string value);
        void WriteLine(string file, string value);
    }
}
