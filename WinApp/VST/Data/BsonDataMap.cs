using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BsonData
{
    public class BsonDataMap<T> : Dictionary<string, T>
    {
        new public T this[string key]
        {
            get
            {
                if (key == null) return default(T);

                key = key.ToLower();
                T value;
                TryGetValue(key, out value);

                return value;
            }
            set
            {
                key = key.ToLower();
                if (base.ContainsKey(key))
                {
                    base[key] = value;
                }
                else
                {
                    base.Add(key, value);
                }
            }
        }
        new public void Add(string key, T value)
        {
            if (key != null)
            {
                key = key.ToLower();
                if (ContainsKey(key) == false)
                {
                    base.Add(key.ToLower(), value);
                }
            }
        }
        new public bool Remove(string key)
        {
            return base.Remove(key.ToLower());
        }

        public void Read(string filename)
        {
            using (var sr = new System.IO.StreamReader(filename))
            {
                var content = sr.ReadToEnd();
                var obj = JObject.Parse(content);

                foreach (var p in obj)
                {
                    this.Add(p.Key, p.Value.ToObject<T>());
                }    
            }
        }

        public void Write(string filename)
        {
            using (var sw = new System.IO.StreamWriter(filename))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    var obj = JObject.FromObject(this);
                    obj.WriteTo(jw);
                }
            }
        }
    }
    public class BsonDataMap : BsonDataMap<Document> { }
}
