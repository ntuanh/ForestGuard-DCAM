using System;
namespace WinApp.Views.GiongCay
{
    using Vst.Controls;
    using Models;
    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "List of GiongCay";
            context.TableColumns = new object[] {
                new TableColumn { Name = "Ten", Caption = "Tên giống", Width = 150, },
                new TableColumn { Name = "Nguon", Caption = "Xuất sứ", Width = 100, },
            };
            // Nếu không cần tìm kiếm thì xóa đoạn code này
            context.Search = (e, s) => {
                var x = (GiongCay)e;
                return x.Ten.ToLower().Contains(s);
            };
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "GiongCay Information";
            context.Editors = new object[] {
                new EditorInfo { Name = "Ten", Caption = " Caption of Ten", Layout = 12,   },
                new EditorInfo { Name = "Nguon", Caption = " Caption of Nguon", Layout = 12,   },
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
