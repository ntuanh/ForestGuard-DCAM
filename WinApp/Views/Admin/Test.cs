using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Views.Admin
{
    class Test : Home.Missing
    {
        protected override void ShowComment(string un)
        {
        }

        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            Add("TEST " + ViewContext.Model, 20);
        }
    }
}
