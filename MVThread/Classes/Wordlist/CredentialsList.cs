namespace MVThread
{
    public class CredentialsList : IWordList
    {
        private readonly object _obj = new object();
        private readonly int _listsLength = 10000;
        private ComboType _type;
        private List<IEnumerable<string>> _list1;
        private List<IEnumerable<string>> _list2;

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
                string user = _list1[userPosition.row].ElementAt(userPosition.index);
                string pass = _list2[passPosition.row].ElementAt(passPosition.index);
                SetPosition(1);
                return $"{user.Trim()}{Constant.Wordlist_Separator}{pass.Trim()}";
            }
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
