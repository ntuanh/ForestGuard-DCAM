using System;
namespace WinApp.Views.ViewRung
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
                new TableColumn { Name = "TenRung", Caption = "TenRung Header", Width = 100, },
                new TableColumn { Name = "ToaDo", Caption = "ToaDo Header", Width = 100, },
                new TableColumn { Name = "DienTich", Caption = "DienTich Header", Width = 100, },
                new TableColumn { Name = "DonViId", Caption = "DonViId Header", Width = 100, },
                new TableColumn { Name = "LoaiCayId", Caption = "LoaiCayId Header", Width = 100, },
                new TableColumn { Name = "NguonGocId", Caption = "NguonGocId Header", Width = 100, },
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
            context.Search = (e, s) =>
            {
                var x = (ViewRung)e;
                return x.TenRung.ToLower().Contains(s);
            };
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "ViewRung Information";
            context.Editors = new object[] {
                new EditorInfo { Name = "TenRung", Caption = " Caption of TenRung ", Layout = 12,   },
                new EditorInfo { Name = "ToaDo", Caption = " Caption of ToaDo", Layout = 12,   },
                new EditorInfo { Name = "DienTich", Caption = " Caption of DienTich", Layout = 12,   },
                new EditorInfo { Name = "DonViId", Caption = " Caption of DonViId", Layout = 12,   },
                new EditorInfo { Name = "LoaiCayId", Caption = " Caption of LoaiCayId", Layout = 12,   },
                new EditorInfo { Name = "NguonGocId", Caption = " Caption of NguonGocId", Layout = 12,   },
                new EditorInfo { Name = "DieuKienId", Caption = " Caption of DieuKienId", Layout = 12,   },
                new EditorInfo { Name = "MucDichId", Caption = " Caption of MucDichId", Layout = 12,   },
                new EditorInfo { Name = "ChuId", Caption = " Caption of ChuId", Layout = 12,   },
                new EditorInfo { Name = "TruLuongId", Caption = " Caption of TruLuongId", Layout = 12,   },
                new EditorInfo { Name = "TrucThuocId", Caption = " Caption of TrucThuocId", Layout = 12,   },
                new EditorInfo { Name = "TenDonVi", Caption = " Caption of TenDonVi", Layout = 12,   },
                new EditorInfo { Name = "LoaiCay", Caption = " Caption of LoaiCay", Layout = 12,   },
                new EditorInfo { Name = "NguonGoc", Caption = " Caption of NguonGoc", Layout = 12,   },
                new EditorInfo { Name = "DieuKien", Caption = " Caption of DieuKien", Layout = 12,   },
                new EditorInfo { Name = "MucDich", Caption = " Caption of MucDich", Layout = 12,   },
                new EditorInfo { Name = "Chu", Caption = " Caption of Chu", Layout = 12,   },
                new EditorInfo { Name = "TruLuong", Caption = " Caption of TruLuong", Layout = 12,   },
                new EditorInfo { Name = "RungTrucThuoc", Caption = " Caption of RungTrucThuoc", Layout = 12,   },
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
