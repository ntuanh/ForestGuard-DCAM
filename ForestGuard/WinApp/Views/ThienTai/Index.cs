using System;
namespace WinApp.Views.Thientai
{
    using Vst.Controls;
    using Models;
    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Danh sách Thiên tai";
            context.TableColumns = new object[] {
                new TableColumn { Name = "TenThienTai", Caption = "Tên thiên tai", Width = 200, },
                new TableColumn { Name = "ToaDo", Caption = "Tọa độ", Width = 200, },
                new TableColumn { Name = "MucDoThietHai", Caption = "Mức độ thiên tai", Width = 200, },
                new TableColumn { Name = "LoaiThienTai", Caption = "Loại thiên tai", Width = 200, },
                new TableColumn { Name = "PhanLoai", Caption = "Phân loại thiên tai chi tiết", Width = 200, },
                //new TableColumn { Name = "LoaiThienTai", Caption = "LoaiThienTai Header", Width = 100, },
                //new TableColumn { Name = "PhanLoai", Caption = "PhanLoai Header", Width = 100, },
            };
            // Nếu không cần tìm kiếm thì xóa đoạn code này
            context.Search = (e, s) => {
                var x = (ThienTai)e;
                return x.Ten.ToLower().Contains(s);
            };
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Thông tin Thiên tai";
            context.Editors = new object[] {
                new EditorInfo { Name = "TenThienTai", Caption = "Tên thiên tai", Layout = 12,   },
                new EditorInfo { Name = "ToaDo", Caption = "Tọa độ", Layout = 6,   },
                new EditorInfo { Name = "MucDoThietHai", Caption = "Mức độ thiệt hại", Layout = 6,   },
                //new EditorInfo { Name = "LoaiThienTaiId", Caption = " Caption of LoaiThienTaiId", Layout = 12,   },
                //new EditorInfo { Name = "PhanLoaiId", Caption = " Caption of PhanLoaiId", Layout = 12,   },
                new EditorInfo { Name = "LoaiThienTaiId", Caption = "Loại thiên tai", Layout = 12, Type = "select", ValueName = "Id", DisplayName = "Ten", Options = Provider.Select<LoaiThienTai>(),   },
                new EditorInfo { Name = "PhanLoaiId", Caption = "Phân loại chi tiết", Layout = 12, Type = "select" , ValueName= "Id" , DisplayName ="Ten" , Options = Provider.Select<PhanLoai>()  },
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
