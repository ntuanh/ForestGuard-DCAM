// WinApp/Views/DataListViewLayout.xaml.cs
using System.Collections;
using System.Collections.Generic; // Cho List<object> trong OnSearching
using System.Linq;            // Cho Cast<object>() và Count()
using System.Windows.Controls;
using Vst.Controls;
using System; // Cho DBNull, Convert (nếu cần thiết, nhưng ở đây chưa dùng)

namespace WinApp.Views
{
    public partial class DataListViewLayout : UserControl, ILayout
    {
        private Vst.Controls.TableView _mainTableView;
        private ScrollViewer _horizontalScrollViewer; // Mặc dù tạo trong code, có thể bạn không cần tham chiếu này trực tiếp nữa

        public DataListViewLayout()
        {
            InitializeComponent();

            // MainTableView đã được định nghĩa trong XAML với x:Name="MainTableView"
            // Chúng ta chỉ cần lấy tham chiếu đến nó.
            // Tuy nhiên, nếu bạn vẫn muốn tạo bằng code, code cũ của bạn là:
            // _mainTableView = new Vst.Controls.TableView();
            // if (_mainTableView.Columns == null)
            // {
            //     _mainTableView.Columns = new TableColumnCollection();
            // }
            // _horizontalScrollViewer = new ScrollViewer
            // {
            //     Content = _mainTableView,
            //     HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            //     VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
            // };
            // Body.Child = _horizontalScrollViewer; // Body là x:Name của Border

            // Với XAML ở trên, _mainTableView sẽ được truy cập qua this.MainTableView (nếu x:Name được đăng ký)
            // Hoặc bạn có thể tìm nó nếu cần.
            // Để đơn giản, chúng ta sẽ giả định this.MainTableView đã có sẵn từ XAML.
            if (this.MainTableView != null && this.MainTableView.Columns == null)
            {
                this.MainTableView.Columns = new TableColumnCollection();
            }


            // Sự kiện và action có thể giữ nguyên hoặc điều chỉnh
            if (this.MainTableView != null)
            {
                this.MainTableView.OpenItem += e => {
                    App.RedirectToAction("edit", e);
                };
            }

            //if (Header != null && Header.CreateAction != null)
            {
                Header.CreateAction(new ActionContext("Thêm mới", () => App.RedirectToAction("add")));
            }
        }

        public void Render(ViewContext context)
        {
            if (this.MainTableView == null || context == null) return;

            Header.DataContext = context;

            this.MainTableView.Columns.Clear();
            if (context.TableColumns != null)
            {
                foreach (var colObj in context.TableColumns)
                {
                    if (colObj is Vst.Controls.TableColumn c)
                    {
                        this.MainTableView.Columns.Add(c);
                    }
                }
            }

            var items = context.Model as IEnumerable;
            this.MainTableView.ItemsSource = items;
            // Gán lại Tag để tìm kiếm hoạt động đúng
            this.MainTableView.Tag = items;


            Total.Text = items?.Cast<object>().Count().ToString() ?? "0";

            if (context.Search != null && Header.SearchBox != null)
            {
                Header.SearchBox.Cleared -= OnSearchCleared; // Gỡ bỏ trước khi thêm
                //Header.SearchBox.Searching -= HandleSearching; // Gỡ bỏ trước khi thêm

                Header.SearchBox.Cleared += OnSearchCleared;
                // Sử dụng lambda để truyền context và items hiện tại vào
                Header.SearchBox.Searching += (searchText) => HandleSearching(searchText, context, this.MainTableView.Tag as IEnumerable);
            }
        }

        private void OnSearchCleared()
        {
            if (this.MainTableView != null && this.MainTableView.Tag is IEnumerable originalItems)
            {
                this.MainTableView.ItemsSource = originalItems;
                Total.Text = originalItems.Cast<object>().Count().ToString();
            }
        }

        // Đổi tên để tránh xung đột với phương thức Search của ViewContext
        private void HandleSearching(string searchText, ViewContext context, IEnumerable originalItems)
        {
            if (this.MainTableView == null || context.Search == null || originalItems == null) return;

            var lst = new List<object>();
            var lowerSearchText = searchText.ToLower();

            foreach (var i in originalItems)
            {
                if (context.Search(i, lowerSearchText))
                {
                    lst.Add(i);
                }
            }
            this.MainTableView.ItemsSource = lst;
            Total.Text = lst.Count.ToString();
        }


        // Thuộc tính để View cụ thể có thể truy cập nếu cần (nhưng Render đã xử lý)
        public IEnumerable ItemsSource
        {
            get => this.MainTableView?.ItemsSource;
            set { if (this.MainTableView != null) this.MainTableView.ItemsSource = value; }
        }

        public TableColumnCollection ViewColumns
        {
            get => this.MainTableView?.Columns;
            set { if (this.MainTableView != null) this.MainTableView.Columns = value; }
        }

        public string TotalCountText
        {
            get { return this.Total?.Text; }
            set { if (this.Total != null) this.Total.Text = value; }
        }
    }
}