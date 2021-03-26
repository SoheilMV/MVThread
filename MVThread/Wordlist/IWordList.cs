using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Wordlist
{
    public interface IWordList
    {
        int Position { get; }
        int Count { get; }
        bool HasNext { get; }
        string GetData();
    }
}
