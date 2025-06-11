using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vst.Controls;

namespace WinApp.Views.Me
{
    using TC = TableColumn;
    using TE = EditorInfo;

    class PassModel
    {
        public string P { get; set; }
        public string C { get; set; }
        public bool IsMatch => P == C;
    }
    class ChangePass : EditView
    {
        protected override void RaiseUpdate()
        {
        }
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Đổi mật khẩu";
            context.Editors = new object[] {
                new TE { Name = "P", Caption = "Mật khẩu", Type = "password" },
                new TE { Name = "C", Caption = "Xác nhận mật khẩu", Type = "password" },
            };

            var model = new PassModel();
            context.Model = model;

            MainView.AcceptButton.Click += (s, e) => {
                if (!model.IsMatch)
                {
                    MessageBox.Show("Mật khẩu không khớp");
                    return;
                }
                App.Request(null, model.P);
            };
        }
    }
}
