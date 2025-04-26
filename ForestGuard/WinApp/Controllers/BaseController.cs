using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    using Models;
    class UpdateContext : EditContext
    {
        public string Message { get; set; }
    }
    class BaseController : System.Mvc.Controller
    {
        public virtual object Index() => View();
    }

    class DataController<T> : BaseController
    {
        protected Type EntityType => typeof(T);
        protected virtual DataSchema.Table DataEngine => Provider.GetTable<T>();
        protected virtual T CreateEntity() => (T)Activator.CreateInstance(EntityType);
        protected virtual string GetProcName()
        {
            var name = EntityType.Name;
            if (name.ToLower().StartsWith("view"))
                name = name.Substring(4);
            return "update" + name;
        }
        public override object Index()
        {
            return View(DataEngine.ToList<T>(null, null));
        }
        public virtual object Delete(T entity)
        {
            return View(new EditContext(entity, EditActions.Delete));
        }
        public virtual object Edit(T entity)
        {
            return View(new EditContext(entity));
        }
        public virtual object Add()
        {
            return View(new EditContext(CreateEntity(), EditActions.Insert));
        }

        protected UpdateContext UpdateContext { get; set; }
        public object Update(EditContext context)
        {
            UpdateContext = new UpdateContext {
                Action = context.Action,
                Model = context.Model,
            };

            UpdateCore((T)context.Model);

            if (UpdateContext.Message != null)
                return UpdateError();
            return UpdateSuccess();
        }

        protected virtual void TryInsert(T e) {
            ExecSQL(DataEngine.CreateInsertSql(e));
        }
        protected virtual void TryUpdate(T e) {
            ExecSQL(DataEngine.CreateUpdateSql(e));
        }
        protected virtual void TryDelete(T e) {
            ExecSQL(DataEngine.CreateDeleteSql(e));
        }
        protected virtual object UpdateSuccess()
        {
            return RedirectToAction("Index");
        }
        protected virtual object UpdateError() => Error(1, UpdateContext.Message);
        protected virtual void UpdateCore(T e)
        {
            var procName = GetProcName();
            var proc = procName == null ? null : Provider.GetStoredProcedure(procName);
            if (proc != null)
            {
                ExecPROC(proc);
            }
            else
            {
                switch (UpdateContext.Action)
                {
                    case EditActions.Delete: TryDelete(e); break;
                    case EditActions.Update: TryUpdate(e); break;
                    case EditActions.Insert: TryInsert(e); break;
                }
            }
        }
        protected void ExecPROC(DataSchema.StoredProc proc)
        {
            Provider.CreateCommand(cmd => {
                cmd.CommandText = proc.Name;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var doc = Document.FromObject(UpdateContext.Model);
                var res = 0;
                
                doc.Add("action", (int)UpdateContext.Action);
                foreach (var p in proc.Parameters.Values)
                {
                    cmd.Parameters.AddWithValue($"@{p.Name}", doc.GetString(p.Name));
                }
                try
                {
                    res = cmd.ExecuteNonQuery();
                }
                catch
                {
                }
                if (res == 0)
                {
                    UpdateContext.Message = $"Không cập nhật được dữ liệu\n{cmd.CommandText}\n{doc}";
                }
            });
        }
        protected void ExecSQL(string sql)
        {
            Provider.CreateCommand(cmd => {
                cmd.CommandText = sql;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    UpdateContext.Message = sql;
                }
            });
        }
    }
}
