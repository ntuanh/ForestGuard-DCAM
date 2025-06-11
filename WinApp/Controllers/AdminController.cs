using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class AdminController : BaseController
    {
        public object Test(string id) => View(id);
    }
}
