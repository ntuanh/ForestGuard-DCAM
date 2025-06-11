using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

namespace System.Mvc
{
    public class Engine
    {
        protected static Assembly _assembly;
        protected static string _assemblyName;
        public static Session Session { get; private set; } = new Session();

        protected static ControllerCollection _controllerMap;
        public static T GetController<T>(string name)
            where T : Controller
        {
            return (T)_controllerMap.CreateController(name);
        }

        public static RequestContext RequestContext { get; private set; }
        public static void Register(object app, Action<ActionResult> viewValidateCallback)
        {
            Register(app, null, viewValidateCallback);
        }
        public static void Register(object app, string assemblyName, Action<ActionResult> viewValidateCallback)
        {
            _assembly = app.GetType().Assembly;
            _assemblyName = assemblyName??_assembly.GetName().Name;

            _controllerMap = new ControllerCollection(_assembly);
            ValidateActionResult = viewValidateCallback;
        }

        public static T CreateObject<T>(string name)
        {
            return (T)_assembly.CreateInstance(_assemblyName + '.' + name);
        }

        public static void Execute(string url, params object[] values)
        {
            var request = new RequestContext(url);
            foreach (var v in values)
                request.Values.Add(v);
            Execute(request);
        }
        public static void Execute(RequestContext request)
        {
            RequestContext = request;

            var controller = GetController<Controller>(request.ControllerName);
            controller?.Execute(request, null);
        }
        public static Action<ActionResult> ValidateActionResult;

        static Stack<Thread> _threads;
        public static Thread BeginInvoke(Action action)
        {
            if (_threads == null)
            {
                _threads = new Stack<Thread>();
            }
            while (_threads.Count > 0)
            {
                if (_threads.Peek().IsAlive) break;
                _threads.Pop();
            }
            var th = new Thread(new ThreadStart(action));
            _threads.Push(th);

            th.Start();
            return th;
        }

        public static void Exit()
        {
            if (_threads != null)
            {
                while (_threads.Count > 0)
                {
                    var th = _threads.Pop();
                    if (th.IsAlive)
                        th.Abort();
                }
            }
        }
    }
}
