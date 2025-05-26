using System;

namespace Models
{
    public partial class Rung
    {
        public string DonVi => Provider.GetTable<DonVi>().Find<DonVi>(DonViId)?.Ten ?? string.Empty;
        public string NguonGoc => Provider.GetTable<NguonGoc>().Find<NguonGoc>(NguonGocId)?.Ten ?? string.Empty;
        public string LoaiCay => Provider.GetTable<LoaiCay>().Find<LoaiCay>(LoaiCayId)?.Ten ?? string.Empty;

        public string DieuKien => Provider.GetTable<DieuKien>().Find<DieuKien>(DieuKienId)?.Ten ?? string.Empty;
        public string MucDich => Provider.GetTable<MucDich>().Find<MucDich>(MucDichId)?.Ten ?? string.Empty;
        public string Chu => Provider.GetTable<Chu>().Find<Chu>(ChuId)?.Ten ?? string.Empty;
        public string TruLuong => Provider.GetTable<TruLuong>().Find<TruLuong>(TruLuongId)?.Ten ?? string.Empty;
    }
}
