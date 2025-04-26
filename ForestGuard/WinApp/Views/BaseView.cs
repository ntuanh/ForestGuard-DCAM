using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vst.Controls;
using SWC = System.Windows.Controls;

namespace WinApp.Views
{
    public interface ILayout
    {
        void Render(ViewContext context);
    }
    public class ViewContext
    {
        public ActionContext TopMenu { get; set; }
        public ActionContext SideMenu { get; set; }
        public object Model { get; set; }
        public string Title { get; set; }
        public IEnumerable TableColumns { get; set; }
        public IEnumerable Editors { get; set; } 
        public object Layout { get; set; }
        public object Result { get; set; }
        public Func<object, string, bool> Search { get; set; }
    }

    public class FileDialogView<TDialog> : System.Mvc.IView
        where TDialog : Microsoft.Win32.FileDialog, new()
    {
        public object Content => null;

        public void Render(object model)
        {
            var dlg = new TDialog();
            if (dlg.ShowDialog() == true)
            {
                App.Post(new Document { Name = dlg.FileName });
            }
        }
    }

    public abstract class BaseView : System.Mvc.IView
    {
        public BaseView()
        {
            ViewContext = CreateViewContext();
            ViewContext.Result = CreateView();
            ViewContext.Layout = CreateLayout();
        }
        public ViewContext ViewContext { get; private set; }
        public virtual object Content => ViewContext.Result;
        public void Render(object model)
        {
            var context = ViewContext;
            context.Model = model;
            RenderCore(context);

            var layout = context.Layout as FrameworkElement;
            var view = (FrameworkElement)context.Result;

            Action<string, Action<FrameworkElement>> find = (s, c) => {
                var e = layout.FindName(s) as FrameworkElement;
                if (e != null) c(e);
            };
            Action<string> set_menu = s => {
                var p = typeof(ViewContext).GetProperty(s);
                if (p != null)
                {
                    var act = p.GetValue(context);
                    if (act != null)
                    {
                        find(s, e => e.DataContext = act);
                    }
                }
            };

            if (layout != null)
            {
                if (!(layout is Window))
                {
                    if (App.User != null)
                    {
                        if (context.TopMenu == null)
                            context.TopMenu = App.User.TopMenu;

                        if (context.SideMenu == null)
                            context.SideMenu = App.User.SideMenu;
                    }
                    set_menu("TopMenu");
                    set_menu("SideMenu");
                }
                find("Body", e => ((SWC.Border)e).Child = view);
            }

            if (context.Result is ILayout)
            {
                ((ILayout)context.Result).Render(context);
            }
            view.DataContext = context.Model;

            OnReady();
        }
        protected virtual void OnReady() { }
        protected virtual ViewContext CreateViewContext() => new ViewContext();
        protected virtual object CreateLayout() => new MainUserLayout();
        protected abstract object CreateView();
        protected virtual void RenderCore(ViewContext context) { }
    }
    public abstract class BaseView<TView> : BaseView
    {
        protected override object CreateView()
        {
            return Activator.CreateInstance(typeof(TView));
        }
        public TView MainView => (TView)ViewContext.Result;
    }
    public abstract class BaseView<TView, TModel> : BaseView<TView>
    {
        public TModel Model => (TModel)ViewContext.Model;
    }
    public class FormView : BaseView<FormViewLayout>
    {
        public FormControl Find(string name, Action<FormControl> callback)
        {
            var e = MainView.Find(name);
            if (e != null && callback != null) callback(e);

            return e;
        }
        public MyEditor FindEditor(string name, Action<MyEditor> callback)
        {
            MyEditor e = null;

            Find(name, fc => {
                e = fc.EditorInfo.Control;
                callback?.Invoke(e);
            });
            return e;
        }
    }
    public class EditView : FormView
    {
        public Models.EditActions Action { get; set; }
        protected virtual void RaiseUpdate()
        {
            var context = new Models.EditContext {
                Action = Action,
                Model = ViewContext.Model,
            };
            App.RedirectToAction("update", context);
        }
        public virtual void ShowDeleteAction(string name, string text = "xóa")
        {
            MainView.DenyButton.Text = text;
            MainView.DenyButton.IsVisible = true;
            MainView.DenyButton.Click += (s, e) => {
                var model = ViewContext.Model;
                var v = model.GetType().GetProperty(name).GetValue(model);

                var res = MessageBox.Show($"Xóa bản ghi {v}", ViewContext.Title, MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    Action = Models.EditActions.Delete;
                    RaiseUpdate();
                }
            };
        }
        public EditView()
        {
            MainView.Accepted += e => { 
                if (e != null)
                {
                    var s = "";
                    var space = "          ";

                    Func<string, string> line = r => space + r + space;
                    Func<string, string> eline = r => line(r) + '\n';
                    if (e.Count == 1)
                    {
                        s = line(e[0].Caption + " không được bỏ trống");
                    }
                    else
                    {
                        s = eline("Một số trường bị bỏ trống:");
                        foreach (var r in e)
                        {
                            s += eline($"- {r.Caption}");
                        }
                    }
                    MessageBox.Show(s, "Lỗi cập nhật");
                    e[0].Control.GetInput().Focus();
                    return;
                }
                RaiseUpdate();
            };

            MainView.CancelButton.IsVisible = true;
            MainView.CancelButton.Click += (s, e) => {
                App.RedirectToAction("index");
            };
        }
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);

            var ec = (Models.EditContext)context.Model;
            context.Model = ec.Model;
            Action = ec.Action;
        }
    }
}
