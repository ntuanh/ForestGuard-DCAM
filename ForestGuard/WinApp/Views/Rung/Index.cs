using System;
namespace WinApp.Views.Rung
{
    using Vst.Controls;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Danh sách Rừng";
            context.TableColumns = new object[] {
                // Đề xuất điều chỉnh:
                new TableColumn { Name = "Ten", Caption = "Tên rừng", Width = 200 },           // Giảm nhẹ
                new TableColumn { Name = "ToaDo", Caption = "Tọa Độ", Width = 60 },
                new TableColumn { Name = "DienTich", Caption = "Diện tích", Width = 68 },
                new TableColumn { Name = "TenDonVi", Caption = "Đơn vị hành chính", Width = 130 },// Giảm
                new TableColumn { Name = "NguonGoc", Caption = "Nguồn gốc", Width = 150 },     // Giảm
                new TableColumn { Name = "DieuKien", Caption = "Điều kiện", Width = 140 },     // Giảm
                new TableColumn { Name = "LoaiCay", Caption = "Loại cây", Width = 150 },       // Giảm
                new TableColumn { Name = "MucDich", Caption = "Mục đích", Width = 150 },       // Giảm
                new TableColumn { Name = "Chu", Caption = "Chủ", Width = 70 },

                // ==== CỘT TRỰC THUỘC RỪNG ĐƯỢC THÊM VÀO ĐÂY ====
                new TableColumn {
                    Name = "TenRungCha", // Tên thuộc tính trong Models.Rung để lấy tên Rừng cha
                    Caption = "Trực thuộc Rừng",
                    Width = 200          // Điều chỉnh độ rộng này cho phù hợp
                },
                // ===========================================
            };
            // Điều chỉnh lại tổng Width nếu cần để vừa với màn hình, hoặc cho phép thanh cuộn ngang.
            // Tổng Width hiện tại (ví dụ): 200+60+68+130+150+140+150+150+70+200 = 1318

            context.Search = (o, s) =>
            {
                var x = (Models.Rung)o;
                // Mở rộng tìm kiếm để bao gồm cả tên rừng cha nếu muốn
                string searchTerm = s.ToLower();
                return x.Ten.ToLower().Contains(searchTerm) ||
                       (x.TenRungCha != null && x.TenRungCha.ToLower().Contains(searchTerm));
            };
        }
    }

    // Phần còn lại của file giữ nguyên
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Thông tin Rừng";

            // Lấy danh sách tất cả các Rừng có thể làm cha
            var allRungs = Provider.Select<Models.Rung>().ToList(); // Lấy tất cả các rừng

            // Tạo danh sách lựa chọn cho ComboBox Trực thuộc
            // Bắt đầu với một lựa chọn "Không trực thuộc"
            var trucThuocOptions = new List<object> // Hoặc List<Models.Rung> nếu Provider.Select trả về kiểu đó
        {
            // Thêm một đối tượng đại diện cho việc không chọn Rừng cha
            // Giá trị Id ở đây (ví dụ 0) sẽ được dùng để biết khi nào người dùng chọn "Không trực thuộc"
            // và Ten sẽ là thứ hiển thị trên UI.
            // Đảm bảo class Models.Rung của bạn có thể được khởi tạo như thế này,
            // hoặc bạn có thể tạo một class/anonymous type riêng cho options nếu cần.
            // Nếu Models.Rung có hàm khởi tạo không tham số:
            new Models.Rung { Id = 0, Ten = "-- Không trực thuộc --" }
            // Nếu không, bạn có thể cần một cách khác để tạo đối tượng này, ví dụ:
            // new { Id = 0, Ten = "-- Không trực thuộc --" }
            // nhưng khi đó bạn cần đảm bảo ValueName và DisplayName hoạt động với anonymous type.
            // Tốt nhất là Models.Rung có thể được dùng.
        };

            // Thêm tất cả các Rừng hiện có vào danh sách lựa chọn
            // Nếu Provider.Select<Models.Rung>() trả về IEnumerable<Models.Rung>
            trucThuocOptions.AddRange(allRungs.Cast<object>()); // Cast<object>() nếu trucThuocOptions là List<object>
                                                                // Hoặc nếu trucThuocOptions là List<Models.Rung> và allRungs là List<Models.Rung>
                                                                // trucThuocOptions.AddRange(allRungs);


            context.Editors = new object[] {
            new EditorInfo { Name = "Ten", Caption = "Tên rừng", Layout = 120 },
            new EditorInfo { Name = "ToaDo", Caption = "Tọa Độ", Layout = 4 },
            new EditorInfo { Name = "DienTich", Caption = "Diện tích", Layout = 4 },
            new EditorInfo { Name = "DonViId", Caption = "Đơn vị hành chính", Layout = 4,
                Type = "select", ValueName = "Id", DisplayName = "TenDayDu",
                Options = Provider.Select<Models.DonVi>() },

            // ==== MỤC TRỰC THUỘC ĐƯỢC THÊM VÀO ĐÂY ====
            new EditorInfo {
                Name = "TrucThuocId", // Tên thuộc tính trong Models.Rung để lưu Id của Rừng cha
                Caption = "Trực thuộc Rừng",
                Layout = 12, // Hoặc 6 nếu bạn muốn nó cùng hàng với trường khác
                Type = "select",
                ValueName = "Id",      // Tên thuộc tính ID của đối tượng Rừng trong 'Options'
                DisplayName = "Ten",   // Tên thuộc tính Tên của đối tượng Rừng trong 'Options'
                Options = trucThuocOptions // Danh sách các Rừng (bao gồm cả "Không trực thuộc")
            },
            // ===========================================

            new EditorInfo { Name = "NguonGocId", Caption = "Nguồn gốc", Layout = 6,
                Type = "select", ValueName = "Id", DisplayName = "Ten",
                Options = Provider.Select<Models.NguonGoc>() },
            new EditorInfo { Name = "DieuKienId", Caption = "Điều kiện", Layout = 6,
                Type = "select", ValueName = "Id", DisplayName = "Ten",
                Options = Provider.Select<Models.DieuKien>() },
            new EditorInfo { Name = "LoaiCayId", Caption = "Loại cây", Layout = 12,
                Type = "select", ValueName = "Id", DisplayName = "Ten",
                Options = Provider.Select<Models.LoaiCay>() },
            new EditorInfo { Name = "MucDichId", Caption = "Mục đích", Layout = 12,
                Type = "select", ValueName = "Id", DisplayName = "Ten",
                Options = Provider.Select<Models.MucDich>() },
            new EditorInfo { Name = "ChuId", Caption = "Chủ", Layout = 12,
                Type = "select", ValueName = "Id", DisplayName = "Ten",
                Options = Provider.Select<Models.Chu>() },
        };
        }
    }

    class Edit : Add
    {
        protected override void OnReady()
        {
            ShowDeleteAction("Ten");
            Find("Ten", c => c.IsEnabled = true);
        }
    }
}