using System;

namespace Models
{
    public class ThongKeThienTaiDonVi : ThienTai {
    }
    public partial class ThienTai
    {
        public int TongSoThienTai { get; set; } = 0;
        public int SoLuongBao { get; set; } = 0;

        public int SoLuongLuLut { get; set; }= 0;
        public int SoLuongHanHan { get; set; } = 0;
        public int SoLuongSatLo { get; set; } = 0;
        public string TenThienTai => Provider.GetTable<LoaiThienTai>().Find<LoaiThienTai>(LoaiThienTaiId)?.Ten ?? string.Empty;

        public string LoaiThienTai => Provider.GetTable<LoaiThienTai>().Find<LoaiThienTai>(LoaiThienTaiId)?.Ten ?? string.Empty;
        public string PhanLoai => Provider.GetTable<PhanLoai>().Find<PhanLoai>(PhanLoaiId)?.Ten ?? string.Empty;

        public string DonVi => Provider.GetTable<DonVi>().Find<DonVi>(DonViId)?.Ten ?? string.Empty;

    }
}