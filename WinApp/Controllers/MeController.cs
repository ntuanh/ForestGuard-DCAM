using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class MeController : DataController<User>
    {
        public override object Index() => View();

        public object ChangePass() => Add();
        public object ChangePass(string value)
        {
            // Chạy lệnh SQL
            ExecSQL($"UPDATE TaiKhoan SET MatKhau='{value}' WHERE Ten='{App.User.UserName}'");

            // Hoặc gọi stored procedure
            //UpdateContext = new UpdateContext {
            //    Model = new Models.TaiKhoan {
            //        Ten = App.User.UserName,
            //        MatKhau = value,
            //    },
            //};
            // CallProc(Provider.GetStoredProcedure(nameof(ChangePass)));

            return Redirect("home/logout");
        }
    }
}
