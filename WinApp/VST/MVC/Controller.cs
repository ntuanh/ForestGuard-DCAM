using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace System.Mvc
{
    public class ControllerCollection : Dictionary<string, Type>
    {
        public ControllerCollection(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var name = type.Name.ToLower();
                if (name.EndsWith("controller"))
                {
                    this.Add(name.Substring(0, name.Length - 10), type);
                }
            }
        }
        public Controller CreateController(string name)
        {
            Type type;
            if (this.TryGetValue(name.ToLower(), out type))
            {
                return (Controller)Activator.CreateInstance(type);
            }
            return null;
        }
    }

    public partial class Controller
    {
        public ViewDataDictionary ViewData { get; private set; } = new ViewDataDictionary();
        public RequestContext RequestContext { get; set; }

        public string ControllerName
        {
            get
            {
                var name = this.GetType().Name;
                return name.Substring(0, name.Length - 10);
            }
        }

        public MethodInfo GetMethod(string name)
        {
            name = name.ToLower();
            foreach (var method in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name.ToLower() == name)
                {
                    return method;
                }
            }
            return null;
        }

        protected virtual bool CheckMethodParams(MethodInfo method, object[] values)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != values.Length) return false;

            for (int i = 0; i < values.Length; i++)
            {
                var type = parameters[i].ParameterType;
                object v = values[i];
                if (v == null)
                {
                    if (type.IsValueType)
                    {
                        return false;
                    }
                    continue;
                }

                if (v.GetType() != type)
                {
                    try
                    {
                        values[i] = Convert.ChangeType(v, type);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public virtual MethodInfo GetMethod(string name, object[] values)
        {
            name = name.ToLower();
            foreach (var method in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name.ToLower() != name)
                {
                    continue;
                }

                if (CheckMethodParams(method, values))
                    return method;
            }
            return null;
        }

        protected virtual T GetActionResult<T>(string actionName, RequestValues values, Action<T> execute)
        {
            var param = values.ToArray();
            var method = GetMethod(actionName, param);
            if (method == null) { return default(T); }

            this.RequestContext.ActionName = method.Name;

            var result = (T)method.Invoke(this, param);

            execute?.Invoke(result);
            return result;
        }

        protected virtual void ExecuteCore(string actionName, RequestValues values, Action<ActionResult> callBack)
        {
            if (RequestContext == null)
                RequestContext = new RequestContext();
            RequestContext.ActionName = actionName;

            try
            {
                GetActionResult<ActionResult>(actionName, values, result => {
                    if (result != null && result.Handled == false)
                    {
                        if (callBack == null)
                            callBack = Engine.ValidateActionResult;

                        callBack?.Invoke(result);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Execute(RequestContext requestContext, Action<ActionResult> callBack)
        {
            RequestContext = requestContext;
            var name = requestContext.ActionName;
            if (string.IsNullOrEmpty(name))
            {
                name = "Index";
            }

            ExecuteCore(name, requestContext.Values, callBack);
        }

        public void Execute(string actionName, params object[] values)
        {
            ExecuteCore(actionName, values, null);
        }

        protected ActionResult View(string name, object model)
        {
            var view = Engine.CreateObject<IView>(name);
            return View(view, model);
        }

        protected ActionResult View(object model)
        {
            return View(Engine.CreateObject<IView>("Views." + ControllerName + '.' + RequestContext.ActionName), model);
        }
        protected virtual ActionResult View(IView view, object model)
        {
            if (view == null)
            {
                var ctx = RequestContext;
                return new ActionResult {
                    Code = -1,
                    Message = $"Views.{ctx.ControllerName}.{ctx.ActionName} not found.",
                };
            }

            ViewData.Model = model;
            view.Render(model);
            return new ActionResult {
                View = view,
                Controller = this,
            };
        }
        protected ActionResult View()
        {
            return View(null);
        }

        protected ActionResult RedirectToAction(string actionName)
        {
            return Redirect(RequestContext.ControllerName + "/" + actionName);
        }

        protected ActionResult Redirect(string url)
        {
            Engine.Execute(url);
            return new ActionResult { Handled = true };
        }

        public virtual ActionResult Done()
        {
            return new ActionResult { Handled = true };
        }

        protected virtual object Error(int code, string message)
        {
            return new ActionResult {
                Code = code,
                Message = message,
            };
        }
        protected object Error(string message) => Error(1, message);
        protected object Error(int code) => Error(code, null);
    }
}
