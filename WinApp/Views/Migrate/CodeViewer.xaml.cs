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

namespace WinApp.Views.Migrate
{
    /// <summary>
    /// Interaction logic for CodeViewer.xaml
    /// </summary>
    public partial class CodeViewer : UserControl
    {
        public CodeViewer()
        {
            InitializeComponent();
        }
        public CodeViewer(string title, string body)
            : this()
        {
            Title.Text = title;
            Body.AppendText(body);

            CopyButton.Click += (s, e) => {
                Body.SelectAll();
                Body.Copy();
                MessageBox.Show(title + " copied to the clipboard", "migrate");
            };
        }
    }
}
