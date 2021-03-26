using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVThread.Wordlist
{
    internal class ComboList : IWordList
    {
        private readonly object _obj = new object();
        private List<string> _list;
        int _position = 0;

        public ComboList(List<string> combos, int position = 0)
        {
            _list = new List<string>(combos);
            if (position > 0 && position < Count)
            {
                _position = position;
            }
        }

        public int Position
        {
            get
            {
                return _position;
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool HasNext
        {
            get
            {
                return _position < _list.Count;
            }
        }

        public string GetData()
        {
            lock (_obj)
            {
                if (HasNext)
                {
                    string account = _list[_position];
                    _position++;
                    return account;
                }
                else
                    throw new Exception("Empty");
            }
        }
    }
}
