namespace MVThread
{
    public class ComboList : IWordList
    {
        private readonly object _obj = new object();
        private readonly int _listsLength = 10000;
        private IEnumerable<IEnumerable<string>> _list;
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
                string account = _list.ElementAt(position.row).ElementAt(position.index);
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

        private IEnumerable<IEnumerable<string>> GetLists(IEnumerable<string> list)
        {
            var items = list.ToList();
            while (items.Any())
            {
                if (items.Count() >= _listsLength)
                {
                    yield return items.Take(_listsLength).ToList();
                    items = items.Skip(_listsLength).ToList();
                }
                else
                {
                    yield return items.ToList();
                    items.Clear();
                }
            }
        }
    }
}
