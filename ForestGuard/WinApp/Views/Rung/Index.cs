using System;
namespace WinApp.Views.Rung
{
    using Vst.Controls;
    using Models;
    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "List of ViewRung";
            context.TableColumns = new object[] {
                new TableColumn { Name = "Ten", Caption = "TenRung Header", Width = 100, },
                new TableColumn { Name = "ToaDo", Caption = "ToaDo Header", Width = 100, },
                new TableColumn { Name = "DienTich", Caption = "DienTich Header", Width = 100, },
                new TableColumn { Name = "DonVi", Caption = "DonViId Header", Width = 100, },
                new TableColumn { Name = "LoaiCay", Caption = "LoaiCayId Header", Width = 100, },
                new TableColumn { Name = "NguonGoc", Caption = "NguonGocId Header", Width = 100, },
                new TableColumn { Name = "DieuKienId", Caption = "DieuKienId Header", Width = 100, },
                new TableColumn { Name = "MucDichId", Caption = "MucDichId Header", Width = 100, },
                new TableColumn { Name = "ChuId", Caption = "ChuId Header", Width = 100, },
                new TableColumn { Name = "TruLuongId", Caption = "TruLuongId Header", Width = 100, },
                new TableColumn { Name = "TrucThuocId", Caption = "TrucThuocId Header", Width = 100, },
                new TableColumn { Name = "TenDonVi", Caption = "TenDonVi Header", Width = 100, },
                new TableColumn { Name = "LoaiCay", Caption = "LoaiCay Header", Width = 100, },
                new TableColumn { Name = "NguonGoc", Caption = "NguonGoc Header", Width = 100, },
                new TableColumn { Name = "DieuKien", Caption = "DieuKien Header", Width = 100, },
                new TableColumn { Name = "MucDich", Caption = "MucDich Header", Width = 100, },
                new TableColumn { Name = "Chu", Caption = "Chu Header", Width = 100, },
                new TableColumn { Name = "TruLuong", Caption = "TruLuong Header", Width = 100, },
                new TableColumn { Name = "RungTrucThuoc", Caption = "RungTrucThuoc Header", Width = 100, },
            };
            // Nếu không cần tìm kiếm thì xóa đoạn code này
            context.Search = (e, s) => {
                var x = (Rung)e;
                return x.Ten.ToLower().Contains(s);
            };
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Rung Information";
            context.Editors = new object[] {
                new EditorInfo { Name = "Ten", Caption = " Caption of Ten", Layout = 12,   },
                new EditorInfo { Name = "ToaDo", Caption = " Caption of ToaDo", Layout = 6,   },
                new EditorInfo { Name = "DienTich", Caption = " Caption of DienTich", Layout = 6,   },
                new EditorInfo { Name = "DonViId", Caption = " Caption of DonViId", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<DonVi>(), },
                new EditorInfo { Name = "NguonGocId", Caption = " Caption of NguonGocId", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<NguonGoc>(), },
                new EditorInfo { Name = "DieuKienId", Caption = " Caption of DieuKienId", Layout = 12,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<DieuKien>(), },
                new EditorInfo { Name = "LoaiCayId", Caption = " Caption of LoaiCayId", Layout = 12,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<LoaiCay>(), },
                new EditorInfo { Name = "MucDichId", Caption = " Caption of MucDichId", Layout = 12,   Type = "select", ValueName = "Id", DisplayName = "FieldName", Options = Provider.Select<MucDich>(), },
                new EditorInfo { Name = "ChuId", Caption = " Caption of ChuId", Layout = 12,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<Chu>(), },
                new EditorInfo { Name = "TruLuongId", Caption = " Caption of TruLuongId", Layout = 12,   Type = "select", ValueName = "Id", DisplayName = "FieldName", Options = Provider.Select<TruLuong>(), },
                new EditorInfo { Name = "TrucThuocId", Caption = " Caption of TrucThuocId", Layout = 12, Required = false, Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<Rung>(), },
            };
        }
    }
    class Edit : Add
    {
        protected override void OnReady()
        {
            // Thay Ten bằng tên trường muốn thể hiện trên câu hỏi xóa bản ghi
            ShowDeleteAction("Ten");
            // Thay EditorName bằng tên trường muốn cấm soạn thảo
            Find("EditorName", c => c.IsEnabled = false);
        }
    }
}
