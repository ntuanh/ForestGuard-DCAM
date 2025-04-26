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
    /// Interaction logic for DataListViewLayout.xaml
    /// </summary>
    public partial class DataListViewLayout : UserControl, ILayout
    {
        public void Render(ViewContext context)
        {
            var grid = (Vst.Controls.TableView)Body.Child;

            Header.DataContext = context;
            foreach (Vst.Controls.TableColumn c in context.TableColumns)
            {
                grid.Columns.Add(c);
            }
            var items = (IEnumerable<object>)context.Model;
            grid.ItemsSource = items;
            Total.Text = grid.RowsCount.ToString();

            if (context.Search != null)
            {
                Header.SearchBox.Cleared += () => grid.ItemsSource = items;
                Header.SearchBox.Searching += (s) => {
                    var lst = new List<object>();
                    
                    s = s.ToLower();
                    foreach (var i in items)
                    {
                        if (context.Search(i, s))
                        {
                            lst.Add(i);
                        }
                    }
                    grid.ItemsSource = lst;
                };
            }
        }
        public DataListViewLayout()
        {
            InitializeComponent();

            var grid = new Vst.Controls.TableView();
            Body.Child = grid;

            grid.OpenItem += e => {
                App.RedirectToAction("edit", e);
            };

            Header.CreateAction(new ActionContext("Thêm mới", () => App.RedirectToAction("add")));
        }
    }
}
