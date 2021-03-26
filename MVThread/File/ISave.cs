using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.File
{
    public interface ISave
    {
        void Write(string file, string value);
        void WriteLine(string file, string value);
    }
}
