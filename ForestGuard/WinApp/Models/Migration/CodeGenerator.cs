using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Migration
{
    using TBL = DataSchema.Table;
    using COL = DataSchema.Column;
    using PRO = DataSchema.StoredProc;
    using PAR = DataSchema.ProcParam;
    public class CodeGenerator
    {
        static protected Dictionary<string, string> sql_types = new Dictionary<string, string> {
            { "varbinary", "byte[]" },
            { "binary", "byte[]" },
            { "uniqueidentifier", "Guid" },
            { "rowversion", "byte[]" },
            { "bit", "bool" },
            { "tinyint", "byte" },
            { "smallint", "short" },
            { "int", "int" },
            { "bigint", "long" },
            { "smallmoney", "decimal" },
            { "money", "decimal" },
            { "numeric", "decimal" },
            { "decimal", "decimal" },
            { "real", "double" },
            { "float", "double" },
            { "smalldatetime", "DateTime" },
            { "datetime", "DateTime" },
            { "date", "DateTime" },
            { "sql_variant", "object" },
        };
        static protected string indent;
        static protected int tab;

        static public string GetCShapType(string sqlType)
        {
            string s;
            sql_types.TryGetValue(sqlType, out s);

            return s ?? "string";
        }

        public class Line
        {
            protected string content = string.Empty;
            public Line Add(string s)
            {
                content += $"{indent}{s}\r\n";
                return this;
            }
            public Line Tab(int n)
            {
                tab += n;
                if (tab < 0) tab = 0;
                indent = new string(' ', tab << 2);
                return this;
            }
            public override string ToString()
            {
                return content;
            }
        }
        public class Block : Line
        {
            protected virtual void RaiseCompleted()
            {
                if (Completed != null)
                {
                    Completed.Invoke();
                    Reset();
                }
            }
            public event Action Completed;

            public virtual string Title
            {
                get
                {
                    var name = GetType().Name;
                    var res = string.Empty;
                    foreach (char c in name)
                    {
                        if (res != string.Empty && char.IsUpper(c))
                            res += ' ';
                        res += c;
                    }
                    return res;
                }
            }
            protected virtual string get_open_text() => "{";
            protected virtual string get_close_text() => "}";
            public virtual void Reset()
            {
                tab = 0;
                ClearContent();
            }
            public virtual void ClearContent() { content = string.Empty; }
            public Block Open()
            {
                Add(get_open_text());
                return (Block)Tab(1);
            }
            public Block Close()
            {
                Tab(-1);
                return (Block)Add(get_close_text());
            }
            public override string ToString()
            {
                End();
                return base.ToString();
            }
            public Block Start(string text)
            {
                Add(text);
                return Open();
            }
            public Block End()
            {
                while (tab > 0) Close();
                return this;
            }
            public Block Generate(string firstLine, Action renderBody)
            {
                Add(firstLine);
                Open();
                renderBody();
                Close();

                RaiseCompleted();
                return this;
            }
            public virtual Block Generate(TBL source)
            {
                return Generate(GetFirstLine(source), () => source.ForEach(c => RenderColumn(c)));
            }
            protected virtual string GetFirstLine(TBL source) => source.Name;
            protected virtual void RenderColumn(COL col) { }
        }
        public string Mode { get; set; }
        protected virtual Block CreateBlock(string name) => DefaultBlock;
        public virtual Block DefaultBlock => new Block();
        public Block CreateGenerator(string name)
        {
            if (name == null)
                return DefaultBlock;
            return CreateBlock(name.ToLower());
        }
    }

    public class CSharp : CodeGenerator
    {
        public override Block DefaultBlock => new ModelBlock();
        protected override Block CreateBlock(string name)
        {
            switch (name[0])
            {
                case 'v': return new ViewBlock();
                case 'c': return new ControllerBlock();
            }
            return DefaultBlock;
        }
        public class ControllerBlock : Block
        {
            public override string Title => "Controller";
            public override string ToString()
            {
                return "using Models;\r\n" + base.ToString();
            }
            public override Block Generate(TBL source)
            {
                Start($"namespace {GetType().Assembly.GetName().Name}.Controllers");
                base.Generate(source);
                return End();
            }
            protected override string GetFirstLine(TBL source)
            {
                return $"public partial class {source.Name}Controller : DataController<{source.Name}>";
            }

            protected override void RenderColumn(COL col)
            {
            }
        }
        public class ModelBlock : Block
        {
            protected string GetColumnType(COL col)
            {
                string t;
                if (!sql_types.TryGetValue(col.Type, out t))
                    return "string";

                return t + '?';
            }
            protected override void RenderColumn(COL col) => Add($"public {GetColumnType(col)} {col.Name} " + "{ get; set; }");
            protected override string GetFirstLine(TBL source) => $"public partial class {source.Name}";

            public override string Title => "Model";
            public override Block Generate(TBL source)
            {
                Start("namespace Models");
                base.Generate(source);
                return End();
            }
        }
        public class ViewBlock : Block 
        {
            public override Block Generate(TBL source)
            {
                Add("using System;");
                Start($"namespace {this.GetType().Assembly.GetName().Name}.Views.{source.Name}");
                Add("using Vst.Controls;");
                Add("using Models;");
                Start("class Index : BaseView<DataListViewLayout>");
                Start("protected override void RenderCore(ViewContext context)");
                Add($"context.Title = \"List of {source.Name}\";");
                Add("context.TableColumns = new object[] {").Tab(1);
                source.ForEach(c => {
                    if (c != source.Identity && c.Parent == null)
                    {
                        Add($"new TableColumn {{ Name = \"{c.Name}\", Caption = \"{c.Name} Header\", Width = 100, }},");
                    }
                });
                Tab(-1).Add("};");
                Close().Close();

                Start("class Add : EditView");
                Start("protected override void RenderCore(ViewContext context)");
                Add($"context.Title = \"{source.Name} Information\";");
                Add("context.Editors = new object[] {").Tab(1);
                source.ForEach(c => {
                    if (c != source.Identity)
                    {
                        var type = "";
                        if (c.Parent != null)
                        {
                            type = $"\r\n\tType = \"select\", ValueName = \"{c.Parent.PrimaryKey.Name}\", DisplayName = \"FieldName\","
                                + $" Options = Provider.Select<{c.Parent.Name}>(),";
                        }
                        Add("new EditorInfo {"
                            + $" Name = \"{c.Name}\", Caption = \" Caption of {c.Name}\", Layout = 12,"
                            + $" {(c.IsNullable ? "Required = false," : "")}"
                            + $" {type}"
                            + " },");
                    }
                });
                Tab(-1).Add("};");
                Close().Close();

                Start("class Edit : Add");
                Start("protected override void OnReady()");
                Add("// Thay FieldName bằng tên trường muốn thể hiện trên câu hỏi xóa bản ghi");
                Add("ShowDeleteAction(\"FieldName\");");

                Add("// Thay EditorName bằng tên trường muốn cấm soạn thảo");
                Add("Find(\"EditorName\", c => c.IsEnabled = false);");
                Close().Close();

                RaiseCompleted();
                return this;
            }
        }
    }

    public class SQL : CodeGenerator
    {
        public override Block DefaultBlock => new StoredProcedure();
        protected override Block CreateBlock(string name)
        {
            return base.CreateBlock(name);
        }
        public class SqlBlock : Block
        {
            protected override string get_open_text() => "BEGIN";
            protected override string get_close_text() => "END";

            public SqlBlock Go()
            {
                Tab(-tab);
                return (SqlBlock)Add("GO");
            }
        }
        public class StoredProcedure : SqlBlock
        {
            abstract class IFBlock : SqlBlock
            {
                protected abstract void CreateBody(TBL source);
                protected virtual void End(TBL source)
                {
                    var n = source.PrimaryKey.Name;
                    Add($"WHERE {n} = @{n}");
                }
                public string Create(int index, TBL source)
                {
                    Generate($"IF @action = {index - 1}", () => {
                        CreateBody(source);
                        if (source.PrimaryKey != null)
                        {
                            End(source);
                        }
                        Add("return");
                    });
                    return content;
                }
            }
            class Insert : IFBlock
            {
                protected override void CreateBody(TBL source)
                {
                    List<string> vals = new List<string>();

                    Add($"INSERT INTO {source.Name} VALUES").Tab(1);
                    source.ForEach(c => {
                        if (c != source.Identity) vals.Add('@' + c.Name);
                    });
                    Add($"({string.Join(", ", vals)})").Tab(-1);
                }
                protected override void End(TBL source)
                {
                    if (source.Identity != null)
                        Add($"SET @{source.Identity.Name} = @@IDENTITY");
                }
            }
            class Update : IFBlock
            {
                protected override void CreateBody(TBL source)
                {
                    var vals = new List<string>();
                    Add($"UPDATE {source.Name} SET").Tab(1);
                    source.ForEach(c => {
                        if (c != source.PrimaryKey)
                            vals.Add($"{indent}{c.Name} = @{c.Name}");
                    });
                    content += string.Join(",\r\n", vals) + "\r\n";
                    Tab(-1);
                }
            }
            class Delete : IFBlock
            {
                protected override void CreateBody(TBL source)
                {
                    Add("DELETE FROM " + source.Name);
                }
            }

            IFBlock[] childs = new IFBlock[] {
                new Delete(),
                new Update(),
                new Insert(),
            };
            public override Block Generate(TBL source)
            {
                string procName = "update" + source.Name;
                Add("IF NOT (SELECT CREATED FROM INFORMATION_SCHEMA.ROUTINES "
                    + $"WHERE ROUTINE_NAME='{procName}') IS NULL")
                    .Tab(1).Add($"drop proc {procName}");

                Go().Add($"create proc {procName}")
                    .Add("( @action int");

                source.ForEach(col => {
                    var s = $", @{col.Name} {col.Type}";
                    if (!string.IsNullOrEmpty(col.DataSize))
                        s += $"({col.DataSize})";
                    s += col == source.Identity ? " output" : " = NULL";
                    Add(s);
                });

                Start(") AS");

                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].ClearContent();
                    content += childs[i].Create(i, source);
                }
                End();
                Go();

                RaiseCompleted();
                return this;
            }

        }
    }
}
