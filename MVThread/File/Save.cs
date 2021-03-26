using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.File
{
    internal class Save : ISave
    {
        private readonly object _obj = new object();

        public void Write(string file, string value)
        {
            lock (_obj)
            {
                using (StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8))
                {
                    sw.Write(value);
                    sw.Flush();
                }
            }
        }

        public void WriteLine(string file, string value)
        {
            lock (_obj)
            {
                using (StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8))
                {
                    sw.WriteLine(value);
                    sw.Flush();
                }
            }
        }
    }
}
