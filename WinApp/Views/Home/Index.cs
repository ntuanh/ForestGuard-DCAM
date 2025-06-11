using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Vst.Controls;

namespace WinApp.Views.Home
{
    class Missing : Index
    {
        protected override void ShowComment(string un)
        {
            Add("Chức năng chưa được phát triển", 20, Brushes.Orange);
        }
        public Missing()
        {
        }
    }
    class Index : BaseView<MyScrollViewer>
    {
        protected TextBlock Add(string text, double size, Brush color)
        {
            var label = new TextBlock {
                Text = text,
                FontSize = size,
                Foreground = color,
                Margin = new System.Windows.Thickness(20, 10, 0, 10),
            };
            MainView.Children.Add(label);
            return label;
        }
        protected TextBlock Add(string text, double size) => Add(text, size, Brushes.Black);
        
        protected virtual void ShowComment(string un)
        {
            if (un != null)
            {
                Add("Hệ thống chức năng được thiết lập trong file /App_Data/Actions.json", 14);

                var act = Document.FromObject(Config.Actions.SelectPath("actors." + un));
                var code = Add(act.ToString(), 14, Brushes.Brown);

                code.FontFamily = new FontFamily("Consolas");
            }
        }
        public Index()
        {
            string un = null;
            if (App.User != null)
            {
                un = App.User.GetType().Name;
            }
            Add(un ?? "HOME", 20);
            ShowComment(un);
        }
    }
}
