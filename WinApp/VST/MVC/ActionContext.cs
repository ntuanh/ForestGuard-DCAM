using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class ActionContext : Document
    {
        public Action Invoke { get; set; }
        public List<string> Refs => GetArray<List<string>>(nameof(Refs));
        public string Text { get => GetString(nameof(Text)); set => Push(nameof(Text), value); }

        ActionContextCollection _childs;
        public bool HasChild => Childs.Count != 0;
        public ActionContextCollection Childs 
        {
            get
            {
                if (_childs == null)
                {
                    _childs = GetArray<ActionContextCollection>(nameof(Childs)) ?? new ActionContextCollection();
                }
                return _childs;
            }
            set => _childs = value;
        }

        public ActionContext Add(ActionContext child)
        {
            Childs.Add(child);
            return this;
        }
        public ActionContext Add(string text, string url)
        {
            return Add(new ActionContext(text, url));
        }
        public ActionContext Add(string text, string url, Action action)
        {
            return Add(new ActionContext(text, url, action));
        }
        public ActionContext Add(string text, Action action)
        {
            return Add(new ActionContext(text, action));
        }

        public ActionContext() { }
        public ActionContext(string text) : this(text, null, null)
        {
        }
        public ActionContext(string text, string url) : this(text, url, null)
        {
        }
        public ActionContext(string text, Action action) : this(text, null, action)
        {
        }
        public ActionContext(string text, string url, Action action)
        {
            Text = text;
            Url = url;
            Invoke = action;
        }
    }
    public class ActionManager : Document
    {
        Document _keys;
        Document _actors;

        ActionContext createContext(string key, ActionContext a)
        {
            if (a == null) a = new ActionContext();
            if (key != null)
            {
                a.Url = key;
                if (a.Text == null)
                {
                    a.Text = (string)_keys[key];
                }
            }
            if (a.HasChild)
            {
                foreach (var child in a.Childs)
                {
                    createContext(null, child);
                }
            }
            var r = a.Refs;
            if (r != null)
            {
                foreach (var s in r)
                {
                    var child = CreateActionContext(s);
                    a.Childs.Add(child);
                }
            }
            return a;
        }
        public ActionManager(Document src)
        {
            Copy(src);

            _keys = GetDocument("#");
            _actors = GetDocument("actors");

            var gen = new Document();
            foreach (var actor in _actors.Keys)
            {
                src = _actors.GetDocument(actor);
                var dst = new ActionContext();
                foreach (var top in src.Keys)
                {
                    var a = src.GetDocument<ActionContext>(top);
                    dst.Childs.Add(createContext(top, a));
                }
                gen.Add(actor, dst);
            }
            _actors = gen;
        }
        public ActionContext GetTopMenu(string name)
        {
            return _actors[name] as ActionContext;
        }
        public ActionContext CreateActionContext(string key)
        {
            return CreateActionContext(key, (string)_keys[key]);
        }
        public ActionContext CreateActionContext(string key, string value)
        {
            return new ActionContext(value, key);
        }    
    }
    public class ActionContextCollection : List<ActionContext>
    {
    }
}

