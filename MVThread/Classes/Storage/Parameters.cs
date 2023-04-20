using System.Collections.Generic;

namespace MVThread
{
    public class Parameters : List<KeyValuePair<string, object>>
    {
        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
        }

        public void Add(string key, object value)
        {
            Remove(key);
            base.Add(new KeyValuePair<string, object>(key, value));
        }

        public void Remove(string key)
        {
            if (ContainsKey(key))
            {
                base.RemoveAll(d => d.Key == key.Trim());
            }
        }

        public bool ContainsKey(string key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var kv = this[i];
                if (kv.Key == key.Trim())
                    return true;
            }
            return false;
        }

        private object GetValue(string key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var kv = this[i];
                if (kv.Key == key.Trim())
                    return kv.Value;
            }
            return null;
        }
    }
}
