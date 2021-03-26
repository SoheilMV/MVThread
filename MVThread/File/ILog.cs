using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.File
{
    public interface ILog
    {
        void Write(string value);
        void WriteLine(string value);
    }
}
