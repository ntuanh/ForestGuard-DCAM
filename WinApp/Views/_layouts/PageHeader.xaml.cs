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

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for PageHeader.xaml
    /// </summary>
    public partial class PageHeader : UserControl
    {
        public PageHeader()
        {
            InitializeComponent();
        }

        public Vst.Controls.ButtonBase CreateAction(ActionContext context)
        {
            var btn = new Vst.Controls.Button().SetAction(context);
            ActionPanel.Children.Add(btn);
            return btn;
        }
    }
}
