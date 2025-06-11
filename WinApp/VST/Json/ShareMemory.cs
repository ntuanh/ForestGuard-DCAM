using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace System.Server
{
    using SE = System.Text.Encoding;
    public partial class Memory : IDisposable
    {
        Mutex _mutex;
        MemoryMappedFile _mmf;

        string _name;
        public string Name => _name;

        public Memory(string name)
        {
            _name = name.ToLower();
        }

        public Memory Create(int MB)
        {
            try
            {
                string mutexName = _name + "-mutex";

                if (!Mutex.TryOpenExisting(mutexName, out _mutex))
                {
                    bool b;
                    _mutex = new Mutex(true, mutexName, out b);
                    _mutex.ReleaseMutex();

                    _mmf = MemoryMappedFile.CreateNew(_name, MB << 20);
                }
            }
            catch
            {
            }
            return this;
        }

        public static bool Exists(string name)
        {
            try
            {
                using (MemoryMappedFile.OpenExisting(name.ToLower())) { }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool Created => _mmf != null;

        protected bool Open(Action callback)
        {
            if (!Created)
            {
                try
                {
                    _mmf = MemoryMappedFile.OpenExisting(_name);
                    _mutex = Mutex.OpenExisting(_name + "-mutex");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ShareMemory: " + ex.Message);
                    return false;
                }
            }
            if (Created)
            {
                callback?.Invoke();
            }
            return true;
        }
        public void Dispose()
        {
            if (Created)
            {
                _mutex.Dispose();
                _mmf.Dispose();

                _mmf = null;
                _mutex = null;
            }
        }
        bool _enabled;
        public bool Enabled
        {
            get
            {
                try
                {
                    bool res = false;
                    this.Streaming(stream => {
                        int b = stream.ReadByte();
                        res = (b != 255 && b > 0);
                    });
                    return res;
                }
                catch
                {

                }
                return false;
            }
            set
            {
                if (_enabled == value) return;

                _enabled = value;
                this.Streaming(stream => {
                    if (_enabled)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            stream.WriteByte(0);
                        }
                    }
                    else
                    {
                        stream.WriteByte(255);
                    }
                });
            }
        }
        public void Streaming(Action<MemoryMappedViewStream> callback)
        {
            this.Open(() => {
                _mutex.WaitOne();
                using (var stream = _mmf.CreateViewStream())
                {
                    callback.Invoke(stream);
                }
                _mutex.ReleaseMutex();
            });
        }
        public void Streaming(Action<MemoryMappedViewStream, int> callback)
        {
            this.Open(() => {
                _mutex.WaitOne();
                using (var stream = _mmf.CreateViewStream())
                {
                    int len = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        len |= (stream.ReadByte() << (i << 3));
                    }

                    callback.Invoke(stream, len);
                }
                _mutex.ReleaseMutex();
            });
        }
    }
    public class ShareMemory : Memory
    {
        public void SetDataLength(MemoryMappedViewStream s, int v)
        {
            s.Position = 0;
            for (int i = 0; i < 4; i++, v >>= 8)
            {
                s.WriteByte((byte)(v & 255));
            }
        }
        public ShareMemory(string name) : base(name) { }
        public List<T> ReadObject<T>()
        {
            List<T> lst = null;
            Streaming((stream, len) => {
                if (len == 0) { return; }

                var buff = new byte[len];
                stream.Read(buff, 0, len);

                try
                {
                    var v = SE.UTF8.GetString(buff);
                    lst = JArray.Parse(v).ToObject<List<T>>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ShareMemory:\n" + e);
                }
                SetDataLength(stream, 0);
            });
            return lst;
        }
        public void WriteBytes(byte[] v)
        {
            Streaming((stream, len) => {
                if (len > 0)
                {
                    stream.Position = (--len) + 4;
                }

                stream.WriteByte((byte)(len == 0 ? '[' : ','));
                stream.Write(v, 0, v.Length);
                stream.WriteByte((byte)']');

                SetDataLength(stream, len + v.Length + 2);
            });
        }
        public void WriteObject(object value)
        {
            WriteBytes(SE.UTF8.GetBytes(JObject.FromObject(value).ToString()));
        }
        public void AsyncReading<T>(int interval, Action<List<T>> callback)
        {
            var ts = new ThreadStart(() => {
                while (true)
                {
                    var s = this.ReadObject<T>();
                    if (s != null)
                    {
                        callback.Invoke(s);
                    }
                    Thread.Sleep(interval);
                }
            });
            new Thread(ts).Start();
        }
        static public bool Open(string name, Action<ShareMemory> callback)
        {
            var sm = new ShareMemory(name);
            sm.Open(() => {
                callback?.Invoke(sm);
            });
            return sm.Created;
        }
    }
}

namespace System.Server
{
    partial class Memory
    {
        static public ShareMemory Manager => new ShareMemory("Memory");
        static public ShareMemory Monitoring => new ShareMemory("Monitoring");
        static public ShareMemory Broker => new ShareMemory("Broker");
        static public ShareMemory WebServer => new ShareMemory("WebServer");
        static public ShareMemory Account => new ShareMemory("Account");
    }
}