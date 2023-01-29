using System.Collections.Generic;
using System.Linq;

namespace MVThread
{
    public class ComboList : IWordList
    {
        private readonly object _obj = new object();
        private List<IEnumerable<string>> _list;
        private int _position = 0;

        public ComboList(IEnumerable<string> combos, int position = 0)
        {
            _list = new List<IEnumerable<string>>();

            if (position > 0 && position < Count)
                _position = position;

            var cb = combos.ToList();
            while (cb.Count > 0)
            {
                string[] array = new string[10000];
                if (cb.Count >= 10000)
                {
                    cb.CopyTo(0, array, 0, 10000);
                    _list.Add(array);
                    cb.RemoveRange(0, 10000);
                }
                else
                {
                    _list.Add(cb.ToArray());
                    cb.RemoveRange(0, cb.Count);
                }
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
                int count = 0;
                foreach (var list in _list)
                {
                    count += list.Count();
                }
                return count;
            }
        }

        public bool HasNext
        {
            get
            {
                return _position < Count;
            }
        }

        public string GetData()
        {
            lock (_obj)
            {
                int[] index = GetIndex(_position);
                string account = _list[index[0]].ElementAt(index[1]);
                _position++;
                return account.Trim();
            }
        }

        private int[] GetIndex(int position)
        {
            int i = 0;
            int pos = 0;

            while (true)
            {
                if (position < 10000 || position < ((i + 1) * 10000))
                    break;
                else
                    i++;
            }

            if (position < 10000)
                pos = position;
            else
                pos = position - (i * 10000);

            return new int[] { i, pos};
        }
    }
}
