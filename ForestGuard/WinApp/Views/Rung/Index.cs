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
            context.Title = "Danh sách Rừng";
            context.TableColumns = new object[] {
                new TableColumn { Name = "Ten", Caption = "Tên rừng", Width = 230, },
                new TableColumn { Name = "ToaDo", Caption = "Tọa độ", Width = 90, },
                new TableColumn { Name = "DienTich", Caption = "Diện tích", Width = 60, },
                new TableColumn { Name = "DonVi", Caption = "Đơn vị", Width = 100, },
                new TableColumn { Name = "LoaiCay", Caption = "Loại cây", Width = 160, },
                new TableColumn { Name = "NguonGoc", Caption = "Nguồn gốc", Width = 80, },
                new TableColumn { Name = "DieuKien", Caption = "Điều kiện", Width = 150, },
                new TableColumn { Name = "MucDich", Caption = "Mục đích", Width = 180, },
                new TableColumn { Name = "Chu", Caption = "Chủ rừng", Width = 100, },
                new TableColumn { Name = "TruLuong", Caption = "Trữ lượng", Width = 100, },
                //new TableColumn { Name = "TrucThuocId", Caption = "TrucThuocId Header", Width = 100, },
                //new TableColumn { Name = "TenDonVi", Caption = "TenDonVi Header", Width = 100, },
                //new TableColumn { Name = "LoaiCay", Caption = "LoaiCay Header", Width = 100, },
                //new TableColumn { Name = "NguonGoc", Caption = "NguonGoc Header", Width = 100, },
                //new TableColumn { Name = "DieuKien", Caption = "DieuKien Header", Width = 100, },
                //new TableColumn { Name = "MucDich", Caption = "MucDich Header", Width = 100, },
                //new TableColumn { Name = "Chu", Caption = "Chu Header", Width = 100, },
                //new TableColumn { Name = "TruLuong", Caption = "TruLuong Header", Width = 100, },
                //new TableColumn { Name = "RungTrucThuoc", Caption = "RungTrucThuoc Header", Width = 100, },
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
            context.Title = "Thông tin rừng";
            context.Editors = new object[] {
                new EditorInfo { Name = "Ten", Caption = "Tên", Layout = 12,   },
                new EditorInfo { Name = "ToaDo", Caption = "Tọa độ", Layout = 4,   },
                new EditorInfo { Name = "DienTich", Caption = "Diện tích", Layout = 4,   },
                new EditorInfo { Name = "DonViId", Caption = "Đơn vị", Layout = 4,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<DonVi>(), },
                new EditorInfo { Name = "NguonGocId", Caption = "Nguồn gốc", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<NguonGoc>(), },
                new EditorInfo { Name = "DieuKienId", Caption = "Điều kiện", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<DieuKien>(), },
                new EditorInfo { Name = "LoaiCayId", Caption = "Loại cây", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<LoaiCay>(), },
                new EditorInfo { Name = "MucDichId", Caption = "Mục đích", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<MucDich>(), },
                new EditorInfo { Name = "TruLuongId", Caption = "Trữ lượng", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<TruLuong>(), },
                new EditorInfo { Name = "ChuId", Caption = "Chủ", Layout = 6,   Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<Chu>(), },
                new EditorInfo { Name = "TrucThuocId", Caption = "Trực thuộc", Layout = 12, Required = false, Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<Rung>(), },
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
