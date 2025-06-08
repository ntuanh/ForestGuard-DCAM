using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinApp
{
    // ntuanh ....
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Mvc.Engine.Register(this, result => {

                if (result.Code != 0)
                {
                    MessageBox.Show(result.Message, "Error");
                    return;
                }

                var view = result.View as Views.BaseView;
                var context = view.ViewContext;
                var content = context.Result as UIElement;

                if (content == null) return;

                var layout = context.Layout as UIElement;
                if (layout is Window)
                {
                    var dlg = (Window)layout;
                    dlg.Content = content;
                    dlg.ShowDialog();

                    return;
                }

                if (layout == null)
                {
                    layout = new Border { Child = content };
                }
                Content = layout;
            });

            Provider.ConnectionError += (s) => {
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu:\n{s}");
            };
            Provider.CommandError += (s) => {
                MessageBox.Show($"Lỗi chạy câu lệnh SQL:\n{s}");
            };

            Provider.Current.Schema.Migrate();
            //App.Start();
            App.User = new Actors.Admin();
            App.Request("home");
        }
    }
}