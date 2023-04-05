using System.Collections.Generic;

namespace MVThread
{
    public class ParametersStorage : List<KeyValuePair<string, Parameters>>
    {
        private readonly object _lock = new object();

        public ParametersStorage()
        {
        }

        public Parameters this[string id]
        {
            get
            {
                return GetID(id);
            }
        }

        public void Add(string id)
        {
            lock (_lock)
            {
                this.Add(new KeyValuePair<string, Parameters>(id, new Parameters()));
            }
        }

        public void Remove(string id)
        {
            lock (_lock)
            {
                if (ContainsID(id))
                {
                    base.RemoveAll(d => d.Key == id.Trim());
                }
            }
        }

        public bool ContainsID(string id)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var kv = this[i];
                if (kv.Key == id.Trim())
                    return true;
            }
            return false;
        }

        private Parameters GetID(string id)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var kv = this[i];
                if (kv.Key == id.Trim())
                    return kv.Value;
            }
            return null;
        }
    }

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
