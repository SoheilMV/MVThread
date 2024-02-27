namespace MVThread
{
    public class CredentialsList : IWordList
    {
        private readonly object _obj = new object();
        private readonly int _listsLength = 10000;
        private ComboType _type;
        private IEnumerable<IEnumerable<string>> _list1;
        private IEnumerable<IEnumerable<string>> _list2;

        private int _calculate = 0;
        private int _row = 0;
        private int _cell = 0;

        public CredentialsList(IEnumerable<string> usernames, IEnumerable<string> passwords, ComboType type = ComboType.ChangePass, int position = 0)
        {
            _list1 = GetLists(usernames);
            _list2 = GetLists(passwords);

            _type = type;

            if (position > 0 && position < Count)
                SetPosition(position);
        }

        public int Position
        {
            get
            {
                return _calculate;
            }
        }

        public int Count
        {
            get
            {
                return (GetUsersCount() * GetPasswordsCount());
            }
        }

        public bool HasNext
        {
            get
            {
                return Count - _calculate > 0;
            }
        }

        public string GetData()
        {
            lock (_obj)
            {
                var userPosition = GetPosition(_row);
                var passPosition = GetPosition(_cell);
                string user = _list1.ElementAt(userPosition.row).ElementAt(userPosition.index);
                string pass = _list2.ElementAt(passPosition.row).ElementAt(passPosition.index);
                SetPosition(1);
                return $"{user.Trim()}{Constant.Wordlist_Separator}{pass.Trim()}";
            }
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

        private void SetPosition(int position)
        {
            for (int i = 0; i < position; i++)
            {
                if (_type == ComboType.ChangePass)
                {
                    if (_cell == GetPasswordsCount() - 1)
                    {
                        _cell = 0;
                        _row++;
                    }
                    else
                        _cell++;
                }
                else
                {
                    if (_row == GetUsersCount() - 1)
                    {
                        _row = 0;
                        _cell++;
                    }
                    else
                        _row++;
                }
                _calculate++;
            }
        }

        private int GetUsersCount()
        {
            int count = 0;
            foreach (var list in _list1)
                count += list.Count();
            return count;
        }

        private int GetPasswordsCount()
        {
            int count = 0;
            foreach (var list in _list2)
                count += list.Count();
            return count;
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
    }
}
