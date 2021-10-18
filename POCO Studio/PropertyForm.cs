using Db.DbObject;
using Db.Helpers;
using Db.POCOIterator;
using POCOGenerator;
using POCOGenerator.CommandLine;
using POCOGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace POCO_Studio
{
    public partial class PropertyForm : DockContent
    {



        public Options POCOConfig { get => Iterator.POCOConfiguration; set => Iterator.POCOConfiguration = value; }
        public DbIterator Iterator { get; set; }

        public PropertyForm()
        {
            InitializeComponent();
        }

        private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void PropertyForm_Load(object sender, EventArgs e)
        {

        }

        public void LoadConfig(DbIterator dbIterator)
        {
            this.Iterator = dbIterator ?? this.Iterator;
            this.GRID.SelectedObject = this.POCOConfig;

        }


        private string settingsFileName => $"configuration.settings";

        private void LoadOptions()
        {
            try
            {
                if (File.Exists(settingsFileName) == false)
                    return;

                POCOConfig = SerializationHelper.BinaryDeserializeFromFile<Options>(settingsFileName);


            }
            catch
            {
            }
        }



        private void SetIncludeObjects(Options options, TreeView trvServer)
        {
            TreeNode serverNode = trvServer.Nodes[0];
            SetIncludeObjects(options, serverNode);

            options.IsIncludeAllTables = (options.IncludeTables.Count > 0 && options.ExcludeTables.Count == 0);
            options.IsExcludeAllTables = (options.IncludeTables.Count == 0 && options.ExcludeTables.Count > 0);

            options.IsIncludeAllViews = (options.IncludeViews.Count > 0 && options.ExcludeViews.Count == 0);
            options.IsExcludeAllViews = (options.IncludeViews.Count == 0 && options.ExcludeViews.Count > 0);

            options.IsIncludeAllStoredProcedures = (options.IncludeStoredProcedures.Count > 0 && options.ExcludeStoredProcedures.Count == 0);
            options.IsExcludeAllStoredProcedures = (options.IncludeStoredProcedures.Count == 0 && options.ExcludeStoredProcedures.Count > 0);

            options.IsIncludeAllFunctions = (options.IncludeFunctions.Count > 0 && options.ExcludeFunctions.Count == 0);
            options.IsExcludeAllFunctions = (options.IncludeFunctions.Count == 0 && options.ExcludeFunctions.Count > 0);

            options.IsIncludeAllTVPs = (options.IncludeTVPs.Count > 0 && options.ExcludeTVPs.Count == 0);
            options.IsExcludeAllTVPs = (options.IncludeTVPs.Count == 0 && options.ExcludeTVPs.Count > 0);

            options.IsIncludeAll =
                options.ExcludeTables.Count == 0 &&
                options.ExcludeViews.Count == 0 &&
                options.ExcludeStoredProcedures.Count == 0 &&
                options.ExcludeFunctions.Count == 0 &&
                options.ExcludeTVPs.Count == 0;

            bool isExcludeAll =
                options.IncludeTables.Count == 0 &&
                options.IncludeViews.Count == 0 &&
                options.IncludeStoredProcedures.Count == 0 &&
                options.IncludeFunctions.Count == 0 &&
                options.IncludeTVPs.Count == 0;

            if (options.IsIncludeAllTables)
                options.IncludeTables.Clear();
            if (options.IsExcludeAllTables)
                options.ExcludeTables.Clear();

            if (options.IsIncludeAllViews)
                options.IncludeViews.Clear();
            if (options.IsExcludeAllViews)
                options.ExcludeViews.Clear();

            if (options.IsIncludeAllStoredProcedures)
                options.IncludeStoredProcedures.Clear();
            if (options.IsExcludeAllStoredProcedures)
                options.ExcludeStoredProcedures.Clear();

            if (options.IsIncludeAllFunctions)
                options.IncludeFunctions.Clear();
            if (options.IsExcludeAllFunctions)
                options.ExcludeFunctions.Clear();

            if (options.IsIncludeAllTVPs)
                options.IncludeTVPs.Clear();
            if (options.IsExcludeAllTVPs)
                options.ExcludeTVPs.Clear();

            if (options.IsIncludeAll)
            {
                options.IsIncludeAllTables = false;
                options.IsIncludeAllViews = false;
                options.IsIncludeAllStoredProcedures = false;
                options.IsIncludeAllFunctions = false;
                options.IsIncludeAllTVPs = false;
            }
            else if (options.IsIncludeAll == false && isExcludeAll == false)
            {
                if (options.IsIncludeAllTables == false && options.IsExcludeAllTables == false && (options.IncludeTables.Count > 0 && options.ExcludeTables.Count > 0))
                {
                    if (1.0 * options.ExcludeTables.Count / (options.IncludeTables.Count + options.ExcludeTables.Count) < 0.5)
                    {
                        options.IsIncludeAllTables = true;
                        options.IncludeTables.Clear();
                    }
                    else
                    {
                        options.ExcludeTables.Clear();
                    }
                }

                if (options.IsIncludeAllViews == false && options.IsExcludeAllViews == false && (options.IncludeViews.Count > 0 && options.ExcludeViews.Count > 0))
                {
                    if (1.0 * options.ExcludeViews.Count / (options.IncludeViews.Count + options.ExcludeViews.Count) < 0.5)
                    {
                        options.IsIncludeAllViews = true;
                        options.IncludeViews.Clear();
                    }
                    else
                    {
                        options.ExcludeViews.Clear();
                    }
                }

                if (options.IsIncludeAllStoredProcedures == false && options.IsExcludeAllStoredProcedures == false && (options.IncludeStoredProcedures.Count > 0 && options.ExcludeStoredProcedures.Count > 0))
                {
                    if (1.0 * options.ExcludeStoredProcedures.Count / (options.IncludeStoredProcedures.Count + options.ExcludeStoredProcedures.Count) < 0.5)
                    {
                        options.IsIncludeAllStoredProcedures = true;
                        options.IncludeStoredProcedures.Clear();
                    }
                    else
                    {
                        options.ExcludeStoredProcedures.Clear();
                    }
                }

                if (options.IsIncludeAllFunctions == false && options.IsExcludeAllFunctions == false && (options.IncludeFunctions.Count > 0 && options.ExcludeFunctions.Count > 0))
                {
                    if (1.0 * options.ExcludeFunctions.Count / (options.IncludeFunctions.Count + options.ExcludeFunctions.Count) < 0.5)
                    {
                        options.IsIncludeAllFunctions = true;
                        options.IncludeFunctions.Clear();
                    }
                    else
                    {
                        options.ExcludeFunctions.Clear();
                    }
                }

                if (options.IsIncludeAllTVPs == false && options.IsExcludeAllTVPs == false && (options.IncludeTVPs.Count > 0 && options.ExcludeTVPs.Count > 0))
                {
                    if (1.0 * options.ExcludeTVPs.Count / (options.IncludeTVPs.Count + options.ExcludeTVPs.Count) < 0.5)
                    {
                        options.IsIncludeAllTVPs = true;
                        options.IncludeTVPs.Clear();
                    }
                    else
                    {
                        options.ExcludeTVPs.Clear();
                    }
                }
            }

            options.IsExcludeAllTables = false;
            options.IsExcludeAllViews = false;
            options.IsExcludeAllStoredProcedures = false;
            options.IsExcludeAllFunctions = false;
            options.IsExcludeAllTVPs = false;
        }

        public Options GetOptions(bool isIncludeObjects = true, TreeView view = null)
        {
            Options options = new Options();

            options.ConnectionString = DbHelper.ConnectionString;



            if (options.IsComments == false)
                options.IsCommentsWithoutNull = false;



            if (options.IsNavigationProperties == false)
            {
                options.IsNavigationPropertiesVirtual = false;
                options.IsNavigationPropertiesOverride = false;
                options.IsNavigationPropertiesShowJoinTable = false;
                options.IsNavigationPropertiesComments = false;
                options.IsNavigationPropertiesList = false;
                options.IsNavigationPropertiesICollection = false;
                options.IsNavigationPropertiesIEnumerable = false;
                options.IsEFForeignKey = false;
            }





            if (options.IsEF == false)
            {
                options.IsEFCore = false;
                options.IsEFColumn = false;
                options.IsEFRequired = false;
                options.IsEFRequiredWithErrorMessage = false;
                options.IsEFConcurrencyCheck = false;
                options.IsEFStringLength = false;
                options.IsEFDisplay = false;
                options.IsEFDescription = false;
                options.IsEFComplexType = false;
                options.IsEFIndex = false;
                options.IsEFForeignKey = false;
            }

            //options.Folder = txtFolder.Text;
            //options.IsSingleFile = chkSingleFile.Checked;
            //options.FileName = txtFileName.Text;

            options.IsIncludeAll = false;


            options.IncludeTables = new List<string>();
            options.ExcludeTables = new List<string>();


            options.IncludeViews = new List<string>();
            options.ExcludeViews = new List<string>();


            options.IncludeStoredProcedures = new List<string>();
            options.ExcludeStoredProcedures = new List<string>();


            options.IncludeFunctions = new List<string>();
            options.ExcludeFunctions = new List<string>();


            options.IncludeTVPs = new List<string>();
            options.ExcludeTVPs = new List<string>();

            if (isIncludeObjects)
                SetIncludeObjects(options,view);

            return options;
        }



        private void SetIncludeObjects(Options options, TreeNode node)
        {
            POCODbType nodeType = ((NodeTag)node.Tag).NodeType;

            if (nodeType == POCODbType.Table || nodeType == POCODbType.View || nodeType == POCODbType.Procedure || nodeType == POCODbType.Function || nodeType == POCODbType.TVP)
            {
                IDbObjectTraverse dbObject = ((NodeTag)node.Tag).DbObject as IDbObjectTraverse;
                if (nodeType == POCODbType.Table)
                    (node.Checked ? options.IncludeTables : options.ExcludeTables).Add(dbObject.Schema == "dbo" ? dbObject.Name : dbObject.ToString());
                else if (nodeType == POCODbType.View)
                    (node.Checked ? options.IncludeViews : options.ExcludeViews).Add(dbObject.Schema == "dbo" ? dbObject.Name : dbObject.ToString());
                else if (nodeType == POCODbType.Procedure)
                    (node.Checked ? options.IncludeStoredProcedures : options.ExcludeStoredProcedures).Add(dbObject.Schema == "dbo" ? dbObject.Name : dbObject.ToString());
                else if (nodeType == POCODbType.Function)
                    (node.Checked ? options.IncludeFunctions : options.ExcludeFunctions).Add(dbObject.Schema == "dbo" ? dbObject.Name : dbObject.ToString());
                else if (nodeType == POCODbType.TVP)
                    (node.Checked ? options.IncludeTVPs : options.ExcludeTVPs).Add(dbObject.Schema == "dbo" ? dbObject.Name : dbObject.ToString());
            }

            bool isRecursion =
                nodeType == POCODbType.Server ||
                nodeType == POCODbType.Database ||
                nodeType == POCODbType.Tables ||
                nodeType == POCODbType.Views ||
                nodeType == POCODbType.Procedures ||
                nodeType == POCODbType.Functions ||
                nodeType == POCODbType.TVPs;

            if (isRecursion)
            {
                foreach (TreeNode child in node.Nodes)
                    SetIncludeObjects(options, child);
            }
        }
        public void SaveOptions()
        {
            try
            {
                SerializationHelper.BinarySerializeToFile(POCOConfig, settingsFileName);
            }
            catch
            {
            }
        }


    }


}


