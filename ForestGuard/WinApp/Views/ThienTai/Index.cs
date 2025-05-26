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
            context.Title = "List of ViewThienTai";
            context.TableColumns = new object[] {
                new TableColumn { Name = "TenThienTai", Caption = "Tên thiên tai", Width = 200, },
                new TableColumn { Name = "ToaDo", Caption = "Tọa độ", Width = 200, },
                new TableColumn { Name = "MucDoThietHai", Caption = "Mức độ thiên tai", Width = 200, },
                new TableColumn { Name = "LoaiThienTaiId", Caption = "Loại thiên tai", Width = 200, },
                new TableColumn { Name = "PhanLoaiId", Caption = "Phân loại thiên tai", Width = 200, },
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
            context.Title = "ViewThienTai Information";
            context.Editors = new object[] {
                new EditorInfo { Name = "TenThienTai", Caption = " Caption of TenThienTai", Layout = 12,   },
                new EditorInfo { Name = "ToaDo", Caption = " Caption of ToaDo", Layout = 12,   },
                new EditorInfo { Name = "MucDoThietHai", Caption = " Caption of MucDoThietHai", Layout = 12,   },
                new EditorInfo { Name = "LoaiThienTaiId", Caption = " Caption of LoaiThienTaiId", Layout = 12,   },
                new EditorInfo { Name = "PhanLoaiId", Caption = " Caption of PhanLoaiId", Layout = 12,   },
                new EditorInfo { Name = "LoaiThienTai", Caption = " Caption of LoaiThienTai", Layout = 12,   },
                new EditorInfo { Name = "PhanLoai", Caption = " Caption of PhanLoai", Layout = 12,   },
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
