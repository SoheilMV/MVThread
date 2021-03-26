using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.File
{
    internal class Log : ILog
    {
        private readonly object _obj = new object();
        public string Address { get; set; } = Environment.CurrentDirectory + "\\Log.txt";

        public void Write(string value)
        {
            lock (_obj)
            {
                using (StreamWriter sw = new StreamWriter(Address, true, Encoding.UTF8))
                {
                    sw.Write(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " - " + value);
                    sw.Flush();
                }
            }
        }

        public void WriteLine(string value)
        {
            lock (_obj)
            {
                using (StreamWriter sw = new StreamWriter(Address, true, Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " - " + value);
                    sw.Flush();
                }
            }
        }
    }
}
