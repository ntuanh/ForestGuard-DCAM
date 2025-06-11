using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class HomeController : BaseController
    {
        public override object Index()
        {
            if (App.User != null)
            {
                var name = App.User.GetType().Name;
                var top = App.User.TopMenu;
                if (top == null)
                {
                    App.User.TopMenu = top = Config.Actions.GetTopMenu(name);
                }
                App.User.SideMenu = new ActionContext { Childs = top.Childs[0].Childs };

                return RedirectToAction(name);
            }
            return View();
        }
        public object Missing() => View(App.User);
        public object Logout()
        {
            App.User = null;
            return Redirect(Config.StartUrl);
        }    

        public object Admin() => View();
        public object Developer() => View();
    }
}
