using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVThread.Wordlist
{
    internal class CredentialsList : IWordList
    {
        private readonly object _obj = new object();
        private ComboType _type;
        private List<string> _list1;
        private List<string> _list2;

        private int row = 0;
        private int cell = 0;
        private int calc = 0;

        public CredentialsList(List<string> usernames, List<string> passwords, ComboType type = ComboType.ChangePass, int position = 0)
        {
            _type = type;
            _list1 = new List<string>(usernames);
            _list2 = new List<string>(passwords);
            if (position > 0 && position < Count)
            {
                SetPosition(position);
            }
        }

        public int Position
        {
            get
            {
                return calc;
            }
        }

        public int Count
        {
            get
            {
                return (_list1.Count * _list2.Count) - calc;
            }
        }

        public bool HasNext
        {
            get
            {
                return (_list1.Count * _list2.Count) - calc > 0;
            }
        }

        public string GetData()
        {
            lock (_obj)
            {
                if (HasNext)
                {
                    string user = _list1[row];
                    string pass = _list2[cell];
                    if (_type == ComboType.ChangePass)
                    {
                        if (cell == _list2.Count - 1)
                        {
                            cell = 0;
                            row++;
                        }
                        else
                            cell++;
                    }
                    else
                    {
                        if (row == _list1.Count - 1)
                        {
                            row = 0;
                            cell++;
                        }
                        else
                            row++;
                    }
                    calc++;
                    return $"{user}:{pass}";
                }
                else
                    throw new Exception("Empty");
            }
        }

        private void SetPosition(int position)
        {
            for (int i = 0; i < position; i++)
            {
                if (_type == ComboType.ChangePass)
                {
                    if (cell == _list2.Count - 1)
                    {
                        cell = 0;
                        row++;
                    }
                    else
                        cell++;
                }
                else
                {
                    if (row == _list1.Count - 1)
                    {
                        row = 0;
                        cell++;
                    }
                    else
                        row++;
                }
                calc++;
            }
        }
    }
}
