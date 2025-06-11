using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class User
    {
        public ActionContext TopMenu { get; set; }
        public ActionContext SideMenu { get; set; }
        public string UserName { get; set; }
        public object Profile { get; set; }
        public string Description { get; set; }
    }
}

namespace Actors
{
    public partial class Admin : User { }
    public partial class Developer : User { }
    public partial class Staff : User { }
}
