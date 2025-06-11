using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace System
{
    public static class TypeExtension
    {
        static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        static string GetMethodKey(Type type, string name) => type.Name + '/' + name;
        public static MethodInfo FindMethod(this Type type, string name, params Type[] args)
        {
            name = name.ToLower();
            string key = GetMethodKey(type, name);

            MethodInfo info = null;
            if (_methods.TryGetValue(key, out info))
            {
                return info;
            }

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.Name.ToLower() == name)
                {
                    var param = method.GetParameters();
                    if (param.Length == args.Length)
                    {
                        int i = 0;
                        for (; i < args.Length; i++)
                        {
                            if (param[i].ParameterType != args[i])
                            {
                                break;
                            }
                        }
                        if (i == param.Length)
                        {
                            _methods.Add(key, method);
                            return method;
                        }
                    }
                }
            }
            return null;
        }
    }
}