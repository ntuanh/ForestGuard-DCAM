using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
    using BsonData;
    public class DocumentMap<T> : Dictionary<string, T>
        where T: Document
    {
        new public T this[string objectId]
        {
            get
            {
                if (string.IsNullOrEmpty(objectId))
                {
                    return default(T);
                }

                T value;
                TryGetValue(objectId, out value);

                return value;
            }
            set
            {
                if (base.ContainsKey(objectId))
                {
                    base[objectId] = value;
                }
                else
                {
                    base.Add(objectId, value);
                }
            }
        }
        public void Add(T doc)
        {
            this[doc.ObjectId] = doc;
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var doc in items)
            {
                this.Add(doc);
            }
        }
        new public DocumentMap<T> Clear()
        {
            base.Clear();
            return this;
        }
    }
    public class DocumentList : List<Document>
    {
        public DocumentList() { }
        public DocumentList(IEnumerable<Document> items)
        {
            this.AddRange(items);
        }
        public Document Push(object value)
        {
            
            Document doc = value as Document;
            if (doc == null)
            {
                doc = Document.FromObject(value);
            }
            base.Add(doc);

            return doc;
        }
        public DocumentGroup[] GroupBy(params string[] names)
        {
            var map = new Dictionary<string, DocumentGroup>();
            foreach (var doc in this)
            {
                DocumentGroup ext;
                var key = doc.Unique(names);

                if (!map.TryGetValue(key, out ext))
                {
                    map.Add(key, ext = new DocumentGroup());
                    ext.Copy(doc, names);
                }
                ext.Records.Add(doc);
            }
            return map.Values.ToArray();
        }
        public string Join(string seperator, string fieldName)
        {
            var s = "";
            foreach (var e in this)
            {
                var a = e.GetString(fieldName);
                if (s.Length > 0) s += seperator;

                s += a;
            }
            return s;
        }

        public DocumentList ChangeType<T>() where T: Document, new()
        {
            var lst = new DocumentList();
            foreach (var e in this)
            {
                Document a = e;
                if (!(e is T)) a = e.ChangeType<T>();
                lst.Add(a);
            }
            return lst;
        }

        public Dictionary<string, Document> ToDictionary(string key)
        {
            var map = new Dictionary<string, Document>();
            foreach (var e in this)
            {
                var k = key == null ? e.ObjectId : e.GetString(key);
                if (k != null) map.Add(k, e);
            }
            return map;
        }
    }
    public class DocumentMap : DocumentMap<Document> { }
    public class DocumentGroup : Document
    {
        DocumentList records;
        public DocumentList Records
        {
            get
            {
                if (records == null)
                {
                    Push("items", records = new DocumentList());
                }
                return records;
            }
        }
    }
}

namespace System
{
    public class DocumentList<T> : DocumentList
        where T: Document, new()
    {
        protected Func<string, T> find_one;
        public DocumentList(BsonData.Collection table)
        {
            find_one = (id) => table.Find<T>(id);
        }
        public T Add(string id)
        {
            var e = find_one(id);
            if (e != null) base.Add(e);

            return e;
        }
        public void Each(Action<T> callback)
        {
            foreach (var e in this) callback((T)e);
        }
    }
}
