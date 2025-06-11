using Models;
namespace WinApp.Controllers
{
    partial class ThientaiController : DataController<ThienTai>
    {
        public object ThongKe() => View();
        public object ThongKeThienTai()
        {
            var lst = System.Provider.GetTable<ThongKeThienTaiDonVi>().ToList<ThienTai>(null, null);
            return View(lst);
        }
    }
}