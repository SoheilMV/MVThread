using System;

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
        public double Percentage
        {
            get
            {
                return MaxValue > 0 ? Math.Round((double)((Value * 100) / MaxValue)) : 0;
            }
        }
    }
}
