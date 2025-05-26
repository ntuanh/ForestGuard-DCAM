using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    using Models;
    using TBL = DataSchema.Table;
    using PROC = DataSchema.StoredProc;
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
        protected virtual TBL ViewEngine => Provider.GetTable<T>();

        protected TBL DataEngine
        {
            get
            {
                if (data == null)
                    data = CreateDataEngine();
                return data;
            }
        }
        TBL data;

        protected virtual TBL CreateDataEngine() => ViewEngine;

        protected virtual T CreateEntity() => (T)Activator.CreateInstance(typeof(T));
        protected virtual PROC GetStoredProcedure(string name) => Provider.GetStoredProcedure(name);
        public override object Index()
        {
            return View(ViewEngine.ToList<T>(null, null));
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
            if (DataEngine.Type == "V")
            {
                return Error(1, "Bộ xử lý dữ liệu là một VIEW.\nCần viết lại hàm CreateDataEngine() để lấy một bảng.");
            }
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
            var proc = GetStoredProcedure("update" + DataEngine.Name);
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
        protected void ExecPROC(PROC proc)
        {
            Provider.CreateCommand(cmd => {
                cmd.CommandText = proc.Name;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var doc = Document.FromObject(UpdateContext.Model);
                var res = 0;

                doc.Add("action", (int)UpdateContext.Action);
                foreach (var p in proc.Parameters.Values)
                {
                    cmd.Parameters.AddWithValue($"@{p.Name}", doc[p.Name]);
                }
                try
                {
                    res = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    UpdateContext.Message = e.Message;
                }
                if (res == 0 && UpdateContext.Message == null)
                {
                    UpdateContext.Message = $"Không có bản ghi nào được cập nhật";
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
