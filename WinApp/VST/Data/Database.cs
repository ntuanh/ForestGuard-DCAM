using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BsonData
{
    public class FileStorage
    {
        public DirectoryInfo Folder { get; private set; }
        public FileStorage(string path)
        {
            Folder = new DirectoryInfo(path);
            if (Folder.Exists == false)
            {
                Folder.Create();
            }
        }
        public FileStorage GetSubStorage(string name)
        {
            return new FileStorage(Folder.FullName + '/' + name);
        }
        public FileInfo GetFile(Document doc)
        {
            return GetFile(doc.ObjectId);
        }
        public FileInfo GetFile(string id)
        {
            return new FileInfo(Folder.FullName + '/' + id);
        }

        public void Write(Document doc)
        {
            WriteDocument(GetFile(doc), doc);
        }
        public void Delete(string id)
        {
            GetFile(id).Delete();
        }

        public void Delete()
        {
            Folder.Delete(true);
        }
        public List<Document> ReadAll()
        {
            return ReadFolderAsync(Folder);
        }

        public Document ReadOne(string objectId)
        {
            var fi = GetFile(objectId);
            if (fi == null)
            {
                return null;
            }
            return CreateDocument(fi);
        }
        public static Document CreateDocument(FileInfo fi)
        {
            var doc = ReadDocument(fi);
            if (doc.ObjectId == null)
            {
                doc.ObjectId = fi.Name;
                WriteDocument(fi, doc);
            }
            return doc;
        }
        public static List<Document> ReadFolderAsync(DirectoryInfo src)
        {
            var dst = new List<Document>();
            var map = new Dictionary<FileInfo, Document>();
            foreach (var fi in src.GetFiles())
            {
                var doc = CreateDocument(fi);
                dst.Add(doc);
            }
            return dst;
        }

        public static Document ReadDocument(FileInfo fi)
        {
            try
            {
                using (var br = new BsonDataReader(fi.OpenRead()))
                {
                    var serializer = new JsonSerializer();
                    var doc = serializer.Deserialize<Document>(br);

                    return doc;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new Document();
        }
        public static void WriteDocument(FileInfo fi, Document doc)
        {
            try
            {
                using (var bw = new BsonDataWriter(fi.OpenWrite()))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(bw, doc);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class MainDatabase : Database
    {
        public MainDatabase(string name) : base(name)
        {

        }
        
        BsonDataMap<Database> _childs;
        public BsonDataMap<Database> Childs
        {
            get
            {
                if (_childs == null)
                {
                    _childs = new BsonDataMap<Database>();
                }
                return _childs;
            }
        }

        protected virtual string GetChildPath() => PhysicalPath + "/SUBS";
        public void Add(Database db)
        {
            db.Connect(GetChildPath());
            Childs.Add(db.Name, db);
        }
        public void Remove(Database db)
        {
            _childs?.Remove(db.Name);
            db.Clear();
        }
        Thread _storageThread;
        public Database StartStorageThread(int interval)
        {
            Func<Database, bool> run = db => {
                foreach (var col in db.Collections.Values)
                {
                    col.BeginWrite();
                }
                return true;
            };

            _storageThread?.Abort();
            _storageThread = new Thread(() => {

                while (true)
                {
                    run(this);
                    Each(db => run(db));

                    Thread.Sleep(interval);
                }
            });
            _storageThread.Start();
            return this;
        }
        public Database StartStorageThread()
        {
            return this.StartStorageThread(1000);
        }

        void Each(Func<Database, bool> callback)
        {
            if (_childs != null)
            {
                foreach (var db in _childs.Values)
                {
                    if (!callback(db)) return;
                }
            }
        }

        public override bool IsBusy
        {
            get
            {
                var b = base.IsBusy;
                if (!b) {
                    Each(db => !(b = db.IsBusy));
                }
                return b;
            }
        }
        public override void Connect(string connectionString)
        {
            base.Connect(connectionString);
            StartStorageThread();
        }
        public override void Disconnect()
        {
            base.Disconnect();
            _storageThread?.Abort();
        }
    }
    public class Database : BsonDataMap<object>
    {
        #region STORAGE
        public FileStorage Storage { get; private set; }
        FileStorage _documentStorage;
        public FileStorage DocumentStorage
        {
            get
            {
                if (_documentStorage == null) { _documentStorage = Storage.GetSubStorage("Documents"); }
                return _documentStorage;
            }
        }
        FileStorage _collectionStorage;
        public FileStorage CollectionStorage
        {
            get
            {
                if (_collectionStorage == null) { _collectionStorage = Storage.GetSubStorage("Collections"); }
                return _collectionStorage;
            }
        }

        public virtual bool IsBusy
        {
            get
            {
                foreach (var p in Collections)
                {
                    if (p.Value.IsBusy) { return true; }
                }
                return false;
            }
        }
        #endregion

        public string ConnectionString { get; private set; }
        public string Name { get; private set; }
        public string PhysicalPath => ConnectionString + '/' + Name;
        public string DataPath(string name) => ConnectionString + '/' + name;
        public Database(string name)
        {
            Name = name;
        }

        public event Action Connected;
        public virtual void Connect(string connectionString)
        {
            this.ConnectionString = connectionString;
            Storage = new FileStorage(PhysicalPath);

            Connected?.Invoke();
        }
        public virtual void Disconnect()
        {
            Console.Write("Disconnecting ... ");
            while (IsBusy) { }
            Console.WriteLine("done");
        }

        #region COLLECTIONS
        public BsonDataMap<Collection> Collections { get; private set; } = new BsonDataMap<Collection>();
        public Collection GetCollection(string name)
        {
            Collection data = Collections[name];
            if (data == null)
            {
                data = new Collection(name, this);
            }
            return data;
        }
        public Collection GetCollection<T>()
        {
            return GetCollection(typeof(T).Name);
        }
        #endregion

        #region MAPPING
        protected DatabaseMapping _mapping;
        public DatabaseMapping Mapping
        {
            get
            {
                if (_mapping == null)
                {
                    _mapping = new DatabaseMapping(this);
                }
                return _mapping;
            }
        }
        #endregion

        #region QUERY
        public DocumentList Select(string name, string[] fields)
        {
            var db = GetCollection(name).Wait();
            var src = db.Select();
            if (fields != null && fields.Length > 0)
            {
                var lst = new List<Document>();
                Document d = null;
                foreach (var doc in src)
                {
                    lst.Add(d = new Document { ObjectId = doc.ObjectId });
                    foreach (var s in fields)
                    {
                        d.Push(s, doc.SelectPath(s));
                    }
                }
                src = lst;
            }
            return new DocumentList(src);
        }
        #endregion

        new public void Clear()
        {
            Disconnect();
            base.Clear();

            Storage.Delete();
        }
    }

    public class DatabaseMapping : BsonDataMap<object>
    {
        string _fileName;
        public DatabaseMapping(string filename)
        {
            _fileName = filename;
            try
            {
                this.Read(_fileName);
            }
            catch
            {
            }
        }
        public DatabaseMapping(Database db)
            : this(db.PhysicalPath + "/mapping.json") {
        }
        public DatabaseMapping Save()
        {
            this.Write(_fileName);
            return this;
        }
        public DatabaseMapping Add(string context)
        {
            return this.Add(Document.Parse(context));
        }
        public DatabaseMapping Add(Document context)
        {
            foreach (var p in context)
            {
                this[p.Key] = p.Value;
            }
            return this;
        }

        protected virtual Document GetRootDocument(string name)
        {
            var doc = this[name];
            return doc == null ? null : Document.FromObject(doc);
        }
        public virtual Document Get(string path)
        {
            var i = path.IndexOf('.');
            if (i < 0)
            {
                return this.GetRootDocument(path);
            }

            var doc = this.GetRootDocument(path.Substring(0, i));
            var v = doc.SelectPath(path.Substring(i + 1));

            return v == null ? null : Document.FromObject(v);
        }
    }
}
