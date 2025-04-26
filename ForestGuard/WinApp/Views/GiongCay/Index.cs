using System;
namespace WinApp.Views.TaiKhoan1
{
    using Vst.Controls;
    using Models;
    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            context.Title = "List of TaiKhoan";
            context.TableColumns = new object[] {
                new TableColumn { Name = "Ten", Caption = "Ten Header", Width = 100, },
                new TableColumn { Name = "MatKhau", Caption = "MatKhau Header", Width = 100, },
            };
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            context.Title = "TaiKhoan Information";
            context.Editors = new object[] {
                new EditorInfo { Name = "Ten", Caption = " Caption of Ten", Layout = 12,   },
                new EditorInfo { Name = "MatKhau", Caption = " Caption of MatKhau", Layout = 12,   },
                new EditorInfo { Name = "QuyenId", Caption = " Caption of QuyenId", Layout = 12,
    Type = "select", ValueName = "Id", DisplayName = "FieldName", Options = Provider.Select<Quyen>(), },
                new EditorInfo { Name = "HoSoId", Caption = " Caption of HoSoId", Layout = 12,
    Type = "select", ValueName = "Id", DisplayName = "FieldName", Options = Provider.Select<HoSo>(), },
            };
        }
    }
    class Edit : Add
    {
        protected override void OnReady()
        {
            // Thay FieldName bằng tên trường muốn thể hiện trên câu hỏi xóa bản ghi
            ShowDeleteAction("FieldName");
            // Thay EditorName bằng tên trường muốn cấm soạn thảo
            Find("EditorName", c => c.IsEnabled = false);
        }
    }
}
