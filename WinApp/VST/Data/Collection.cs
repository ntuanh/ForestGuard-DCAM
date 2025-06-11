using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BsonData
{
    class DocumentState
    {
        public Document Document { get; set; }
        public int State { get; set; }
    }
    class UpdatingState : Dictionary<string, int>
    {
        public const int Deleted = -1;
        public const int Changed = 0;
        public const int Inserted = 1;

        public bool Busy { get; private set; }
        public void Set(string key, int val)
        {
            while (Busy) { }
            int v;
            if (this.TryGetValue(key, out v))
            {
                if (v != val)
                {
                    base[key] = val;
                }
            }
            else
            {
                base.Add(key, val);
            }
        }
        public int Get(string key)
        {
            while (Busy) { }

            int v = int.MaxValue;
            this.TryGetValue(key, out v);
            return v;
        }
        public void Clear(Action<string, int> action)
        {
            if (this.Count > 0)
            {
                Busy = true;
                var ts = new ThreadStart(() =>
                {
                    try
                    {
                        foreach (var p in this)
                        {
                            action(p.Key, p.Value);
                        }
                        base.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Busy = false;
                });
                new Thread(ts).Start();
            }
        }
    }
    public partial class Collection
    {
        //UpdatingState _updating = new UpdatingState();
        
        DocumentList inserting = new DocumentList();
        Dictionary<Document, int> updating = new Dictionary<Document, int>();
        DocumentMap documents = new DocumentMap();

        bool _all_documents_loaded = false;

        public Database Database { get; private set; }
        public string Name { get; private set; }
        public Collection(string name, Database db)
        {
            Database = db;
            Name = name;

            db.Collections.Add(name, this);
        }

        #region LIST
        public bool IsBusy { get; protected set; }
        public int Count => documents.Count;
        protected virtual FileStorage GetStorage(string path) => Database.CollectionStorage.GetSubStorage(this.Name);

        void _store()
        {
            DocumentList writing = new DocumentList(inserting);
            List<string> deleting = new List<string>();

            foreach (var p in updating)
            {
                if (p.Value == UpdatingState.Deleted)
                {
                    deleting.Add(p.Key.ObjectId);
                }
                else
                {
                    writing.Add(p.Key);
                }
            }
            
            if (writing.Count + deleting.Count > 0)
            {
                var s = GetStorage(Name);

                foreach (var e in deleting)
                {
                    s.Delete(e);
                }
                foreach (var e in writing)
                {
                    s.Write(e);
                }

                inserting.Clear();
                updating.Clear();
            }
        }
        public void BeginWrite()
        {
            _store();
        }
        #endregion

        #region FINDING
        public Document Find(string objectId, Action<Document> callback)
        {
            Wait();
            if (string.IsNullOrEmpty(objectId))
            {
                return null;
            }

            Document doc = documents[objectId];
            if (doc == null && _all_documents_loaded == false)
            {
                var s = GetStorage(Name);
                var fi = s.GetFile(objectId);

                if (fi.Exists == false)
                {
                    return null;
                }
                if (documents.ContainsKey(objectId) == false)
                {
                    documents.Add(objectId, doc = FileStorage.CreateDocument(fi));
                }
            }
            if (doc != null && callback != null)
            {
                callback(doc);
            }
            return doc;
        }
        public Document Find(string objectId)
        {
            return Find(objectId, null);
        }
        public T Find<T>(string objectId)
            where T : Document, new()
        {
            var doc = Find(objectId);
            if (doc == null) { return null; }

            if (doc.GetType() == typeof(T)) { return (T)doc; }

            var context = doc.ChangeType<T>();
            documents[objectId] = context;

            return context;
        }

        public void FindAndDelete(string objectId, Action<Document> before)
        {
            Find(objectId, doc => {
                before?.Invoke(doc);

                documents.Remove(objectId);

                updating.Remove(doc);
                updating.Add(doc, UpdatingState.Deleted);
                return;
            });
        }
        public void FindAndUpdate(string objectId, Action<Document> before)
        {
            Find(objectId, doc => {
                before?.Invoke(doc);

                updating.Remove(doc);
                updating.Add(doc, UpdatingState.Changed);
                return;
            });
        }
        #endregion

        #region DB
        public void Wait(Action callback)
        {
            while (IsBusy) { }
            callback?.Invoke();
        }
        public Collection Wait()
        {
            while (IsBusy) { }
            return this;
        }

        public IEnumerable<Document> Select(Func<Document, bool> where)
        {
            var lst = Select();
            if (where != null)
            {
                lst = lst.Where(where);
            }
            return lst;
        }
        public List<string> GetKeys()
        {
            var s = GetStorage(Name);
            var r = new List<string>();
            foreach (var fi in s.Folder.GetFiles())
            {
                r.Add(fi.Name);
            }
            return r;
        }
        public IEnumerable<Document> Select()
        {
            if (_all_documents_loaded == false)
            {
                IsBusy = true;

                var s = GetStorage(Name);
                var r = new List<FileInfo>();
                foreach (var fi in s.Folder.GetFiles())
                {
                    if (documents.ContainsKey(fi.Name) == false)
                    {
                        r.Add(fi);
                    }
                }
                foreach (var fi in r)
                {
                    if (documents.ContainsKey(fi.Name) == false)
                        documents.Add(fi.Name, FileStorage.CreateDocument(fi));
                }
                _all_documents_loaded = true;
                
                IsBusy = false;
            }

            return new DocumentList(documents.Values);
        }
        public DocumentList SelectAll() => (DocumentList)Select();
        public bool Insert(string id, Document doc)
        {
            if (id != null && documents.ContainsKey(id))
            {
                return false;
            }

            if (id == null) id = new ObjectId();
            doc.ObjectId = id;

            inserting.Add(doc);
            documents.Add(id, doc);

            return true;
        }
        public bool Insert(Document doc)
        {
            return Insert(doc.ObjectId, doc);
        }
        public bool Update(string id, Document doc)
        {
            var res = false;
            FindAndUpdate(id, current => {
                res = true;
                if (doc != current)
                {
                    foreach (var p in doc)
                    {
                        current.Push(p.Key, p.Value);
                    }
                }
            });

            return res;

        }
        public bool Update(Document doc)
        {
            return Update(doc.ObjectId, doc);
        }
        public bool Delete(Document doc)
        {
            return Delete(doc.ObjectId);
        }
        public bool Delete(string id)
        {
            var res = false;
            FindAndDelete(id, exist => res = true);

            return res;
        }
        public void DeleteAll()
        {
            Wait();

            GetStorage(Name).Folder.Delete(true);
            documents.Clear();
        }
        public void ClearOrRemove(string id)
        {
            if (id == null)
                DeleteAll();
            else
                Delete(id);
        }
        public void InsertOrUpdate(Document doc)
        {
            var id = doc.ObjectId;
            FindAndUpdate(id, e => {
                doc.Move(e);
                doc = null;
            });
            if (doc != null)
            {
                inserting.Add(doc);
                documents.Add(id, doc);
            }
        }
        
        public virtual DocumentList Import(DocumentList items)
        {
            var lst = new DocumentList();
            foreach (var e in items)
            {
                if (Insert(e)) { lst.Add(e); }
            }
            return lst;
        }
        public DocumentList Import(Document context)
        {
            var items = context.GetArray<DocumentList>("items");
            return Import(items);
        }
        #endregion

        public Document[] DistinctRow(params string[] names)
        {
            var map = new DocumentMap();
            foreach (var doc in this.Select())
            {
                var key = doc.Unique(names);
                if (map.ContainsKey(key) == false)
                {
                    map.Add(key, new Document().Copy(doc, names));
                }
            }
            return map.Values.ToArray();
        }
        public string[] DistinctColumn(string name)
        {
            var map = new DocumentMap();
            foreach (var doc in this.Select())
            {
                var key = doc.GetString(name);
                if (map.ContainsKey(key) == false)
                {
                    map.Add(key, doc);
                }
            }
            return map.Keys.ToArray();
        }
        public DocumentGroup[] GroupBy(params string[] names) => SelectAll().GroupBy(names);
    }
}

namespace System
{
    public static class CollectionExtension
    {
        static public IEnumerable<Document> InnerJoin(this IEnumerable<Document> lst, BsonData.Collection db, string foreignKey, params string[] fields)
        {
            var res = new List<Document>();
            db.Wait(() =>
            {
                foreach (var doc in lst)
                {
                    var d = doc.Clone();
                    db.Find(d.GetString(foreignKey), s => d.Copy(s, fields));

                    res.Add(d);
                }
            });
            return res;
        }
    }

}
