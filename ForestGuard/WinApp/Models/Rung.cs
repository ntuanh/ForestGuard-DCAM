using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // Cần cho [NotMapped] nếu bạn dùng Entity Framework

namespace Models
{
    public partial class Rung
    {
        // --- ĐẢM BẢO CÁC THUỘC TÍNH ID CƠ BẢN NÀY ĐÃ ĐƯỢC ĐỊNH NGHĨA ---
        // Nếu chúng chưa có trong một file partial khác, bạn cần thêm chúng vào đây, ví dụ:
        /*
        public int Id { get; set; }
        public string Ten { get; set; }
        public string ToaDo { get; set; }
        public double? DienTich { get; set; }
        public int? DonViId { get; set; }
        public int? LoaiCayId { get; set; }
        public int? NguonGocId { get; set; }
        public int? DieuKienId { get; set; }
        public int? MucDichId { get; set; }
        public int? ChuId { get; set; }
        public int? TruLuongId { get; set; }
        */
        // --------------------------------------------------------------------

        // == CÁC THUỘC TÍNH SUY LUẬN HIỆN TẠI CỦA BẠN ==
        public string TenDonVi => DonViId.HasValue ? Provider.GetTable<DonVi>().Find<DonVi>(DonViId.Value)?.TenDayDu : null;
        public string LoaiCay => LoaiCayId.HasValue ? Provider.GetTable<LoaiCay>().Find<LoaiCay>(LoaiCayId.Value)?.Ten : null;

        // Sửa NguonGoc để tham chiếu đúng bảng NguonGoc và thêm kiểm tra null
        public string NguonGoc => NguonGocId.HasValue ? Provider.GetTable<NguonGoc>().Find<NguonGoc>(NguonGocId.Value)?.Ten : null;

        public string DieuKien => DieuKienId.HasValue ? Provider.GetTable<DieuKien>().Find<DieuKien>(DieuKienId.Value)?.Ten : null;
        public string MucDich => MucDichId.HasValue ? Provider.GetTable<MucDich>().Find<MucDich>(MucDichId.Value)?.Ten : null;
        public string Chu => ChuId.HasValue ? Provider.GetTable<Chu>().Find<Chu>(ChuId.Value)?.Ten : null;
        public string TruLuong => TruLuongId.HasValue ? Provider.GetTable<TruLuong>().Find<TruLuong>(TruLuongId.Value)?.Ten : null;


        // == THUỘC TÍNH ĐỂ HIỂN THỊ TÊN TỈNH/THÀNH TRỰC THUỘC (TỪ BẢNG LIÊN KẾT) ==
        // Thuộc tính này KHÔNG được ánh xạ vào cột nào của bảng Rung.
        // Giá trị của nó sẽ được điền vào khi bạn load dữ liệu Rừng (thông qua Provider).
        [NotMapped] // Quan trọng nếu bạn dùng Entity Framework, nếu không thì có thể bỏ qua.
        public string TenTinhThanhTrucThuoc { get; set; }


        // == THUỘC TÍNH TẠM ĐỂ GIỮ ID TỈNH ĐƯỢC CHỌN TRÊN UI KHI THÊM/SỬA ==
        // Thuộc tính này cũng KHÔNG được ánh xạ vào cột nào của bảng Rung.
        // Nó sẽ được dùng để truyền ID tỉnh đã chọn từ UI đến logic lưu trữ
        // để cập nhật bảng RungTinhThanhLienKet.
        [NotMapped] // Quan trọng nếu bạn dùng Entity Framework, nếu không thì có thể bỏ qua.
        public int? SelectedTinhThanhIdForUI { get; set; }

    }
}