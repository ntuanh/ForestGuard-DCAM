using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace WinApp.Controllers
{
    partial class HanhChinhController
    {
        protected override ViewDonVi CreateEntity() => new ViewDonVi {
            TenHanhChinh = "Quận",
            Ten = "Hoàng Mai",
            HanhChinhId = DonVi.CapHanhChinhDangXuLy 
        };
        public object Add(ViewDonVi one) => View(new EditContext { Model = one, Action = EditActions.Insert });
        public override object Index()
        {
            return View(Select(null));
        }
        protected object Select(int? cap)
        {
            return DonVi.DanhSach(DonVi.CapHanhChinhDangXuLy = cap);
        }
        public object Huyen() => View(Select(2));
        public object Xa() => View(Select(3));

        protected override DataSchema.Table CreateDataEngine() => Provider.GetTable<DonVi>();

        protected override object UpdateSuccess()
        {
            switch (UpdateContext.Action)
            {
                case EditActions.Insert: DonVi.All.Clear(); break;
                case EditActions.Delete: DonVi.All.Remove((ViewDonVi)UpdateContext.Model); break;
            }    
            return base.UpdateSuccess();
        }
    }
}
