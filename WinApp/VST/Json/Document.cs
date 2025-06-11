using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public partial class Document
    {
        #region Request
        public string Name { get => GetString("name"); set => Push("name", value); }
        public string Url { get { return GetString("url"); } set => Push("url", value); }
        public string Action { get { return GetString("action"); } set => Push("action", value); }

        #endregion

        #region Response
        public object Value { get { return GetValueCore("value", false); } set => Push("value", value); }
        public int Code { get { return GetValue<int>("code"); } set => Push("code", value); }
        public string Message { get { return GetString("message"); } set => Push("message", value); }

        public int GetActionCode()
        {
            return Pop<int>("code");
        }
        #endregion

        #region VALUES
        public Document ValueContext => SelectContext("value", v => { });
        public T GetValue<T>() => (T)Convert.ChangeType(Value, typeof(T));
        public static NameMapping NameMapping { get; private set; } = new NameMapping();
        public object GetObject(string name, bool ignoreCase)
        {
            if (ignoreCase)
            {
                name = NameMapping[name];
                if (name == null)
                {
                    return null;
                }
            }
            return this.GetValueCore(name, false);


        }
        #endregion

        #region CONVERT
        public static implicit operator Document(string s)
        {
            return Parse<Document>(s);
        }
        public static implicit operator Document(byte[] bytes)
        {
            return Parse<Document>(bytes.UTF8());
        }
        public static explicit operator byte[](Document context)
        {
            return context.ToString().UTF8();
        }
        #endregion
    }

    public class ObjectMap<T> : Document
    {
        new public T this[string key]
        {
            get
            {
                key = key.ToLower();
                object value;
                TryGetValue(key, out value);

                return (T)value;
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
        public void Add(string key, T value)
        {
            base.Add(key.ToLower(), value);
        }
        new public bool Remove(string key)
        {
            return base.Remove(key.ToLower());
        }

        public ObjectMap<T> Read(string filename)
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
            return this;
        }
        public ObjectMap<T> Write(string filename)
        {
            using (var sw = new System.IO.StreamWriter(filename))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    var obj = JObject.FromObject(this);
                    obj.WriteTo(jw);
                }
            }
            return this;
        }
    }

    public class NameMapping : Dictionary<string, string>
    {
        new public string this[string key]
        {
            get
            {
                key = key.ToLower();
                string value;
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
        public void Add(string key)
        {
            base.Add(key.ToLower(), key);
        }
        public void Add(object obj)
        {
            Add(Document.FromObject(obj));
        }
        public void Add(Document doc)
        {
            foreach (var key in doc.Keys)
            {
                var k = key.ToLower();
                if (base.ContainsKey(k) == false)
                {
                    base.Add(k, key);
                }
            }
        }
        new public bool Remove(string key)
        {
            return base.Remove(key.ToLower());
        }
    }
    public class JsonDocument : Document
    {
        string _path;
        public virtual JsonDocument Read(string path)
        {
            using (var sr = new System.IO.StreamReader(_path = path))
            {
                var content = sr.ReadToEnd();
                var doc = Document.Parse(content);

                NameMapping.Add(doc);
                foreach (var p in doc)
                {
                    this.Add(p.Key, Document.FromObject(p.Value));
                }
            }
            return this;
        }
        public virtual JsonDocument Write()
        {
            using (var sw = new System.IO.StreamWriter(_path))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    var obj = JObject.FromObject(this);
                    obj.WriteTo(jw);
                }
            }
            return this;
        }

        public virtual Document Find(ref string name)
        {
            var ignore = NameMapping[name];
            if (ignore != null)
            {
                name = ignore;
            }
            var v = GetObject(name, true);
            return v == null ? null : (Document)v;
        }
    }
}
