using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace Vst.Controls
{
    public class TableColumn
    {
        string _header;
        public string Caption 
        {
            get => _header ?? Name; 
            set => _header = value; 
        }
        public double Width { get; set; }
        public string Name { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Brush Background { get; set; }
        public Brush Foreground { get; set; } = Brushes.Black;
    }
    public class TableColumnCollection : List<TableColumn>
    {
        public TableColumn this[string name] => Find(x => x.Name == name);
    }
    public class TableCell : TextBase
    {
        public int ColumnIndex => (int)GetValue(System.Windows.Controls.Grid.ColumnProperty);
        public TableCell()
        {
            VerticalTextAlignment = VerticalAlignment.Center;
        }
    }
    public class TableRow : GridLayout
    {
        protected virtual TableCell CreateCell(TableColumn column)
        {
            return new TableCell {
                HorizontalTextAlignment = column.HorizontalAlignment,
                Background = column.Background,
                Foreground = column.Foreground,
            };
        }
        public virtual void SetColumns(TableColumn[] cols)
        {
            Columns.Clear();
            Children.Clear();

            foreach (var col in cols)
            {   
                Children.Add(CreateCell(col));
            }
            Split(0, cols.Length);
            for (int i = 0; i < cols.Length; i++)
            {
                var w = cols[i].Width;
                if (w != 0)
                {
                    Columns[i].Width = new GridLength(w);
                }
            }
        }
        public int RowIndex { get; set; }
    }
    public class TableHeaderCell : TableCell
    {
        public TableHeaderCell()
        {
        }
    }
    public class TableHeader : TableRow
    {
        protected override TableCell CreateCell(TableColumn column) {
            return new TableHeaderCell { 
                Text = column.Caption,
                HorizontalTextAlignment = column.HorizontalAlignment
            };
        }
    }
    public class TableView : Border
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(nameof(Columns),
            typeof(TableColumnCollection),
            typeof(TableView),
            null);
        public TableColumnCollection Columns
        {
            get { return (TableColumnCollection)GetValue(ColumnsProperty); }
            set {
                SetValue(ColumnsProperty, value);
            }
        }

        TableHeader header = new TableHeader();
        StackPanel body = new StackPanel { 
            Background = Brushes.White
        };
        ScrollBar vscroll;

        public int RowsCount
        {
            get
            {
                if (items == null)
                    return 0;
                return items.Length;
            }
        }

        int visibleRows;
        object[] items = new object[] { };
        protected virtual void RaiseOpenOne(int index)
        {
            if (index < items.Length)
            {
                OpenItem?.Invoke(items[index]);
            }
        }
        public event Action<object> OpenItem;
        public System.Collections.IEnumerable ItemsSource
        {
            get => items;
            set
            {
                items = new object[] { };
                if (value is Array)
                {
                    items = (object[])value;
                }
                else
                {
                    var lst = value as IList<object>;
                    if (lst == null)
                    {
                        var ie = value as IEnumerable<object>;
                        if (ie != null)
                        {
                            lst = new List<object>();
                            foreach (var e in ie)
                                lst.Add(e);
                        }
                    }    
                    if (lst != null)
                    {
                        items = new object[lst.Count];
                        int i = 0;
                        foreach (var e in value) items[i++] = e;
                    }
                }
                
                vscroll.Maximum = 0;
                vscroll.Maximum = items.Length - 1;

                InvalidateVisual();
            }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.RegisterAttached(nameof(RowHeight),
            typeof(double),
            typeof(TableView),
            new PropertyMetadata(30.0));
        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }
        public static readonly DependencyProperty RowIndexWidthProperty =
            DependencyProperty.RegisterAttached(nameof(RowIndexWidth),
            typeof(double),
            typeof(TableView),
            new PropertyMetadata(30.0));
        public double RowIndexWidth
        {
            get => (double)GetValue(RowIndexWidthProperty);
            set => SetValue(RowIndexWidthProperty, value);
        }

        public static readonly DependencyProperty RowIndexBackgroundProperty =
            DependencyProperty.RegisterAttached(nameof(RowIndexBackground),
            typeof(Brush),
            typeof(TableView),
            new PropertyMetadata(Brushes.WhiteSmoke));
        public Brush RowIndexBackground
        {
            get => (Brush)GetValue(RowIndexBackgroundProperty);
            set => SetValue(RowIndexBackgroundProperty, value);
        }

        public static readonly DependencyProperty RowOverBackgroundProperty =
            DependencyProperty.RegisterAttached(nameof(RowOverBackground),
            typeof(Brush),
            typeof(TableView),
            new PropertyMetadata(Brushes.Orange));
        public Brush RowOverBackground
        {
            get => (Brush)GetValue(RowOverBackgroundProperty);
            set => SetValue(RowOverBackgroundProperty, value);
        }

        public int FirstRow
        {
            get => (int)vscroll.Value;
            set => vscroll.Value = value < 0 ? 0 : (value >= vscroll.Maximum ? (int)vscroll.Maximum - 1 : value);
        }

        TableColumn[] GetRenderColumns()
        {
            var lst = new List<TableColumn> {
                new TableColumn { Width = RowIndexWidth, 
                    HorizontalAlignment = HorizontalAlignment.Right }
            };
            lst.AddRange(Columns);
            lst.Add(new TableColumn());

            return lst.ToArray();
        }
        void Measure(double height)
        {
            var h = RowHeight;
            visibleRows = (int)(height / h);
            body.Children.Clear();

            var cols = GetRenderColumns();
            for (int i = 0; i < visibleRows; i++)
            {
                var iv = new TableRow {
                    Height = h,
                };
                iv.SetColumns(cols);
                body.Children.Add(iv);
            }

            header.SetColumns(cols);

            if (items != null)
            {
                vscroll.Maximum = items.Length - visibleRows;
            }
            else
            {
                vscroll.Maximum = 0;
            }
        }
        void Render()
        {
            if (items == null) return;
            var k = FirstRow;
            var cols = GetRenderColumns();

            foreach (TableRow r in body.Children)
            {
                if (k >= items.Length)
                {
                    //r.DataContext = null;
                    //foreach (TableCell e in r.Children)
                    //    e.Text = null;

                    r.Visibility = Visibility.Collapsed;
                    continue;
                }

                var one = items[k];
                r.RowIndex = k++;
                r.DataContext = one;
                r.Visibility = Visibility.Visible;

                var doc = Document.FromObject(one);
                foreach (TableCell e in r.Children)
                {
                    if (e.ColumnIndex == 0)
                    {
                        e.Text = k.ToString();
                        e.Background = RowIndexBackground;
                        continue;
                    }

                    var name = cols[e.ColumnIndex].Name;
                    e.Text = name == null ? null : doc.GetString(name);
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Measure(body.ActualHeight);
            Render();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        public TableView()
        {
            Columns = new TableColumnCollection();

            var grid = new GridLayout { 
                Children = { header, body },
            };
            grid.Split(2, 1);
            grid.Rows[0].Height = new GridLength(RowHeight);

            vscroll = new ScrollBar { 
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            vscroll.ValueChanged += (s, e) => Render();
            grid.Add(vscroll, 1, 0);

            Child = grid;
            Focusable = true;

            PreviewMouseDown += (s, e) => {
                Focus();
            };
            
            TableRow mouse_over = null;
            Action<bool> on_row_over = b => { 
                if (mouse_over != null)
                {
                    mouse_over.Background = b ? RowOverBackground : Brushes.Transparent;
                }
                if (!b) mouse_over = null;
            };
            body.PreviewMouseMove += (s, e) => {
                int index = (int)(e.GetPosition(body).Y / RowHeight);
                if (index < 0 || index >= body.Children.Count)
                {
                    on_row_over(false);
                    return;
                }

                var r = (TableRow)body.Children[index];
                if (r != mouse_over)
                {
                    on_row_over(false);
                    mouse_over = index + FirstRow >= items.Length ? null : r;
                    on_row_over(true);
                }
            };
            body.PreviewMouseLeftButtonDown += (s, e) => {
                if (e.ClickCount >= 2 && mouse_over != null)
                {
                    RaiseOpenOne(mouse_over.RowIndex);
                }
            };
            body.MouseWheel += (s, e) => {
                var d = e.Delta;
                FirstRow -= (int)(d / RowHeight);
            };
            body.MouseLeave += (s, e) => on_row_over(false);
            
            Application.Current.MainWindow.PreviewKeyDown += (s, e) => {
                if (!IsFocused) return;

                switch (e.Key)
                {
                    case Key.Home: FirstRow = 0; return;
                    case Key.End: FirstRow = (int)vscroll.Maximum; return;
                    case Key.PageDown: FirstRow += visibleRows; return;
                    case Key.PageUp: FirstRow -= visibleRows; return;
                    case Key.Down: ++FirstRow; return;
                    case Key.Up: --FirstRow; return;
                }
            };
        }
    }
}
