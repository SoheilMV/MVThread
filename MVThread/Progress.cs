using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread
{
    public class Progress
    {
        public Progress(int max, int value)
        {
            MaxValue = max;
            Value = value;
        }

        public int MaxValue { get; private set; }
        public int Value { get; private set; }
    }
}
