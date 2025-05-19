using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{

    //partial class RungController : DataController<Models.ViewRung>
    //{
    //    // Tạo lại DataEngine từ RungDb
    //    //protected override DataSchema.Table CreateDataEngine() => RungDb;

    //    // Tạo lại thực thể mới
    //    protected override ViewRung CreateEntity()
    //    {
    //        return new ViewRung
    //        {
    //            DonViId = 1,
    //            NguonGocId = 1,
    //            DieuKienId = 1,
    //            LoaiCayId = 1,
    //            MucDichId = 1,
    //            ChuId = 1,
    //            TruLuongId = 1,
    //            TrucThuocId = 1
    //        };
    //    }

    //    // Cập nhật các trường của ViewRung
    //    protected override void UpdateCore(ViewRung e)
    //    {
    //        // Đảm bảo tên rừng không để trống
    //        if (string.IsNullOrWhiteSpace(e.TenRung))
    //            UpdateContext.Message = "Tên rừng không được để trống.";

    //        base.UpdateCore(e);
    //    }

    //    #region Không dùng Procedure
    //    // Đảm bảo bảng dữ liệu đúng với kiểu Rung
    //    DataSchema.Table RungDb => Provider.GetTable<Rung>();

    //    // Không cần stored procedure
    //    //protected override DataSchema.StoredProc GetStoredProcedure(string name) => null;

    //    // Thêm mới rừng
    //    protected override void TryInsert(ViewRung e)
    //    {
    //        // Thêm mới rừng
    //        var sql = RungDb.CreateInsertSql(e);
    //        ExecSQL(sql);

    //        // Không gọi lại hàm của lớp cơ sở
    //        // base.TryInsert(e);
    //    }

    //    // Cập nhật rừng
    //    protected override void TryUpdate(ViewRung e)
    //    {
    //        ExecSQL(RungDb.CreateUpdateSql(e));
    //    }

    //    // Xóa rừng
    //    protected override void TryDelete(ViewRung e)
    //    {
    //        ExecSQL(RungDb.CreateDeleteSql(e));
    //    }
    //    #endregion
    //}
}
