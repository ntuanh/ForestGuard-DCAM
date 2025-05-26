using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vst.Controls;

namespace WinApp.Views.TaiKhoan
{
    using TC = TableColumn;
    using TE = EditorInfo;

    class Index : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Danh sách người dùng";
            context.TableColumns = new TC[] {
                new TC { Name = "Ten", Caption = "Họ tên", Width = 250 },
                new TC { Name = "SDT", Caption = "Số điện thoại", Width = 120 },
                new TC { Name = "Email", Caption = "Email", Width = 120 },
                new TC { Name = "TenDangNhap", Caption = "Tên đăng nhập", Width = 120 },
                new TC { Name = "Quyen", Caption = "Quyền truy cập", Width = 150 },
                new TC { Name = "MatKhau", Caption = "Mật khẩu", Width = 150 },
            };
            context.Search = (o, s) =>
            {
                var e = (Models.ViewHoSo)o;
                return e.Ten.ToLower().Contains(s) || e.TenDangNhap.Contains(s);
            };
        }
    }
    class Edit : Add
    {
        protected override void OnReady()
        {
            var key = "TenDangNhap";
            Find(key, e => e.IsEnabled = false);
            ShowDeleteAction("Ten");
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Cập nhật người dùng";
            context.Editors = new object[] {
                new TE { Name = "Ten", Caption = "Họ tên" },
                new TE { Name = "SDT", Caption = "Số điện thoại", Layout = 6 },
                new TE { Name = "Email", Caption = "Email", Layout = 6 },
                new TE { Name = "TenDangNhap", Caption = "Tên đăng nhập", Layout = 6 },

                new TE { Name = "QuyenId", Caption = "Quyền truy cập", Layout = 6,
                    Type = "select", ValueName = "Id", DisplayName = "Ten", 
                    Options = Provider.Select<Models.Quyen>(),
                },
            };
        }

        protected override void OnReady()
        {
            FindEditor("SDT", sdt => {
                sdt.GetInput().LostFocus += (s, e) => {
                    var un = FindEditor("TenDangNhap", null);
                    if (un.Text == string.Empty)
                        un.Value = sdt.Value;
                };
            });
        }
    }
}
