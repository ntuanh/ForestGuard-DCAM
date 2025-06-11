using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Vst.Controls;
using TBL = System.DataSchema.Table;
using COL = System.DataSchema.Column;
using CBD = System.Migration.CodeGenerator.Block;

namespace WinApp.Views.Migrate
{
    class Gen : BaseView<MigrateLayout, CBD>
    {
        ActionContext CreateMenu()
        {
            var sch = Provider.Current.Schema;
            var tbl = new ActionContext("TABLES");
            var vie = new ActionContext("VIEWS");
            var context = new ActionContext()
                .Add(tbl)
                .Add(vie);

            foreach (var e in sch.Tables.Values.OrderBy(x => x.Name))
            {
                var act = new ActionContext(e.Name, () => {
                    view.Body.Document.Blocks.Clear();
                    Model.Generate(e);
                });
                switch (e.Type)
                {
                    case "V": vie.Add(act); continue;
                }
                tbl.Add(act);
            }
            return context;
        }

        CodeViewer view;
        protected override void RenderCore(ViewContext context)
        {
            MainView.ItemsMenu.DataContext = CreateMenu();
            view = new CodeViewer(Model.Title, null);

            Model.Completed += () => {
                view.Body.AppendText(Model.ToString());
            };
            Task.Run(() => {
                foreach (TBL table in Provider.Current.Schema.Tables.Values.OrderBy(x => x.Name))
                {
                    MainView.Dispatcher.InvokeAsync(() => {
                        Model.Generate(table);
                    });
                    System.Threading.Thread.Sleep(100);
                }
            });
            MainView.Body.Child = view;
        }
    }
}
