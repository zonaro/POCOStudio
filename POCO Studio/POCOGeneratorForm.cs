using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using CommandLine;
using Db;
using Db.DbObject;
using Db.Helpers;
using Db.POCOIterator;
using InnerLibs;

using POCO_Studio;
using POCOGenerator.CommandLine;
using POCOGenerator.Extensions;
using POCOGenerator.Helpers;
using POCOGenerator.POCOWriter;

using WeifenLuo.WinFormsUI.Docking;

namespace POCOGenerator
{
    public partial class POCOGeneratorForm : DockContent
    {
        #region Form

        public POCOGeneratorForm()
        {
            InitializeComponent();
        }

        private void POCOGeneratorForm_Load(object sender, EventArgs e)
        {
        }

        private void POCOGeneratorForm_Shown(object sender, EventArgs e)
        {
            GetConnectionString();
            if (DbHelper.ConnectionString.IsNotBlank())
            {
                SetConnectionString(DbHelper.ConnectionString);
                BuildServerTree();
                GetPOCOIterator(GetSelectedObjects(), null);
            }
        }

        private void POCOGeneratorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion Form

        #region Connection String

        private string GetConnectionString()
        {
            using (var cs = new ConnectionDialog())
            {
                var ConnectionString = DbHelper.ConnectionString;
                if (File.Exists(Program.PropertyForm.settingsFileName))
                    ConnectionString = SerializationHelper.BinaryDeserializeFromFile<Options>(Program.PropertyForm.settingsFileName)?.ConnectionString ?? DbHelper.ConnectionString;

                cs.LoadConnectionString(ConnectionString);
                if (cs.ShowDialog() == DialogResult.OK)
                {
                    DbHelper.ConnectionString = cs.ConnectionstringBox.Text;
                }
            }

            return DbHelper.ConnectionString;
        }

        private void SetConnectionString(string connectionString)
        {
            DbHelper.ConnectionString = connectionString;

            var cs = new SqlServerConnectionsStringParser(connectionString);

            Server = new Server()
            {
                ServerName = cs.Server
            };

            Server.UserId = cs.UserID;

            if (cs.IntegratedSecurity)
            {
                Server.UserId = WindowsIdentity.GetCurrent().Name;
            }

            Server.Version = DbHelper.GetServerVersion();

            InitialCatalog = cs.InitialCatalog;
        }

        #endregion Connection String

        #region Server Tree

        private Server Server;
        private string InitialCatalog;

        private enum ImageType
        {
            Server,
            Database,
            Folder,
            Table,
            View,
            Procedure,
            Function,
            Column,
            PrimaryKey,
            ForeignKey,
            UniqueKey,
            Index
        }

        private void BuildServerTree()
        {
            try
            {
                DisableServerTree();

                TreeNode serverNode = BuildServerNode();
                trvServer.Nodes.Add(serverNode);
                Application.DoEvents();

                Database databaseCurrent = null;
                TreeNode databaseNodeCurrent = null;
                TreeNode tablesNode = null;
                TreeNode viewsNode = null;
                TreeNode proceduresNode = null;
                TreeNode functionsNode = null;
                TreeNode tvpsNode = null;

                Action<IDbObject> buildingDbObject = (IDbObject dbObject) =>
                {
                    if (dbObject is Database)
                    {
                        Database database = dbObject as Database;
                        TreeNode databaseNode = AddDatabaseNode(serverNode, database);

                        databaseCurrent = database;
                        databaseNodeCurrent = databaseNode;
                        tablesNode = null;
                        viewsNode = null;
                        proceduresNode = null;
                        functionsNode = null;
                        tvpsNode = null;
                    }

                    ShowBuildingStatus(dbObject);
                };

                Action<IDbObject> builtDbObject = (IDbObject dbObject) =>
                {
                    if (dbObject is Database)
                    {
                        Database database = dbObject as Database;
                        if (database.Errors.Count > 0)
                            databaseNodeCurrent.ForeColor = Color.Red;
                        toolStripStatusLabel.Text = string.Empty;
                        Application.DoEvents();
                    }
                    else if (dbObject is Table && (dbObject is Db.DbObject.View) == false)
                    {
                        Table table = dbObject as Table;
                        tablesNode = AddTablesNode(tablesNode, databaseCurrent, databaseNodeCurrent);
                        AddTableNode(tablesNode, table);
                    }
                    else if (dbObject is Db.DbObject.View)
                    {
                        Db.DbObject.View view = dbObject as Db.DbObject.View;
                        viewsNode = AddViewsNode(viewsNode, databaseCurrent, databaseNodeCurrent);
                        AddViewNode(viewsNode, view);
                    }
                    else if (dbObject is Procedure && (dbObject is Function) == false)
                    {
                        Procedure procedure = dbObject as Procedure;
                        proceduresNode = AddProceduresNode(proceduresNode, databaseCurrent, databaseNodeCurrent);
                        AddProcedureNode(proceduresNode, procedure);
                    }
                    else if (dbObject is Function)
                    {
                        Function function = dbObject as Function;
                        functionsNode = AddFunctionsNode(functionsNode, databaseCurrent, databaseNodeCurrent);
                        AddFunctionNode(functionsNode, function);
                    }
                    else if (dbObject is TVP)
                    {
                        TVP tvp = dbObject as TVP;
                        tvpsNode = AddTVPsNode(tvpsNode, databaseCurrent, databaseNodeCurrent);
                        AddTVPNode(tvpsNode, tvp);
                    }
                };

                DbHelper.BuildServerSchema(Server, InitialCatalog, buildingDbObject, builtDbObject);

                Program.PropertyForm.GetOptions(true, trvServer);

                trvServer.SelectedNode = serverNode;

                EnableServerTree();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = ex.ToFullExceptionString();
                toolStripStatusLabel.ForeColor = Color.Red;
            }
        }

        public ToolStripStatusLabel toolStripStatusLabel => Program.MainForm.toolStripStatusLabel;

        private void DisableServerTree()
        {
            trvServer.BeforeCollapse += trvServer_DisableEvent;
            trvServer.BeforeExpand += trvServer_DisableEvent;
            trvServer.AfterCheck += trvServer_AfterCheck;
            trvServer.MouseUp += trvServer_MouseUp;
        }

        private void EnableServerTree()
        {
            trvServer.BeforeCollapse -= trvServer_DisableEvent;
            trvServer.BeforeExpand -= trvServer_DisableEvent;
            trvServer.AfterCheck -= trvServer_AfterCheck;
            trvServer.MouseUp -= trvServer_MouseUp;
        }

        private void trvServer_DisableEvent(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void ShowBuildingStatus(IDbObject dbObject)
        {
            if (dbObject is Table)
            {
                Table table = dbObject as Table;
                toolStripStatusLabel.Text = string.Format("{0}.{1}", table.Database.ToString(), dbObject.ToString());
            }
            else if (dbObject is Procedure)
            {
                Procedure procedure = dbObject as Procedure;
                toolStripStatusLabel.Text = string.Format("{0}.{1}", procedure.Database.ToString(), dbObject.ToString());
            }
            else if (dbObject is TVP)
            {
                TVP tvp = dbObject as TVP;
                toolStripStatusLabel.Text = string.Format("{0}.{1}", tvp.Database.ToString(), dbObject.ToString());
            }
            else
            {
                toolStripStatusLabel.Text = string.Format("{0}", dbObject.ToString());
            }
            toolStripStatusLabel.ForeColor = Color.Black;
            Application.DoEvents();
        }

        private TreeNode AddDatabaseNode(TreeNode serverNode, Database database)
        {
            TreeNode databaseNode = BuildDatabaseNode(database);
            serverNode.Nodes.AddSorted(databaseNode);
            EnableServerTree();
            serverNode.Expand();
            Application.DoEvents();
            DisableServerTree();
            return databaseNode;
        }

        private TreeNode AddTablesNode(TreeNode tablesNode, Database databaseCurrent, TreeNode databaseNodeCurrent)
        {
            if (tablesNode == null)
            {
                tablesNode = BuildTablesNode(databaseCurrent);
                databaseNodeCurrent.Nodes.Insert(0, tablesNode);
                Application.DoEvents();
            }
            return tablesNode;
        }

        private void AddTableNode(TreeNode tablesNode, Table table)
        {
            TreeNode tableNode = BuildTableNode(table);
            tablesNode.Nodes.AddSorted(tableNode);
            Application.DoEvents();
        }

        private TreeNode AddViewsNode(TreeNode viewsNode, Database databaseCurrent, TreeNode databaseNodeCurrent)
        {
            if (viewsNode == null)
            {
                viewsNode = BuildViewsNode(databaseCurrent);
                databaseNodeCurrent.Nodes.Insert(1, viewsNode);
                Application.DoEvents();
            }
            return viewsNode;
        }

        private void AddViewNode(TreeNode viewsNode, Db.DbObject.View view)
        {
            TreeNode viewNode = BuildViewNode(view);
            viewsNode.Nodes.AddSorted(viewNode);
            Application.DoEvents();
        }

        private TreeNode AddProceduresNode(TreeNode proceduresNode, Database databaseCurrent, TreeNode databaseNodeCurrent)
        {
            if (proceduresNode == null)
            {
                proceduresNode = BuildProceduresNode(databaseCurrent);
                databaseNodeCurrent.Nodes.Insert(2, proceduresNode);
                Application.DoEvents();
            }
            return proceduresNode;
        }

        private void AddProcedureNode(TreeNode proceduresNode, Procedure procedure)
        {
            TreeNode procedureNode = BuildProcedureNode(procedure);
            proceduresNode.Nodes.AddSorted(procedureNode);
            Application.DoEvents();
        }

        private TreeNode AddFunctionsNode(TreeNode functionsNode, Database databaseCurrent, TreeNode databaseNodeCurrent)
        {
            if (functionsNode == null)
            {
                functionsNode = BuildFunctionsNode(databaseCurrent);
                databaseNodeCurrent.Nodes.Insert(3, functionsNode);
                Application.DoEvents();
            }
            return functionsNode;
        }

        private void AddFunctionNode(TreeNode functionsNode, Function function)
        {
            TreeNode functionNode = BuildFunctionNode(function);
            functionsNode.Nodes.AddSorted(functionNode);
            Application.DoEvents();
        }

        private TreeNode AddTVPsNode(TreeNode tvpsNode, Database databaseCurrent, TreeNode databaseNodeCurrent)
        {
            if (tvpsNode == null)
            {
                tvpsNode = BuildTVPsNode(databaseCurrent);
                databaseNodeCurrent.Nodes.Add(tvpsNode); // first one to be inserted
                Application.DoEvents();
            }
            return tvpsNode;
        }

        private void AddTVPNode(TreeNode tvpsNode, TVP tvp)
        {
            TreeNode tvpNode = BuildTVPNode(tvp);
            tvpsNode.Nodes.AddSorted(tvpNode);
            Application.DoEvents();
        }

        private TreeNode BuildServerNode()
        {
            string serverName = Server.ToString();
            if (string.IsNullOrEmpty(Server.Version) == false)
            {
                serverName += string.Format(" (SQL Server {0}", Server.Version.Substring(0, Server.Version.LastIndexOf('.')));
                if (string.IsNullOrEmpty(Server.UserId) == false)
                    serverName += " - " + Server.UserId;
                serverName += ")";
            }
            TreeNode serverNode = new TreeNode(serverName);
            serverNode.Tag = new NodeTag() { NodeType = POCODbType.Server, DbObject = Server };
            serverNode.ImageIndex = (int)ImageType.Server;
            serverNode.SelectedImageIndex = (int)ImageType.Server;
            return serverNode;
        }

        private TreeNode BuildDatabaseNode(Database database)
        {
            TreeNode databaseNode = new TreeNode(database.ToString());
            databaseNode.Tag = new NodeTag() { NodeType = POCODbType.Database, DbObject = database };
            databaseNode.ImageIndex = (int)ImageType.Database;
            databaseNode.SelectedImageIndex = (int)ImageType.Database;
            return databaseNode;
        }

        private TreeNode BuildTablesNode(Database database)
        {
            TreeNode tablesNode = new TreeNode("Tables");
            tablesNode.Tag = new NodeTag() { NodeType = POCODbType.Tables, DbObject = database };
            tablesNode.ImageIndex = (int)ImageType.Folder;
            tablesNode.SelectedImageIndex = (int)ImageType.Folder;
            return tablesNode;
        }

        private TreeNode BuildTableNode(Table table)
        {
            TreeNode tableNode = new TreeNode(table.ToString().Replace("dbo.", ""));
            tableNode.Tag = new NodeTag() { NodeType = POCODbType.Table, DbObject = table };
            tableNode.ImageIndex = (int)ImageType.Table;
            tableNode.SelectedImageIndex = (int)ImageType.Table;

            TreeNode columnsNode = new TreeNode("Columns");
            columnsNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
            columnsNode.ImageIndex = (int)ImageType.Folder;
            columnsNode.SelectedImageIndex = (int)ImageType.Folder;
            tableNode.Nodes.Add(columnsNode);

            if (table.TableColumns != null)
            {
                foreach (TableColumn column in table.TableColumns.OrderBy(c => c.ordinal_position ?? 0))
                    columnsNode.Nodes.Add(BuildTableColumn(column));

                if (table.TableColumns.Exists(c => c.IsPrimaryKey))
                    tableNode.Nodes.Add(BuildPrimaryKeysNode(table));

                if (table.TableColumns.Exists(c => c.HasUniqueKeys))
                    tableNode.Nodes.Add(BuildUniqueKeysNode(table));

                if (table.TableColumns.Exists(c => c.HasForeignKeys))
                    tableNode.Nodes.Add(BuildForeignKeysNode(table));

                if (table.TableColumns.Exists(c => c.HasIndexColumns))
                    tableNode.Nodes.Add(BuildIndexesNode(table));
            }
            else if (table.Error != null)
            {
                tableNode.ForeColor = Color.Red;
            }

            return tableNode;
        }

        private TreeNode BuildPrimaryKeysNode(Table table)
        {
            TreeNode primaryKeysNode = new TreeNode("Primary Keys");
            primaryKeysNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
            primaryKeysNode.ImageIndex = (int)ImageType.Folder;
            primaryKeysNode.SelectedImageIndex = (int)ImageType.Folder;

            var primaryKeys = table.TableColumns.Where(c => c.IsPrimaryKey).Select(c => c.PrimaryKey.Name).Distinct().OrderBy(n => n);
            foreach (string primaryKey in primaryKeys)
            {
                TreeNode columnNode = new TreeNode(primaryKey);
                columnNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
                columnNode.ImageIndex = (int)ImageType.PrimaryKey;
                columnNode.SelectedImageIndex = (int)ImageType.PrimaryKey;
                primaryKeysNode.Nodes.Add(columnNode);

                foreach (TableColumn column in table.TableColumns.Where(c => c.IsPrimaryKey && c.PrimaryKey.Name == primaryKey).OrderBy(c => c.PrimaryKey.Ordinal))
                    columnNode.Nodes.Add(BuildTableColumn(column));
            }

            return primaryKeysNode;
        }

        private TreeNode BuildUniqueKeysNode(Table table)
        {
            TreeNode uniqueKeysNode = new TreeNode("Unique Keys");
            uniqueKeysNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
            uniqueKeysNode.ImageIndex = (int)ImageType.Folder;
            uniqueKeysNode.SelectedImageIndex = (int)ImageType.Folder;

            var uniqueKeys = table.TableColumns.Where(c => c.HasUniqueKeys).SelectMany(c => c.UniqueKeys).Select(uk => uk.Name).Distinct().OrderBy(n => n);
            foreach (string uniqueKey in uniqueKeys)
            {
                TreeNode columnNode = new TreeNode(uniqueKey);
                columnNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
                columnNode.ImageIndex = (int)ImageType.UniqueKey;
                columnNode.SelectedImageIndex = (int)ImageType.UniqueKey;
                uniqueKeysNode.Nodes.Add(columnNode);

                foreach (TableColumn column in table.TableColumns.Where(c => c.HasUniqueKeys && c.UniqueKeys.Exists(uk => uk.Name == uniqueKey)).OrderBy(c => c.UniqueKeys.First(uk => uk.Name == uniqueKey).Ordinal))
                    columnNode.Nodes.Add(BuildTableColumn(column));
            }

            return uniqueKeysNode;
        }

        private TreeNode BuildForeignKeysNode(Table table)
        {
            TreeNode foreignKeysNode = new TreeNode("Foreign Keys");
            foreignKeysNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
            foreignKeysNode.ImageIndex = (int)ImageType.Folder;
            foreignKeysNode.SelectedImageIndex = (int)ImageType.Folder;

            var foreignKeys = table.TableColumns.Where(c => c.HasForeignKeys).SelectMany(c => c.ForeignKeys).Select(fk => fk.Name).Distinct().OrderBy(n => n);
            foreach (string foreignKey in foreignKeys)
            {
                TreeNode columnNode = new TreeNode(foreignKey);
                columnNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
                columnNode.ImageIndex = (int)ImageType.ForeignKey;
                columnNode.SelectedImageIndex = (int)ImageType.ForeignKey;
                foreignKeysNode.Nodes.Add(columnNode);

                foreach (TableColumn column in table.TableColumns.Where(c => c.HasForeignKeys && c.ForeignKeys.Exists(fk => fk.Name == foreignKey)).OrderBy(c => c.ForeignKeys.First(fk => fk.Name == foreignKey).Ordinal))
                {
                    ForeignKey fk = column.ForeignKeys.First(fk1 => fk1.Name == foreignKey);
                    string primary = string.Format(" -> {0}.{1}.{2}", fk.Primary_Schema, fk.Primary_Table, fk.Primary_Column);
                    columnNode.Nodes.Add(BuildTableColumn(column, primary));
                }
            }

            return foreignKeysNode;
        }

        private TreeNode BuildIndexesNode(Table table)
        {
            TreeNode indexesNode = new TreeNode("Indexes");
            indexesNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
            indexesNode.ImageIndex = (int)ImageType.Folder;
            indexesNode.SelectedImageIndex = (int)ImageType.Folder;

            var indexColumns = table.TableColumns.Where(c => c.HasIndexColumns).SelectMany(c => c.IndexColumns).Select(ic => ic.ToStringFull()).Distinct().OrderBy(n => n);
            foreach (string indexColumn in indexColumns)
            {
                TreeNode columnNode = new TreeNode(indexColumn);
                columnNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = table };
                columnNode.ImageIndex = (int)ImageType.Index;
                columnNode.SelectedImageIndex = (int)ImageType.Index;
                indexesNode.Nodes.Add(columnNode);

                foreach (TableColumn column in table.TableColumns.Where(c => c.HasIndexColumns && c.IndexColumns.Exists(ic => ic.ToStringFull() == indexColumn)).OrderBy(c => c.IndexColumns.First(ic => ic.ToStringFull() == indexColumn).Ordinal))
                    columnNode.Nodes.Add(BuildTableColumn(column));
            }

            return indexesNode;
        }

        private TreeNode BuildTableColumn(TableColumn column, string postfix = null)
        {
            TreeNode tableColumnNode = new TreeNode(column.ToStringFull() + postfix);
            tableColumnNode.Tag = new NodeTag() { NodeType = POCODbType.Column, DbObject = column };
            tableColumnNode.ImageIndex = (int)(column.IsPrimaryKey ? ImageType.PrimaryKey : (column.HasForeignKeys ? ImageType.ForeignKey : ImageType.Column));
            tableColumnNode.SelectedImageIndex = (int)(column.IsPrimaryKey ? ImageType.PrimaryKey : (column.HasForeignKeys ? ImageType.ForeignKey : ImageType.Column));
            return tableColumnNode;
        }

        private TreeNode BuildViewsNode(Database database)
        {
            TreeNode viewsNode = new TreeNode("Views");
            viewsNode.Tag = new NodeTag() { NodeType = POCODbType.Views, DbObject = database };
            viewsNode.ImageIndex = (int)ImageType.Folder;
            viewsNode.SelectedImageIndex = (int)ImageType.Folder;
            return viewsNode;
        }

        private TreeNode BuildViewNode(Db.DbObject.View view)
        {
            TreeNode viewNode = new TreeNode(view.ToString().Replace("dbo.", ""));
            viewNode.Tag = new NodeTag() { NodeType = POCODbType.View, DbObject = view };
            viewNode.ImageIndex = (int)ImageType.View;
            viewNode.SelectedImageIndex = (int)ImageType.View;

            TreeNode columnsNode = new TreeNode("Columns");
            columnsNode.Tag = new NodeTag() { NodeType = POCODbType.Columns, DbObject = view };
            columnsNode.ImageIndex = (int)ImageType.Folder;
            columnsNode.SelectedImageIndex = (int)ImageType.Folder;
            viewNode.Nodes.Add(columnsNode);

            if (view.TableColumns != null)
            {
                foreach (TableColumn column in view.TableColumns.OrderBy(c => c.ordinal_position ?? 0))
                {
                    TreeNode columnNode = new TreeNode(column.ToString());
                    columnNode.Tag = new NodeTag() { NodeType = POCODbType.Column, DbObject = column };
                    columnNode.ImageIndex = (int)ImageType.Column;
                    columnNode.SelectedImageIndex = (int)ImageType.Column;
                    columnsNode.Nodes.Add(columnNode);
                }
            }
            else if (view.Error != null)
            {
                viewNode.ForeColor = Color.Red;
            }

            return viewNode;
        }

        private TreeNode BuildProceduresNode(Database database)
        {
            TreeNode proceduresNode = new TreeNode("Stored Procedures");
            proceduresNode.Tag = new NodeTag() { NodeType = POCODbType.Procedures, DbObject = database };
            proceduresNode.ImageIndex = (int)ImageType.Folder;
            proceduresNode.SelectedImageIndex = (int)ImageType.Folder;
            return proceduresNode;
        }

        private TreeNode BuildProcedureNode(Procedure procedure)
        {
            TreeNode procedureNode = new TreeNode(procedure.ToString().Replace("dbo.", ""));
            procedureNode.Tag = new NodeTag() { NodeType = POCODbType.Procedure, DbObject = procedure };
            procedureNode.ImageIndex = (int)ImageType.Procedure;
            procedureNode.SelectedImageIndex = (int)ImageType.Procedure;

            TreeNode parametersNode = new TreeNode("Parameters");
            parametersNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureParameters, DbObject = procedure };
            parametersNode.ImageIndex = (int)ImageType.Folder;
            parametersNode.SelectedImageIndex = (int)ImageType.Folder;
            procedureNode.Nodes.Add(parametersNode);

            if (procedure.ProcedureParameters != null && procedure.ProcedureParameters.Count > 0)
            {
                foreach (ProcedureParameter parameter in procedure.ProcedureParameters.OrderBy<ProcedureParameter, int>(c => c.ordinal_position ?? 0))
                {
                    TreeNode parameterNode = new TreeNode(parameter.ToString());
                    parameterNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureParameter, DbObject = parameter };
                    parameterNode.ImageIndex = (int)ImageType.Column;
                    parameterNode.SelectedImageIndex = (int)ImageType.Column;
                    parametersNode.Nodes.Add(parameterNode);
                }
            }

            TreeNode columnsNode = new TreeNode("Columns");
            columnsNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureColumns, DbObject = procedure };
            columnsNode.ImageIndex = (int)ImageType.Folder;
            columnsNode.SelectedImageIndex = (int)ImageType.Folder;

            if (procedure.ProcedureColumns != null && procedure.ProcedureColumns.Count > 0)
            {
                procedureNode.Nodes.Add(columnsNode);

                foreach (ProcedureColumn column in procedure.ProcedureColumns.OrderBy<ProcedureColumn, int>(c => c.ColumnOrdinal ?? 0))
                {
                    TreeNode columnNode = new TreeNode(column.ToString());
                    columnNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureColumn, DbObject = column };
                    columnNode.ImageIndex = (int)ImageType.Column;
                    columnNode.SelectedImageIndex = (int)ImageType.Column;
                    columnsNode.Nodes.Add(columnNode);
                }
            }
            else if (procedure.Error != null)
            {
                procedureNode.ForeColor = Color.Red;
            }

            return procedureNode;
        }

        private TreeNode BuildFunctionsNode(Database database)
        {
            TreeNode functionsNode = new TreeNode("Table-valued Functions");
            functionsNode.Tag = new NodeTag() { NodeType = POCODbType.Functions, DbObject = database };
            functionsNode.ImageIndex = (int)ImageType.Folder;
            functionsNode.SelectedImageIndex = (int)ImageType.Folder;
            return functionsNode;
        }

        private TreeNode BuildFunctionNode(Function function)
        {
            TreeNode functionNode = new TreeNode(function.ToString().Replace("dbo.", ""));
            functionNode.Tag = new NodeTag() { NodeType = POCODbType.Function, DbObject = function };
            functionNode.ImageIndex = (int)ImageType.Function;
            functionNode.SelectedImageIndex = (int)ImageType.Function;

            TreeNode parametersNode = new TreeNode("Parameters");
            parametersNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureParameters, DbObject = function };
            parametersNode.ImageIndex = (int)ImageType.Folder;
            parametersNode.SelectedImageIndex = (int)ImageType.Folder;
            functionNode.Nodes.Add(parametersNode);

            if (function.ProcedureParameters != null && function.ProcedureParameters.Count > 0)
            {
                foreach (ProcedureParameter parameter in function.ProcedureParameters.OrderBy<ProcedureParameter, int>(c => c.ordinal_position ?? 0))
                {
                    TreeNode parameterNode = new TreeNode(parameter.ToString());
                    parameterNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureParameter, DbObject = parameter };
                    parameterNode.ImageIndex = (int)ImageType.Column;
                    parameterNode.SelectedImageIndex = (int)ImageType.Column;
                    parametersNode.Nodes.Add(parameterNode);
                }
            }

            TreeNode columnsNode = new TreeNode("Columns");
            columnsNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureColumns, DbObject = function };
            columnsNode.ImageIndex = (int)ImageType.Folder;
            columnsNode.SelectedImageIndex = (int)ImageType.Folder;

            if (function.ProcedureColumns != null && function.ProcedureColumns.Count > 0)
            {
                functionNode.Nodes.Add(columnsNode);

                foreach (ProcedureColumn column in function.ProcedureColumns.OrderBy<ProcedureColumn, int>(c => c.ColumnOrdinal ?? 0))
                {
                    TreeNode columnNode = new TreeNode(column.ToString());
                    columnNode.Tag = new NodeTag() { NodeType = POCODbType.ProcedureColumn, DbObject = column };
                    columnNode.ImageIndex = (int)ImageType.Column;
                    columnNode.SelectedImageIndex = (int)ImageType.Column;
                    columnsNode.Nodes.Add(columnNode);
                }
            }
            else if (function.Error != null)
            {
                functionNode.ForeColor = Color.Red;
            }

            return functionNode;
        }

        private TreeNode BuildTVPsNode(Database database)
        {
            TreeNode tvpsNode = new TreeNode("User-Defined Table Types");
            tvpsNode.Tag = new NodeTag() { NodeType = POCODbType.TVPs, DbObject = database };
            tvpsNode.ImageIndex = (int)ImageType.Folder;
            tvpsNode.SelectedImageIndex = (int)ImageType.Folder;
            return tvpsNode;
        }

        private TreeNode BuildTVPNode(TVP tvp)
        {
            TreeNode tvpNode = new TreeNode(tvp.ToString());
            tvpNode.Tag = new NodeTag() { NodeType = POCODbType.TVP, DbObject = tvp };
            tvpNode.ImageIndex = (int)ImageType.Table;
            tvpNode.SelectedImageIndex = (int)ImageType.Table;

            TreeNode columnsNode = new TreeNode("Columns");
            columnsNode.Tag = new NodeTag() { NodeType = POCODbType.TVPColumns, DbObject = tvp };
            columnsNode.ImageIndex = (int)ImageType.Folder;
            columnsNode.SelectedImageIndex = (int)ImageType.Folder;
            tvpNode.Nodes.Add(columnsNode);

            if (tvp.TVPColumns != null)
            {
                foreach (TVPColumn column in tvp.TVPColumns.OrderBy<TVPColumn, int>(c => c.column_id))
                {
                    TreeNode columnNode = new TreeNode(column.ToString());
                    columnNode.Tag = new NodeTag() { NodeType = POCODbType.TVPColumn, DbObject = column };
                    columnNode.ImageIndex = (int)ImageType.Column;
                    columnNode.SelectedImageIndex = (int)ImageType.Column;
                    columnsNode.Nodes.Add(columnNode);
                }
            }
            else if (tvp.Error != null)
            {
                tvpNode.ForeColor = Color.Red;
            }

            return tvpNode;
        }

        #endregion Server Tree

        #region Server Tree CheckBoxes

        private void trvServer_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            POCODbType nodeType = ((NodeTag)e.Node.Tag).NodeType;

            bool isDrawCheckBox =
                nodeType == POCODbType.Database ||
                nodeType == POCODbType.Tables ||
                nodeType == POCODbType.Views ||
                nodeType == POCODbType.Procedures ||
                nodeType == POCODbType.Functions ||
                nodeType == POCODbType.TVPs ||
                nodeType == POCODbType.Table ||
                nodeType == POCODbType.View ||
                nodeType == POCODbType.Procedure ||
                nodeType == POCODbType.Function ||
                nodeType == POCODbType.TVP;

            if (isDrawCheckBox == false)
                trvServer.HideCheckBox(e.Node);
            e.DrawDefault = true;
        }

        private void trvServer_AfterCheck(object sender, TreeViewEventArgs e)
        {
            trvServer.AfterCheck -= trvServer_AfterCheck;

            SetChildrenCheckBoxes(e.Node);

            TreeNode root = e.Node;
            while (root != null)
            {
                root.Checked = IsAllChildrenChecked(root);
                root = root.Parent;
            }

            trvServer.AfterCheck += trvServer_AfterCheck;

            Program.PropertyForm.GetOptions(true, trvServer);
        }

        private void SetChildrenCheckBoxes(TreeNode root)
        {
            if (root != null)
            {
                bool isChecked = root.Checked;
                foreach (TreeNode node in root.Nodes)
                {
                    node.Checked = isChecked;
                    SetChildrenCheckBoxes(node);
                }
            }
        }

        private bool IsAllChildrenChecked(TreeNode root)
        {
            if (root != null)
            {
                foreach (TreeNode node in root.Nodes)
                {
                    if (node.Checked == false)
                        return false;
                    if (IsAllChildrenChecked(node) == false)
                        return false;
                }
            }

            return true;
        }

        #endregion Server Tree CheckBoxes

        #region Server Tree Context Menu

        private static FilterSettingsForm filterSettingsForm = new FilterSettingsForm();
        private static Dictionary<TreeNode, FilterSettings> filters = new Dictionary<TreeNode, FilterSettings>();
        private const string filteredPostfix = " (filtered)";

        private void trvServer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point point = new Point(e.X, e.Y);

                TreeNode node = trvServer.GetNodeAt(point);
                if (node != null)
                {
                    trvServer.SelectedNode = node;

                    POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

                    bool isShowContextMenu = false;
                    removeFilterToolStripMenuItem.Visible = false;
                    filterSettingsToolStripMenuItem.Visible = false;
                    clearCheckBoxesToolStripMenuItem.Visible = false;
                    checkTablesConnectedFromToolStripMenuItem.Visible = false;
                    checkTablesConnectedToToolStripMenuItem.Visible = false;
                    checkTablesConnectedToolStripMenuItem.Visible = false;
                    checkRecursivelyTablesConnectedFromToolStripMenuItem.Visible = false;
                    checkRecursivelyTablesConnectedToToolStripMenuItem.Visible = false;
                    checkRecursivelyTablesConnectedToolStripMenuItem.Visible = false;
                    refreshToolStripMenuItem.Visible = false;
                    refreshTableToolStripMenuItem.Visible = false;

                    if (nodeType == POCODbType.Database)
                    {
                        isShowContextMenu = true;
                        clearCheckBoxesToolStripMenuItem.Visible = true;
                    }
                    else if (
                        nodeType == POCODbType.Tables ||
                        nodeType == POCODbType.Views ||
                        nodeType == POCODbType.Procedures ||
                        nodeType == POCODbType.Functions ||
                        nodeType == POCODbType.TVPs)
                    {
                        isShowContextMenu = true;
                        removeFilterToolStripMenuItem.Visible = true;
                        removeFilterToolStripMenuItem.Enabled = filters.ContainsKey(node);
                        filterSettingsToolStripMenuItem.Visible = true;
                        clearCheckBoxesToolStripMenuItem.Visible = true;
                    }
                    else if (
                        nodeType == POCODbType.Table ||
                        nodeType == POCODbType.View ||
                        nodeType == POCODbType.Procedure ||
                        nodeType == POCODbType.Function ||
                        nodeType == POCODbType.TVP)
                    {
                        isShowContextMenu = true;
                        if (nodeType == POCODbType.Table)
                        {
                            checkTablesConnectedFromToolStripMenuItem.Visible = true;
                            checkTablesConnectedToToolStripMenuItem.Visible = true;
                            checkTablesConnectedToolStripMenuItem.Visible = true;
                            checkRecursivelyTablesConnectedFromToolStripMenuItem.Visible = true;
                            checkRecursivelyTablesConnectedToToolStripMenuItem.Visible = true;
                            checkRecursivelyTablesConnectedToolStripMenuItem.Visible = true;
                            refreshTableToolStripMenuItem.Visible = true;
                        }
                        else
                        {
                            refreshToolStripMenuItem.Visible = true;
                        }
                    }

                    if (isShowContextMenu)
                    {
                        if (nodeType == POCODbType.Table)
                            contextMenuTable.Show(trvServer, point);
                        else
                            contextMenu.Show(trvServer, point);
                    }
                    else
                    {
                        contextMenu.Hide();
                        contextMenuTable.Hide();
                    }
                }
                else
                {
                    contextMenu.Hide();
                    contextMenuTable.Hide();
                }
            }
        }

        private void removeFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = trvServer.SelectedNode;
            if (node == null)
                return;

            if (filters.ContainsKey(node))
            {
                FilterSettings filterSettings = filters[node];
                foreach (TreeNode child in filterSettings.Nodes)
                    node.Nodes.AddSorted(child);
                filters.Remove(node);
                node.Text = node.Text.Replace(filteredPostfix, string.Empty);
            }
        }

        private void filterSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = trvServer.SelectedNode;
            if (node == null)
                return;

            bool isContains = filters.ContainsKey(node);
            if (isContains)
                filterSettingsForm.SetFilter(filters[node]);
            else
                filterSettingsForm.ClearFilter();

            DialogResult dialogResult = filterSettingsForm.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
            {
                FilterSettings filterSettings = filterSettingsForm.GetFilter();
                if (isContains)
                {
                    filterSettings.Nodes = filters[node].Nodes;
                    filters.Remove(node);
                }
                filters.Add(node, filterSettings);
                if (isContains == false)
                    node.Text += filteredPostfix;

                POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

                List<TreeNode> outList = new List<TreeNode>();
                List<TreeNode> inList = new List<TreeNode>();

                if (nodeType == POCODbType.Tables || nodeType == POCODbType.Views)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        Table table = (Table)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, table.table_name, table.table_schema);
                        if (isMatchFilter == false)
                            outList.Add(child);
                    }

                    foreach (TreeNode child in filterSettings.Nodes)
                    {
                        Table table = (Table)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, table.table_name, table.table_schema);
                        if (isMatchFilter)
                            inList.Add(child);
                    }
                }
                else if (nodeType == POCODbType.Procedures || nodeType == POCODbType.Functions)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        Procedure procedure = (Procedure)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, procedure.routine_name, procedure.routine_schema);
                        if (isMatchFilter == false)
                            outList.Add(child);
                    }

                    foreach (TreeNode child in filterSettings.Nodes)
                    {
                        Procedure procedure = (Procedure)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, procedure.routine_name, procedure.routine_schema);
                        if (isMatchFilter)
                            inList.Add(child);
                    }
                }
                else if (nodeType == POCODbType.TVPs)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        TVP tvp = (TVP)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, tvp.tvp_name, tvp.tvp_schema);
                        if (isMatchFilter == false)
                            outList.Add(child);
                    }

                    foreach (TreeNode child in filterSettings.Nodes)
                    {
                        TVP tvp = (TVP)((NodeTag)child.Tag).DbObject;
                        bool isMatchFilter = IsMatchFilter(filterSettings, tvp.tvp_name, tvp.tvp_schema);
                        if (isMatchFilter)
                            inList.Add(child);
                    }
                }

                foreach (TreeNode child in outList)
                {
                    node.Nodes.Remove(child);
                    filterSettings.Nodes.Add(child);
                }

                foreach (TreeNode child in inList)
                {
                    filterSettings.Nodes.Remove(child);
                    node.Nodes.AddSorted(child);
                }
            }
        }

        private bool IsMatchFilter(FilterSettings filterSettings, string name, string schema)
        {
            bool isMatchFilter = true;
            if (string.IsNullOrEmpty(filterSettings.FilterName.Filter) == false)
            {
                if (filterSettings.FilterName.FilterType == FilterType.Equals)
                    isMatchFilter = (string.Compare(name, filterSettings.FilterName.Filter, true) == 0);
                else if (filterSettings.FilterName.FilterType == FilterType.Contains)
                    isMatchFilter = (name.IndexOf(filterSettings.FilterName.Filter, StringComparison.CurrentCultureIgnoreCase) != -1);
                else if (filterSettings.FilterName.FilterType == FilterType.Does_Not_Contain)
                    isMatchFilter = (name.IndexOf(filterSettings.FilterName.Filter, StringComparison.CurrentCultureIgnoreCase) == -1);
            }

            if (isMatchFilter == false)
                return false;

            isMatchFilter = true;
            if (string.IsNullOrEmpty(filterSettings.FilterSchema.Filter) == false)
            {
                if (filterSettings.FilterSchema.FilterType == FilterType.Equals)
                    isMatchFilter = (string.Compare(schema, filterSettings.FilterSchema.Filter, true) == 0);
                else if (filterSettings.FilterSchema.FilterType == FilterType.Contains)
                    isMatchFilter = (schema.IndexOf(filterSettings.FilterSchema.Filter, StringComparison.CurrentCultureIgnoreCase) != -1);
                else if (filterSettings.FilterSchema.FilterType == FilterType.Does_Not_Contain)
                    isMatchFilter = (schema.IndexOf(filterSettings.FilterSchema.Filter, StringComparison.CurrentCultureIgnoreCase) == -1);
            }

            return isMatchFilter;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = trvServer.SelectedNode;
            if (node == null)
                return;

            POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

            if (nodeType == POCODbType.Table)
            {
                Table table = (Table)((NodeTag)node.Tag).DbObject;
                DbHelper.GetTableSchema(table,
                    (IDbObject dbObject) =>
                    {
                        table.Error = null;
                    },
                    (IDbObject dbObject) =>
                    {
                        AddTableNode(node.Parent, table);
                        node.Remove();
                    }
                );
            }
            else if (nodeType == POCODbType.View)
            {
                Db.DbObject.View view = (Db.DbObject.View)((NodeTag)node.Tag).DbObject;
                DbHelper.GetViewSchema(view,
                    (IDbObject dbObject) =>
                    {
                        view.Error = null;
                    },
                    (IDbObject dbObject) =>
                    {
                        AddViewNode(node.Parent, view);
                        node.Remove();
                    }
                );
            }
            else if (nodeType == POCODbType.Procedure)
            {
                Procedure procedure = (Procedure)((NodeTag)node.Tag).DbObject;
                DbHelper.GetProcedureSchema(procedure,
                    (IDbObject dbObject) =>
                    {
                        procedure.Error = null;
                    },
                    (IDbObject dbObject) =>
                    {
                        AddProcedureNode(node.Parent, procedure);
                        node.Remove();
                    }
                );
            }
            else if (nodeType == POCODbType.Function)
            {
                Function function = (Function)((NodeTag)node.Tag).DbObject;
                DbHelper.GetFunctionSchema(function,
                    (IDbObject dbObject) =>
                    {
                        function.Error = null;
                    },
                    (IDbObject dbObject) =>
                    {
                        AddFunctionNode(node.Parent, function);
                        node.Remove();
                    }
                );
            }
            else if (nodeType == POCODbType.TVP)
            {
                TVP tvp = (TVP)((NodeTag)node.Tag).DbObject;
                DbHelper.GetTVPSchema(tvp,
                    (IDbObject dbObject) =>
                    {
                        tvp.Error = null;
                    },
                    (IDbObject dbObject) =>
                    {
                        AddTVPNode(node.Parent, tvp);
                        node.Remove();
                    }
                );
            }
        }

        private void clearCheckBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = trvServer.SelectedNode;
            if (node == null)
                return;

            POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

            if (nodeType == POCODbType.Database)
            {
                trvServer.AfterCheck -= trvServer_AfterCheck;

                node.Checked = false;
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = false;
                    foreach (TreeNode child1 in child.Nodes)
                        child1.Checked = false;
                }

                trvServer.AfterCheck += trvServer_AfterCheck;
            }
            else if (nodeType == POCODbType.Tables ||
                nodeType == POCODbType.Views ||
                nodeType == POCODbType.Procedures ||
                nodeType == POCODbType.Functions ||
                nodeType == POCODbType.TVPs)
            {
                trvServer.AfterCheck -= trvServer_AfterCheck;

                node.Checked = false;
                foreach (TreeNode child in node.Nodes)
                    child.Checked = false;

                trvServer.AfterCheck += trvServer_AfterCheck;
            }
        }

        private void checkTablesConnectedFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(true, false, false);
        }

        private void checkTablesConnectedToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(false, true, false);
        }

        private void checkTablesConnectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(true, true, false);
        }

        private void checkRecursivelyTablesConnectedFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(true, false, true);
        }

        private void checkRecursivelyTablesConnectedToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(false, true, true);
        }

        private void checkRecursivelyTablesConnectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkTablesConnected(true, true, true);
        }

        private void checkTablesConnected(bool refFrom, bool refTo, bool recursively)
        {
            TreeNode node = trvServer.SelectedNode;
            if (node == null)
                return;

            POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

            if (nodeType == POCODbType.Table)
            {
                trvServer.AfterCheck -= trvServer_AfterCheck;

                Table table = (Table)((NodeTag)node.Tag).DbObject;
                List<Table> tables = DbHelper.GetConnectedTables(table, refFrom, refTo, recursively);

                TreeNode serverNode = trvServer.Nodes[0];
                foreach (TreeNode databaseNode in serverNode.Nodes)
                {
                    if (((NodeTag)databaseNode.Tag).NodeType == POCODbType.Database && ((NodeTag)databaseNode.Tag).DbObject == table.Database)
                    {
                        foreach (TreeNode tablesNode in databaseNode.Nodes)
                        {
                            if (((NodeTag)tablesNode.Tag).NodeType == POCODbType.Tables)
                            {
                                foreach (TreeNode tableNode in tablesNode.Nodes)
                                {
                                    if (tables.Contains((Table)((NodeTag)tableNode.Tag).DbObject))
                                        tableNode.Checked = true;
                                }

                                break;
                            }
                        }

                        break;
                    }
                }

                trvServer.AfterCheck += trvServer_AfterCheck;
            }
        }

        #endregion Server Tree Context Menu

        #region POCO Writer & Iterator

        internal void IterateDbObjects(IDbObjectTraverse dbObject, StringBuilder sb = null)
        {
            Program.PropertyForm.LoadConfig(GetPOCOIterator(new IDbObjectTraverse[] { dbObject }, sb));
            Program.PropertyForm.Iterator.Iterate();
        }

        internal void IterateDbObjects(IEnumerable<IDbObjectTraverse> dbObjects, StringBuilder sb = null)
        {
            Program.PropertyForm.LoadConfig(GetPOCOIterator(dbObjects, sb));
            Program.PropertyForm.Iterator.Iterate();
        }

        private void ClearDbObjects(StringBuilder sb = null)
        {
            Program.PropertyForm.LoadConfig(GetPOCOIterator(null, sb));
            Program.PropertyForm.Iterator.Clear();
        }

        private void WriteErrors(string objectName, IEnumerable<Exception> errors, StringBuilder sb = null)
        {
            if (errors != null && errors.Any())
            {
                IPOCOWriter pocoWriter = GetPOCOWriter(sb);

                pocoWriter.WriteLineError("/*");
                pocoWriter.WriteLineError(objectName);

                Exception lastError = errors.Last();
                foreach (Exception error in errors)
                {
                    Exception currentError = error;
                    while (currentError != null)
                    {
                        pocoWriter.WriteLineError(currentError.Message);
                        currentError = currentError.InnerException;
                    }

                    if (error != lastError)
                        pocoWriter.WriteLine();
                }

                pocoWriter.WriteLineError("*/");
            }
        }

        private DbIterator GetPOCOIterator(IEnumerable<IDbObjectTraverse> dbObjects, StringBuilder sb)
        {
            IPOCOWriter pocoWriter = GetPOCOWriter(sb);
            Program.PropertyForm.LoadConfig(new DbIterator(dbObjects, pocoWriter));
            return Program.PropertyForm.Iterator;
        }

        private IPOCOWriter GetPOCOWriter(StringBuilder sb)
        {
            if (sb == null)
                return new RichTextBoxWriterFactory(txtPocoEditor).CreatePOCOWriter();
            else
                return new StringBuilderWriterFactory(sb).CreatePOCOWriter();
        }

        #endregion POCO Writer & Iterator

        #region POCO Editor

        internal List<IDbObjectTraverse> GetSelectedObjects()
        {
            List<IDbObjectTraverse> selectedObjects = new List<IDbObjectTraverse>();
            TreeNode serverNode = trvServer.Nodes[0];
            GetSelectedObjects(serverNode, selectedObjects);
            return selectedObjects;
        }

        private void GetSelectedObjects(TreeNode node, List<IDbObjectTraverse> selectedObjects)
        {
            POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

            bool isDbObjectTraverse =
                nodeType == POCODbType.Table ||
                nodeType == POCODbType.View ||
                nodeType == POCODbType.Procedure ||
                nodeType == POCODbType.Function ||
                nodeType == POCODbType.TVP;

            if (isDbObjectTraverse && node.Checked)
                selectedObjects.Add(((NodeTag)node.Tag).DbObject as IDbObjectTraverse);

            if (isDbObjectTraverse == false)
            {
                foreach (TreeNode child in node.Nodes)
                    GetSelectedObjects(child, selectedObjects);
            }
        }

        private List<IDbObjectTraverse> selectedObjectsPrevious = new List<IDbObjectTraverse>();
        private TreeNode selectedNodePrevious;

        private void WritePocoToEditor(bool forceRefresh = true)
        {
            List<IDbObjectTraverse> selectedObjects = GetSelectedObjects();
            TreeNode selectedNode = trvServer.SelectedNode;

            if (selectedObjects.Count > 0)
            {
                if (forceRefresh || selectedObjects.Except(selectedObjectsPrevious).Any() || selectedObjectsPrevious.Except(selectedObjects).Any())
                    IterateDbObjects(selectedObjects);
            }
            else if (selectedNode != null && (forceRefresh || selectedNode != selectedNodePrevious || selectedObjectsPrevious.Count > 0))
            {
                IDbObjectTraverse dbObject = null;
                POCODbType nodeType = ((NodeTag)selectedNode.Tag).NodeType;
                if (nodeType == POCODbType.Table || nodeType == POCODbType.View || nodeType == POCODbType.Columns)
                    dbObject = (Table)((NodeTag)selectedNode.Tag).DbObject;
                else if (nodeType == POCODbType.Column)
                    dbObject = ((TableColumn)((NodeTag)selectedNode.Tag).DbObject).Table;
                else if (nodeType == POCODbType.Procedure || nodeType == POCODbType.Function || nodeType == POCODbType.ProcedureParameters || nodeType == POCODbType.ProcedureColumns)
                    dbObject = (Procedure)((NodeTag)selectedNode.Tag).DbObject;
                else if (nodeType == POCODbType.ProcedureParameter)
                    dbObject = ((ProcedureParameter)((NodeTag)selectedNode.Tag).DbObject).Procedure;
                else if (nodeType == POCODbType.ProcedureColumn)
                    dbObject = ((ProcedureColumn)((NodeTag)selectedNode.Tag).DbObject).Procedure;
                else if (nodeType == POCODbType.TVP || nodeType == POCODbType.TVPColumns)
                    dbObject = (TVP)((NodeTag)selectedNode.Tag).DbObject;
                else if (nodeType == POCODbType.TVPColumn)
                    dbObject = ((TVPColumn)((NodeTag)selectedNode.Tag).DbObject).TVP;

                IDbObjectTraverse dbObjectPrevious = null;
                if (selectedNodePrevious != null && forceRefresh == false && selectedObjectsPrevious.Count == 0)
                {
                    POCODbType nodeTypePrevious = ((NodeTag)selectedNodePrevious.Tag).NodeType;
                    if (nodeTypePrevious == POCODbType.Table || nodeTypePrevious == POCODbType.View || nodeTypePrevious == POCODbType.Columns)
                        dbObjectPrevious = (Table)((NodeTag)selectedNodePrevious.Tag).DbObject;
                    else if (nodeTypePrevious == POCODbType.Column)
                        dbObjectPrevious = ((TableColumn)((NodeTag)selectedNodePrevious.Tag).DbObject).Table;
                    else if (nodeTypePrevious == POCODbType.Procedure || nodeTypePrevious == POCODbType.Function || nodeTypePrevious == POCODbType.ProcedureParameters || nodeTypePrevious == POCODbType.ProcedureColumns)
                        dbObjectPrevious = (Procedure)((NodeTag)selectedNodePrevious.Tag).DbObject;
                    else if (nodeTypePrevious == POCODbType.ProcedureParameter)
                        dbObjectPrevious = ((ProcedureParameter)((NodeTag)selectedNodePrevious.Tag).DbObject).Procedure;
                    else if (nodeTypePrevious == POCODbType.ProcedureColumn)
                        dbObjectPrevious = ((ProcedureColumn)((NodeTag)selectedNodePrevious.Tag).DbObject).Procedure;
                    else if (nodeTypePrevious == POCODbType.TVP || nodeTypePrevious == POCODbType.TVPColumns)
                        dbObjectPrevious = (TVP)((NodeTag)selectedNodePrevious.Tag).DbObject;
                    else if (nodeTypePrevious == POCODbType.TVPColumn)
                        dbObjectPrevious = ((TVPColumn)((NodeTag)selectedNodePrevious.Tag).DbObject).TVP;
                }

                if (dbObject != null)
                {
                    if (dbObject != dbObjectPrevious || forceRefresh || selectedObjectsPrevious.Count > 0)
                        IterateDbObjects(dbObject);
                }
                else
                {
                    ClearDbObjects();

                    if (nodeType == POCODbType.Database)
                    {
                        Database database = (Database)((NodeTag)selectedNode.Tag).DbObject;
                        WriteErrors(database.ToString(), database.Errors);
                    }
                }
            }
            else if (forceRefresh || selectedNode == null)
            {
                ClearDbObjects();
            }

            selectedObjectsPrevious = selectedObjects;
            selectedNodePrevious = selectedNode;
        }

        #endregion POCO Editor

        #region Copy

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtPocoEditor.Text);
            }
            catch { }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtPocoEditor.SelectedText ?? string.Empty);
            }
            catch { }
            txtPocoEditor.Focus();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtPocoEditor.SelectAll();
            txtPocoEditor.Focus();
        }

        #endregion Copy

        #region Type Mapping

        private TypeMappingForm typeMappingForm;

        private void btnTypeMapping_Click(object sender, EventArgs e)
        {
            if (typeMappingForm == null)
                typeMappingForm = new TypeMappingForm();
            typeMappingForm.ShowDialog(this);
        }

        #endregion Type Mapping

        #region Command Line

        private CommandLineForm commandLineForm;

        private void btnCommandLine_Click(object sender, EventArgs e)
        {
            Options options = Program.PropertyForm.POCOConfig;
            List<string> shortCommandParts = CommandLineParser<Options>.GetCommandLineParts(options, true);
            List<string> longCommandParts = CommandLineParser<Options>.GetCommandLineParts(options, false);
            if (commandLineForm == null)
                commandLineForm = new CommandLineForm();
            commandLineForm.SetCommandLineParts(shortCommandParts, longCommandParts);
            commandLineForm.ShowDialog(this);
        }

        #endregion Command Line

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WritePocoToEditor();
        }
    }
}