using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum EditActions
    {
        Delete = -1,
        Update,
        Insert
    }
    public class EditContext
    {
        public object Model { get; set; }
        public EditActions Action { get; set; }

        public EditContext() { }
        public EditContext(object model)
        {
            Model = model;
        }
        public EditContext(object model, EditActions action)
        {
            Model = model;
            Action = action;
        }
    }
}
