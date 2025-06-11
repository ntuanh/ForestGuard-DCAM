using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Network
{
    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Newtonsoft.Json.Linq.JObject Value { get; set; }

        public T GetObject<T>()
        {
            if (Value == null) return default(T);
            return Value.ToObject<T>();
        }
    }
}
