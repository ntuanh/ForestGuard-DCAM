using System;
using System.Dynamic;
using System.Collections.Generic;

namespace System.Mvc
{
    public class Session
    {
        Dictionary<string, object> map = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                object v;
                map.TryGetValue(key, out v);
                return v;
            }
            set
            {
                if (map.ContainsKey(key))
                    map[key] = value;
                else
                    map.Add(key, value);
            }
        }

        public void Abandon()
        {
            map.Clear();
        }
    }
}
