using System.Collections.Generic;
using System.Data;
using System.Linq;
using Models;
namespace WinApp.Controllers
{
    partial class RungController : DataController<Rung>
    {
        public object ThongKe() => View();
        public object ThongKeDienTich()
        {
            var lst = System.Provider.GetTable<ThongKeDienTichRung>().ToList<Rung>(null, null);
            return View(lst);
        }
    }
}