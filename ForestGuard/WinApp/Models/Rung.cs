using System;

namespace Models
{
    public partial class Rung
    {
        public string Donvi => Provider.GetTable<DonVi>().Find<DonVi>(DonViId)?.Ten ?? string.Empty;
        public string NguonGoc => Provider.GetTable<NguonGoc>().Find<NguonGoc>(NguonGocId)?.Ten ?? string.Empty;
        public string LoaiCay => Provider.GetTable<LoaiCay>().Find<LoaiCay>(LoaiCayId)?.Ten ?? string.Empty;
    }
}
