using System;

namespace Models
{
    public partial class ThienTai
    {
        public string TenThienTai => Provider.GetTable<LoaiThienTai>().Find<LoaiThienTai>(LoaiThienTaiId)?.Ten ?? string.Empty;

    }
}
