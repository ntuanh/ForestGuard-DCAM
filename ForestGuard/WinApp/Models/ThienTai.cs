using System;

namespace Models
{
    public partial class ThienTai
    {
        public string TenThienTai => Provider.GetTable<LoaiThienTai>().Find<LoaiThienTai>(LoaiThienTaiId)?.Ten ?? string.Empty;

        public string LoaiThienTai => Provider.GetTable<LoaiThienTai>().Find<LoaiThienTai>(LoaiThienTaiId)?.Ten ?? string.Empty;
        public string PhanLoai => Provider.GetTable<PhanLoai>().Find<PhanLoai>(PhanLoaiId)?.Ten ?? string.Empty;

        public string DonVi => Provider.GetTable<DonVi>().Find<DonVi>(DonViId)?.Ten ?? string.Empty;

    }
}