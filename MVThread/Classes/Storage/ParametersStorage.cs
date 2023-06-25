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
            lock (_lock)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    var kv = this[i];
                    if (kv.Key == id.Trim())
                        return true;
                }
                return false;
            }
        }

        private Parameters GetID(string id)
        {
            lock (_lock)
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
    }
}
