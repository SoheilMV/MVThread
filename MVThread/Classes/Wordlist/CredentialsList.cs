namespace MVThread
{
    public class CredentialsList : IWordList
    {
        private readonly object _obj = new object();
        private ComboType _type;
        private List<IEnumerable<string>> _list1;
        private List<IEnumerable<string>> _list2;

        private int _calculate = 0;
        private int _row = 0;
        private int _cell = 0;

        public CredentialsList(IEnumerable<string> usernames, IEnumerable<string> passwords, ComboType type = ComboType.ChangePass, int position = 0)
        {
            _list1 = new List<IEnumerable<string>>();
            _list2 = new List<IEnumerable<string>>();

            _type = type;

            var userlist = usernames.ToList();
            while (userlist.Count > 0)
            {
                string[] array = new string[10000];
                if (userlist.Count >= 10000)
                {
                    userlist.CopyTo(0, array, 0, 10000);
                    _list1.Add(array);
                    userlist.RemoveRange(0, 10000);
                }
                else
                {
                    _list1.Add(userlist.ToArray());
                    userlist.RemoveRange(0, userlist.Count);
                }
            }

            var passlist = passwords.ToList();
            while (passlist.Count > 0)
            {
                string[] array = new string[10000];
                if (passlist.Count >= 10000)
                {
                    passlist.CopyTo(0, array, 0, 10000);
                    _list2.Add(array);
                    passlist.RemoveRange(0, 10000);
                }
                else
                {
                    _list2.Add(passlist.ToArray());
                    passlist.RemoveRange(0, passlist.Count);
                }
            }

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
                var userIndex = GetIndex(_row);
                var passIndex = GetIndex(_cell);
                string user = _list1[userIndex[0]].ElementAt(userIndex[1]);
                string pass = _list2[passIndex[0]].ElementAt(passIndex[1]);
                SetPosition(1);
                return $"{user.Trim()}:{pass.Trim()}";
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
            {
                count += list.Count();
            }
            return count;
        }

        private int GetPasswordsCount()
        {
            int count = 0;
            foreach (var list in _list2)
            {
                count += list.Count();
            }
            return count;
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

            return new int[] { i, pos };
        }
    }
}
