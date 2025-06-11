using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public void RedirectToAction(string action, params object[] args)
        {
            var context = System.Mvc.Engine.RequestContext;
            Request($"{context.ControllerName}/{action}", args);
        }
        static public void Request(string url, params object[] args) => 
            System.Mvc.Engine.Execute(url ?? GetCurrentUrl(), args);
        static public void Post(Document context) => System.Mvc.Engine.Execute(GetCurrentUrl(), context);

        static public string GetCurrentUrl()
        {
            var context = System.Mvc.Engine.RequestContext;
            return $"{context.ControllerName}/{context.ActionName}";
        }
        internal static void Start() => Request(Config.StartUrl);

        static public User User { get; set; }

        static public Provider Provider { get; private set; } = new Provider();
        protected override void OnStartup(StartupEventArgs e)
        {
            Config.Load(Environment.CurrentDirectory + "/app_data/");
            base.OnStartup(e);
        }
    }
}