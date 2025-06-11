using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SWC = System.Windows.Controls;

namespace Vst.Controls
{
    public class MyDialog : Window
    {
        public MyDialog()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

}
namespace Vst.Controls
{
    public class EditorInfo
    {
        public string Name { get; set; }
        public int Layout { get; set; }
        public string Type { get; set; } = "text";
        public string Caption { get; set; }
        public string Placeholder { get; set; }
        public bool Required { get; set; } = true;

        object _options;
        public object Options 
        {
            get => _options;
            set
            {
                if (_options == null)
                {
                    _options = value;
                    if (Control != null)
                    {
                        Control.SetEditorInfo(this);
                    }
                }
            }
        }
        public string DisplayName { get; set; }
        public string ValueName { get; set; }
        public MyEditor Control { get; set; }
    }
    public class EditorPanel : MyPanel<SWC.WrapPanel>
    {
        public void ForEach(Action<FormControl, MyEditor> callback)
        {
            foreach (FormControl c in Children)
            {
                callback(c, c.EditorInfo.Control);
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            ForEach((c, e) => {
                var w = c.EditorInfo.Layout;
                if (w == 0) w = 12;

                c.Width = constraint.Width / 12 * w;
            });
            return base.MeasureOverride(constraint);
        }

        public MyEditor Add(EditorInfo context)
        {
            Children.Add(new FormControl { EditorInfo = context });
            return context.Control;
        }

        public bool CheckError(Action<List<EditorInfo>> callback)
        {
            var error = new List<EditorInfo>();
            ForEach((f, c) => {
                if (f.EditorInfo.Required && string.IsNullOrWhiteSpace(c.Text))
                {
                    error.Add(f.EditorInfo);
                }
            });
            if (error.Count > 0)
            {
                callback?.Invoke(error);
                return true;
            }
            return false;
        }
    }
    public class FormControl : MyPanel<SWC.StackPanel>
    {
        static Dictionary<string, Type> temp = new Dictionary<string, Type> {
            {"text", typeof(TextBox) },
            {"select", typeof(SelectBox) },
            {"check", typeof(CheckBox) },
            {"number", typeof(NumberBox) },
            {"password", typeof(PasswordBox) },
        };

        EditorInfo _info;
        public EditorInfo EditorInfo
        {
            get => _info;
            set
            {
                if (_info != value)
                {
                    _info = value;
                    _info.Type = _info.Type.ToLower();
                }

                Children.Clear();

                var caption = new EditorLabel();
                caption.Add(_info.Caption);
                if (_info.Required)
                    caption.Add(" *", Brushes.Red);

                var editor = _info.Control;
                if (editor == null)
                {
                    editor = (MyEditor)Activator.CreateInstance(temp[_info.Type]);
                    _info.Control = editor;
                }
                editor.SetEditorInfo(_info);

                caption.Click += (s, e) => {
                    editor.SelectAll();
                    editor.GetInput().Focus();
                };

                Children.Add(caption);
                Children.Add(editor);
            }
        }
    }
    public class EditorLabel : TextBase
    {
        public EditorLabel()
        {
            Padding = new Thickness(0, 0, 0, 3);
            Cursor = Cursors.Hand;
        }
    }
    public class EditorFrame : MyElement
    {
        public void SetContent(UIElement element)
        {
            element.SetValue(BorderThicknessProperty, default(Thickness));
            element.SetValue(BackgroundProperty, Brushes.Transparent);
            element.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            Action<bool> raise_activate = b => {
                Activated = b;
                element.SetValue(ForegroundProperty, Foreground);
            };

            element.GotFocus += (s, e) => raise_activate(true);
            element.LostFocus += (s, e) => {
                raise_activate(false);
            };

            PreviewMouseDown += (s, e) => element.Focus();

            raise_activate(false);
        }

        public EditorFrame()
        {
            BorderThickness = new Thickness(1);
        }
    }
    public class Placeholder : SWC.TextBlock
    {
        public Placeholder()
        {
            VerticalAlignment = VerticalAlignment.Center;
        }

        new public bool IsVisible
        {
            get => base.IsVisible;
            set => base.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
    }
    public abstract class MyEditor : GridLayout
    {
        #region dependency
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.RegisterAttached(nameof(Caption),
            typeof(string),
            typeof(MyEditor),
            new PropertyMetadata(default(string)));
        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(nameof(Placeholder),
            typeof(string),
            typeof(MyEditor),
            new PropertyMetadata(default(string)));
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set
            {
                SetValue(PlaceholderProperty, value);
                placeholder.Text = value;
            }
        }

        #endregion

        protected EditorFrame frame = new EditorFrame();
        protected Placeholder placeholder = new Placeholder();

        public bool IsEmpty
        {
            get
            {
                var v = GetEditValue();
                return (v == null || v.Equals(string.Empty));
            }
        }

        public object Value
        {
            get => GetEditValue(); set => SetEditValue(value);
        }
        public string Text
        {
            get => GetEditValue()?.ToString() ?? string.Empty;
            set => SetEditValue(value);
        }
        protected virtual object GetEditValue() => null;
        protected virtual void SetEditValue(object v) { }

        protected void TryBinding(Action<PropertyInfo> callback)
        {
            if (!string.IsNullOrEmpty(Name) && DataContext != null)
            {
                var p = DataContext.GetType().GetProperty(Name);
                if (p != null)
                {
                    callback(p);
                }
            }
        }
        public MyEditor()
        {
            frame.Child = placeholder;
            Children.Add(frame);

            DataContextChanged += (s, e) => {
                if (DataContext == null)
                {
                    Value = null;
                    return;
                }
                TryBinding(p => SetEditValue(p.GetValue(DataContext)));
            };
        }
        public abstract UIElement GetInput();
        public virtual void SelectAll() { }
        public virtual void SetEditorInfo(EditorInfo i)
        {
            Name = i.Name;
            Placeholder = i.Placeholder;

            SetOptions(i.Options);
        }
        public virtual void SetOptions(object values) { }

        #region EVENTS
        public event EventHandler ValueChanged;
        protected virtual void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
    public abstract class MyEditor<TInput> : MyEditor
        where TInput : FrameworkElement, new()
    {
        public override UIElement GetInput() => input;
        protected TInput input { get; private set; } = new TInput();
        public MyEditor()
        {
            frame.SetContent(input);
            Children.Add(input);
            input.LostFocus += (s, e) => {
                object v = GetEditValue();
                Func<bool> empty = () => v == null || v.Equals(string.Empty);

                if (DataContext != null && !string.IsNullOrEmpty(Name))
                {
                    TryBinding(p => {
                        var t = p.PropertyType;
                        if (empty())
                        {
                            v = t.IsValueType ? Activator.CreateInstance(t) : null;
                            SetEditValue(v);
                        }
                        else
                        {
                            try
                            {
                                v = Convert.ChangeType(v, p.PropertyType);
                            }
                            catch
                            {
                            }
                        }
                        try
                        {
                            p.SetValue(DataContext, v);
                        }
                        catch
                        {
                        }
                    });
                }
                placeholder.IsVisible = empty();
            };

            input.GotFocus += (s, e) => placeholder.IsVisible = false;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            input.SetValue(MarginProperty, frame.Padding);
            placeholder.IsVisible = IsEmpty;
            return base.MeasureOverride(constraint);
        }
    }
    public class TextBox : MyEditor<SWC.TextBox>
    {
        protected virtual bool IsKeyInvalid(Key key) => false;
        public TextBox()
        {
            input.PreviewKeyDown += (s, e) => {
                switch (e.Key)
                {
                    case Key.Enter:
                        return;

                    case Key.Tab:
                        return;

                    case Key.Delete:
                        return;

                    case Key.Back:
                        return;
                }
                e.Handled = IsKeyInvalid(e.Key);
            };
            input.TextChanged += (s, e) => RaiseValueChanged();
        }
        protected override object GetEditValue() => input.Text;
        protected override void SetEditValue(object v) => input.Text = $"{v}";
    }
    public class SearchBox : TextBox
    {
        public event Action Cleared;
        public event Action<string> Searching;
        protected override void RaiseValueChanged()
        {
            string s = input.Text;
            if (s == string.Empty)
            {
                Cleared?.Invoke();
            }
            else
            {
                Searching?.Invoke(s);
            }
            base.RaiseValueChanged();
        }
        public SearchBox()
        {
            Placeholder = "search ...";
        }
    }
    public class NumberBox : TextBox
    {
        protected override bool IsKeyInvalid(Key key)
        {
            switch (key)
            {
                case Key.OemPeriod: 
                    return input.Text.IndexOf('.') >= 0;

                case Key.OemMinus:
                    return input.Text != string.Empty;
            }    
            return !((key >= Key.D0 && key <= Key.D9) 
                || (key >= Key.NumPad0 && key <= Key.NumPad9));
        }
        public override void SelectAll() => input.SelectAll();
    }
    public class PasswordBox : MyEditor<SWC.PasswordBox>
    {
        public PasswordBox()
        {
        }    
        protected override object GetEditValue() => input.Password;
        protected override void SetEditValue(object v) => input.Password = $"{v}";
        public override void SelectAll() => input.SelectAll();
    }
    public class SelectBox : MyEditor<SWC.ComboBox>
    {
        public SelectBox()
        {
            input.IsEditable = true;
            input.Padding = new Thickness(0);
        }
        protected override object GetEditValue()
        {
            if (string.IsNullOrEmpty(ValueName))
                return input.Text;

            var v = input.SelectedValue;
            if (v != null && !v.Equals(string.Empty))
            {
                var p = v.GetType().GetProperty(ValueName);
                if (p != null)
                {
                    return p.GetValue(v);
                }
            }

            return v;
        }
        protected override void SetEditValue(object v)
        {
            if (string.IsNullOrEmpty(ValueName) || input.ItemsSource == null)
            {
                input.Text = v?.ToString();
                return;
            }

            PropertyInfo p = null;
            foreach (var e in input.ItemsSource)
            {
                if (p == null)
                {
                    p = e.GetType().GetProperty(ValueName);
                    if (p == null)
                        return;
                }

                if (p.GetValue(e).Equals(v))
                {
                    input.SelectedValue = e;
                    return;
                }
            }
        }

        public bool Editable
        {
            get => input.IsEditable;
            set => input.IsEditable = value;
        }
        public string DisplayName
        {
            get => input.DisplayMemberPath;
            set => input.DisplayMemberPath = value;
        }
        public string ValueName { get; set; }
        public override void SetEditorInfo(EditorInfo i)
        {
            base.SetEditorInfo(i);
            DisplayName = i.DisplayName;
            ValueName = i.ValueName;
        }
        public override void SetOptions(object value)
        {
            if (value is string)
            {
                input.ItemsSource = ((string)value).Split(';');
                return;
            }
            if (value is IDictionary)
            {

                return;
            }
            input.ItemsSource = (IEnumerable)value;
        }
    }
    public class CheckBox : MyEditor<SWC.CheckBox>
    {
        //protected override void LayoutElements()
        //{
        //    label.Margin = new Thickness(0);
        //    frame.HorizontalAlignment = HorizontalAlignment.Left;
        //    frame.VerticalAlignment = VerticalAlignment.Center;
        //    frame.Padding = default(Thickness);

        //    Children.Add(frame);
        //    Children.Add(label);

        //    Split(0, 2);
        //    Columns[0].Width = new GridLength(30);
        //}
        //public CheckBox()
        //{
        //    Cursor = Cursors.Hand;
        //    label.Click += (s, e) => {
        //        Input.IsChecked = !true.Equals(Input.IsChecked);
        //    };
        //}
        protected override void SetEditValue(object v) => input.IsChecked = (bool?)v;
        protected override object GetEditValue() => input.IsChecked;
    }
}
