namespace MVThread
{
    public class ComboList : IWordList
    {
        private readonly object _obj = new object();
        private readonly int _listsLength = 10000;
        private List<IEnumerable<string>> _list;
        private int _position = 0;

        public ComboList(IEnumerable<string> combos, int position = 0)
        {
            _list = GetLists(combos);

            if (position > 0 && position < Count)
                _position = position;
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
                    count += list.Count();
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
                var position = GetPosition(_position);
                string account = _list[position.row].ElementAt(position.index);
                _position++;
                return account.Trim();
            }
        }

        private (int row, int index) GetPosition(int position)
        {
            int row = 0;
            while (!(position < _listsLength || position < ((row + 1) * _listsLength)))
            {
                row++;
            }

            int index;
            if (position < _listsLength)
                index = position;
            else
                index = position - (row * _listsLength);

            return (row, index);
        }

        private List<IEnumerable<string>> GetLists(IEnumerable<string> list)
        {
            List<IEnumerable<string>> lists = new List<IEnumerable<string>>();
            var items = list.ToList();
            while (items.Count > 0)
            {
                string[] array = new string[_listsLength];
                if (items.Count >= _listsLength)
                {
                    items.CopyTo(0, array, 0, _listsLength);
                    lists.Add(array);
                    items.RemoveRange(0, _listsLength);
                }
                else
                {
                    lists.Add(items.ToArray());
                    items.RemoveRange(0, items.Count);
                }
            }
            return lists;
        }
    }
}
