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
using Vst.Controls;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for FormViewLayout.xaml
    /// </summary>
    public partial class FormViewLayout : UserControl, ILayout
    {
        public event Action<List<EditorInfo>> Accepted;
        public event Action<MyEditor> EditorCreated;
        public FormControl Find(string name)
        {
            var grid = (EditorPanel)Body.Child;
            foreach (FormControl fc in grid.Children)
                if (fc.EditorInfo.Name == name)
                    return fc;
            return null;
        }
        public void Render(ViewContext context)
        {
            Title.Text = context.Title;
            if (context.Model is string)
            {
                var br = new Label { 
                    Content = context.Model,
                };
                Body.Child = br;
            }
            else if (context.Editors != null)
            {
                EditorPanel grid = new EditorPanel();
                foreach (EditorInfo e in context.Editors)
                {
                    var c = grid.Add(e);
                    EditorCreated?.Invoke(c);
                }
                Body.Child = grid;
            }
        }
        public FormViewLayout()
        {
            InitializeComponent();
        }
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var dlg = Parent as Window;

            AcceptButton.Click += (s, e) => {
                var grid = Body.Child as EditorPanel;
                List<EditorInfo> error = null;
                grid.CheckError(err => error = err);

                if (dlg != null && error == null)
                {
                    dlg.DialogResult = true;
                }
                Accepted?.Invoke(error);
            };
            if (dlg != null)
            {
                CancelButton.Click += (s, e) => dlg.DialogResult = false;
            }
        }
    }
}
