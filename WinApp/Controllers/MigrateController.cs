using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Migration;
using System.Mvc;

namespace WinApp.Controllers
{
    class MigrateController : BaseController
    {
        static CodeGenerator generator;
        static CodeGenerator.Block engine;

        CodeGenerator Generator
        {
            get => generator;
            set
            {
                generator = value;
                engine = generator.DefaultBlock;
            }
        }
        public override object Index()
        {
            Generator = new CSharp();
            return View();
        }
        //public object Gen(DataSchema.Table table)
        //{
        //    return View(engine.Run(table));
        //}
        //public object Gen(string mode)
        //{
        //    var m = App.User.SideMenu;
        //    if (m == null || m.Childs == null)
        //    {
        //        CreateMenu();
        //    }
        //    Engine.Mode = mode;
        //    return View(Engine.Run(null));
        //}

        protected override ActionResult View(IView view, object model)
        {
            return base.View(new Views.Migrate.Gen(), engine);
        }

        public object S(string id)
        {
            engine = Generator.CreateGenerator(id);
            return View();
        }
        public object CSharp() 
        {
            Generator = new CSharp();
            return View();
        }
        public object SQL()
        {
            Generator = new SQL();
            return View();
        }

    }
}
