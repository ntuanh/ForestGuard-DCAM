using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

namespace Vst.Controls
{
    public static class UIElementExtension
    {
        public static UIElement SetRowSpan(this UIElement e, int v)
        {
            e.SetValue(Grid.RowSpanProperty, v);
            return e;
        }
        public static UIElement SetColumnSpan(this UIElement e, int v)
        {
            e.SetValue(Grid.ColumnSpanProperty, v);
            return e;
        }
        public static UIElement SetGridLayout(this UIElement e, int r, int c)
        {
            e.SetValue(Grid.ColumnProperty, c);
            e.SetValue(Grid.RowProperty, r);
            return e;
        }
        public static UIElement Show(this UIElement e, bool b)
        {
            e.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
            return e;
        }
    }
    public abstract class MyElement : Border
    {
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.RegisterAttached(nameof(BorderRadius),
            typeof(double),
            typeof(MyElement),
            new PropertyMetadata(0.0));
        public double BorderRadius
        {
            get => (double)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached(nameof(Foreground),
            typeof(Brush),
            typeof(MyElement),
            new PropertyMetadata(default(Brush)));
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public static readonly DependencyProperty ActivatedProperty =
            DependencyProperty.RegisterAttached(nameof(Activated),
            typeof(bool),
            typeof(MyElement),
            new PropertyMetadata(default(bool)));
        public bool Activated
        {
            get => (bool)GetValue(ActivatedProperty);
            set => SetValue(ActivatedProperty, value);
        }
        new public bool IsVisible
        {
            get => base.IsVisible;
            set => Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public MyElement()
        {
            DataContextChanged += (s, e) => Binding(DataContext);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                Child.SetValue(Control.ForegroundProperty, this.Foreground);
            }
            var r = BorderRadius;
            if (r > 0)
            {
                CornerRadius = new CornerRadius(r);
            }
            return base.MeasureOverride(constraint);
        }

        protected virtual void BindingAction(ActionContext context)
        {
        }
        protected virtual void Binding(object context) 
        { 
            if (context is ActionContext)
            {
                BindingAction((ActionContext)context);
            }
        }
    }

    [ContentProperty(nameof(Children))]
    public class MyPanel<T> : MyElement
        where T : Panel, new()
    {
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.RegisterAttached(nameof(Children),
            typeof(UIElementCollection),
            typeof(MyPanel<T>),
            null);
        public UIElementCollection Children
        {
            get { return (UIElementCollection)GetValue(ChildrenProperty); }
            protected set { SetValue(ChildrenProperty, value); }
        }
        protected T panel;
        public MyPanel()
        {
            Child = panel = new T();
            Children = panel.Children;
        }
    }

    public class MyScrollViewer : MyPanel<StackPanel>
    {
        public MyScrollViewer()
        {
            var sc = new ScrollViewer { 
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };
            Child = sc;
            sc.Content = panel;
        }
    }
    public class GridLayout : MyPanel<Grid>
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(nameof(Columns),
            typeof(ColumnDefinitionCollection),
            typeof(GridLayout),
            null);
        public ColumnDefinitionCollection Columns
        {
            get { return (ColumnDefinitionCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached(nameof(Rows),
            typeof(RowDefinitionCollection),
            typeof(GridLayout),
            null);
        public RowDefinitionCollection Rows
        {
            get { return (RowDefinitionCollection)GetValue(RowsProperty); }
            private set { SetValue(RowsProperty, value); }
        }
        public GridLayout()
        {
            Rows = panel.RowDefinitions;
            Columns = panel.ColumnDefinitions;
        }
        public GridLayout Split(int rows, int cols)
        {
            for (int i = 0; i < rows; i++) Rows.Add(new RowDefinition());
            for (int i = 0; i < cols; i++) Columns.Add(new ColumnDefinition());

            int r = 0, c = 0;
            foreach (UIElement e in Children)
            {
                e.SetGridLayout(r, c);
                if (++c >= cols)
                {
                    ++r; c = 0;
                }
            }

            return this;
        }
        public UIElement Add(UIElement e, int r, int c)
        {
            Children.Add(e.SetGridLayout(r, c));
            return e;
        }
    }
    public class TextBase : MyElement
    {
        #region dependency
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.RegisterAttached(
                nameof(HorizontalTextAlignment),
                typeof(HorizontalAlignment),
                typeof(TextBase),
                new PropertyMetadata(default(HorizontalAlignment)));
        public HorizontalAlignment HorizontalTextAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.RegisterAttached(nameof(VerticalTextAlignment),
                typeof(VerticalAlignment), typeof(TextBase), null);
        public VerticalAlignment VerticalTextAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalTextAlignmentProperty);
            set => SetValue(VerticalTextAlignmentProperty, value);
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.RegisterAttached(nameof(FontSize),
            typeof(double),
            typeof(TextBase),
            new PropertyMetadata(default(double)));
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.RegisterAttached(nameof(FontWeight),
            typeof(FontWeight),
            typeof(TextBase),
            new PropertyMetadata(default(FontWeight)));
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(nameof(Text),
            typeof(string),
            typeof(TextBase),
            new PropertyMetadata(default(string)));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.RegisterAttached(nameof(Url),
            typeof(string),
            typeof(TextBase),
            new PropertyMetadata(default(string)));
        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        #endregion
        public EventHandler Click { get; set; }
        protected override Size MeasureOverride(Size constraint)
        {
            if (FontSize > 0) _caption.FontSize = FontSize;
            _caption.FontWeight = FontWeight;
            _caption.HorizontalAlignment = HorizontalTextAlignment;
            _caption.VerticalAlignment = VerticalTextAlignment;

            return base.MeasureOverride(constraint);
        }
        protected override void OnRender(DrawingContext dc)
        {
            _caption.Foreground = Foreground;
            base.OnRender(dc);
        }
        protected TextBlock _caption;

        public UIElement Add(UIElement child)
        {
            _caption.Inlines.Add(child);
            return child;
        }
        public UIElement Add(string text)
        {
            return Add(new TextBlock {
                Text = text,
            });
        }
        public UIElement Add(string text, Brush color)
        {
            return Add(new TextBlock { 
                Text = text,
                Foreground = color,
            });
        }
        public TextBase()
        {
            Child = _caption = new TextBlock {
                DataContext = this,
            };
            var first = new TextBlock { };
            first.SetBinding(TextBlock.TextProperty, nameof(Text));

            Add(first);
            
            PreviewMouseLeftButtonDown += (s, e) => mouse_down_element = this;
            PreviewMouseLeftButtonUp += (s, e) =>
            {
                if (mouse_down_element == this)
                {
                    RaiseClicked();
                }
                mouse_down_element = null;
            };
        }

        static UIElement mouse_down_element;
        protected virtual void RaiseClicked()
        {
            Click?.Invoke(this, EventArgs.Empty);
            if (!string.IsNullOrWhiteSpace(Url))
            {
                System.Mvc.Engine.Execute(Url);
            }
        }
        protected override void BindingAction(ActionContext i)
        {
            if (i.Text != null) Text = i.Text;
            if (i.Url != null) Url = i.Url;
            if (i.Invoke != null)
            {
                Click += (_s, _e) => i.Invoke();
            }
        }
    }
}

namespace Vst.Controls
{
    public class ButtonBase : TextBase
    {
        public ButtonBase()
        {
            VerticalTextAlignment = VerticalAlignment.Center;
            Padding = new Thickness(5);
            Cursor = Cursors.Hand;
        }
        public ButtonBase SetAction(ActionContext model)
        {
            Text = model.Text;
            Url = model.Url;

            if (model.Invoke != null)
                Click += (s, e) => model.Invoke();
            return this;
        }
    }
    public class Button : ButtonBase
    {
        public Button()
        {
            Text = "Button";
            HorizontalTextAlignment = HorizontalAlignment.Center;
            Focusable = true;

            Click += (s, e) => Focus();
        }
    }
    public class MenuButton : ButtonBase
    {
        public MenuButton()
        {
            Text = "Menu Item";
        }
    }
    public class SideMenuButton : MenuButton
    {
    }
}

namespace Vst.Controls
{
    public abstract class MenuBase : MyPanel<StackPanel>
    {
        protected abstract MyElement CreateItem();
        protected override void BindingAction(ActionContext context) 
        {
            if (context.Childs != null)
            {
                foreach (var a in context.Childs)
                {
                    var btn = CreateItem();
                    btn.DataContext = a;
                    Children.Add(btn);
                }
            }
        }
    }
    public class HorizontalMenu : MenuBase
    {
        protected override MyElement CreateItem() => new MenuButton();
        public HorizontalMenu()
        {
            panel.Orientation = Orientation.Horizontal;
        }
    }
    public class VerticalMenu : MenuBase
    {
        protected override MyElement CreateItem() => new SideMenu();
    }

    public class SideMenuCaption : TextBase
    {

    }
    public class SideMenu : MenuBase
    {
        SideMenuCaption _cap = new SideMenuCaption();
        public string Text
        {
            get => _cap.Text;
            set => _cap.Text = value;
        }

        protected override MyElement CreateItem() => new SideMenuButton();
        public SideMenu()
        {
            var p = new StackPanel();

            Children.Add(_cap);
            Children.Add(p);

            Children = p.Children;
        }
        protected override void BindingAction(ActionContext context)
        {
            Text = context.Text;
            base.BindingAction(context);
        }
    }
}