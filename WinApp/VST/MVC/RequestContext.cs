using System;

namespace System.Mvc
{
    public class RequestValues : System.Collections.ArrayList
    {
        static public implicit operator RequestValues(object[] values)
        {
            var r = new RequestValues();
            r.AddRange(values);
            return r;
        }
    }
    public class RequestContext
    {
        public RequestContext() { }
        public RequestContext(string controllerName, string actionName)
        {
            ControllerName = controllerName;
            ActionName = actionName;
        }
        public RequestContext(string url)
        {
            var v = url.Split('/');
            foreach (var s in v)
            {
                if (s == string.Empty)
                    continue;

                if (ControllerName == null)
                {
                    ControllerName = s;
                    continue;
                }

                if (ActionName == null)
                {
                    ActionName = s;
                    continue;
                }

                Values.Add(s);
            }
        }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public RequestValues Values { get; set; } = new RequestValues();
    }

    public class ResponseContext : RequestContext
    {
        public ResponseContext() { }
        public ResponseContext(RequestContext request)
            : base(request.ControllerName, request.ActionName)
        {
        }
        public ResponseContext(string url, params object[] values) : base(url)
        {
            this.Values.AddRange(values);
        }
    }
}
