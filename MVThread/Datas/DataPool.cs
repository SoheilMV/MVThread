using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVThread.Datas
{
    internal class DataPool
    {
        private List<int> _datas = new List<int>();

        /// <summary>The checks per minute.</summary>
        public int CPM { get; private set; }

        public DataPool()
        {
        }

        public void Add()
        {
            _datas.Add(Environment.TickCount);
            Update();
        }

        public void Clear()
        {
            _datas.Clear();
            Update();
        }

        public void Update()
        {
            try
            {
                var now = DateTime.Now;
                _datas = _datas.Where(t => Environment.TickCount - t < 60000).ToList();
                CPM = _datas.Count;
            }
            catch
            {
            }
        }
    }
}
