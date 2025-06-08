namespace Models
{
    public partial class Chu
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public int? LoaiChuId { get; set; }
    }
}
namespace Models
{
    public partial class DieuKien
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class DonVi
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public int? HanhChinhId { get; set; }
        public string TenHanhChinh { get; set; }
        public int? TrucThuocId { get; set; }
    }
}
namespace Models
{
    public partial class HanhChinh
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public int? TrucThuocId { get; set; }
    }
}
namespace Models
{
    public partial class HoSo
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string Ext { get; set; }
    }
}
namespace Models
{
    public partial class LoaiCay
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class LoaiChu
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class LoaiThienTai
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class MucDich
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public int? TrucThuocId { get; set; }
    }
}
namespace Models
{
    public partial class NguonGoc
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class PhanLoai
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class Quyen
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public string Ext { get; set; }
    }
}
namespace Models
{
    public partial class Rung
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public string ToaDo { get; set; }
        public int DienTich { get; set; }
        public int? DonViId { get; set; }
        public int? NguonGocId { get; set; }
        public int? DieuKienId { get; set; }
        public int? LoaiCayId { get; set; }
        public int? MucDichId { get; set; }
        public int? ChuId { get; set; }
        public int? TruLuongId { get; set; }
        public int? TrucThuocId { get; set; }
    }
}
namespace Models
{
    public partial class TaiKhoan
    {
        public string Ten { get; set; }
        public string MatKhau { get; set; }
        public int? QuyenId { get; set; }
        public int? HoSoId { get; set; }
    }
}
namespace Models
{
    public partial class TenHanhChinh
    {
        public string Ten { get; set; }
    }
}
namespace Models
{
    public partial class ThienTai
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public string ToaDo { get; set; }
        public string MucDoThietHai { get; set; }
        public int? LoaiThienTaiId { get; set; }
        public int? PhanLoaiId { get; set; }
        public int? DonViId { get; set; }
        //public int? TongSoThienTai { get; set; }
    }
}
namespace Models
{
    public partial class TruLuong
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
    }
}

//namespace Models
//{
//    public partial class TrucThuoc
//    {
//        public int? Id { get; set; }
//        public string Ten { get; set; }
//    }
//}



namespace Models
{
    public partial class ViewDonVi
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public int? HanhChinhId { get; set; }
        public string TenHanhChinh { get; set; }
        public int? TrucThuocId { get; set; }
        public string Cap { get; set; }
        public string TrucThuoc { get; set; }
    }
}
namespace Models
{
    public partial class ViewHoSo
    {
        public int? Id { get; set; }
        public string Ten { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string Ext { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public int? QuyenId { get; set; }
        public string Quyen { get; set; }
    }
}
namespace Models
{
    public partial class ViewRung
    {
        public string TenRung { get; set; }
        public string ToaDo { get; set; }
        public string DienTich { get; set; }
        public int? DonViId { get; set; }
        public int? LoaiCayId { get; set; }
        public int? NguonGocId { get; set; }
        public int? DieuKienId { get; set; }
        public int? MucDichId { get; set; }
        public int? ChuId { get; set; }
        public int? TruLuongId { get; set; }
        public int? TrucThuocId { get; set; }
        public string TenDonVi { get; set; }
        public string LoaiCay { get; set; }
        public string NguonGoc { get; set; }
        public string DieuKien { get; set; }
        public string MucDich { get; set; }
        public string Chu { get; set; }
        public string TruLuong { get; set; }
        public string TrucThuoc { get; set; }
    }
}
namespace Models
{
    public partial class ViewThienTai
    {
        public string TenThienTai { get; set; }
        public string ToaDo { get; set; }
        public string MucDoThietHai { get; set; }
        public int? LoaiThienTaiId { get; set; }
        public int? PhanLoaiId { get; set; }
        public string LoaiThienTai { get; set; }
        public string PhanLoai { get; set; }
    }
}
