using System.Text;

namespace MVThread
{
    public class Log : ILog
    {
        private readonly object _obj = new object();
        public string Address { get; set; } = $"{Environment.CurrentDirectory}\\{Constant.Log_FileName}";

        public string Write(string value)
        {
            lock (_obj)
            {
                string log = Constant.Log_Format.Replace(Constant.Log_DateTime, DateTime.Now.ToString(Constant.Log_DateTimeFormat)).Replace(Constant.Log_Value, value);
                using (StreamWriter sw = new StreamWriter(Address, true, Encoding.UTF8))
                {
                    sw.WriteLine(log);
                    sw.Flush();
                }
                return log;
            }
        }
    }
}
