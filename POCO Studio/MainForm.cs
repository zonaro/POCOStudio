using Db.DbObject;
using InnerLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace POCOGenerator
{
    public partial class MainForm : Form
    {
        public void DockForm(DockContent Form, DockState State = DockState.Float) => Form.Show(this.dockPanel, State);

        public MainForm()
        {
            InitializeComponent();
            this.dockPanel.Theme = new VS2015DarkTheme();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DockForm(Program.PocoForm, DockState.DockLeft);
            DockForm(Program.PropertyForm, DockState.DockRight);
        }

        #region Export

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            var pt = txtFolder.Text;

            if (!pt.IsPath())
            {
                txtFolder.Text = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\Export";
            }

            if (pt.IsFilePath())
            {
                pt = new FileInfo(txtFolder.Text).Directory.FullName;
            }

            if (Program.PropertyForm.POCOConfig.IsSingleFile)
            {
                SaveFileDialogExport.InitialDirectory = pt;

                if (SaveFileDialogExport.ShowDialog(this) == DialogResult.OK)
                {
                    txtFolder.Text = SaveFileDialogExport.FileName;
                }
            }
            else
            {
                folderBrowserDialogExport.SelectedPath = pt;

                if (folderBrowserDialogExport.ShowDialog(this) == DialogResult.OK)
                {
                    txtFolder.Text = folderBrowserDialogExport.SelectedPath;
                }
            }
        }

        private List<IDbObjectTraverse> GetExportObjects()
        {
            List<IDbObjectTraverse> exportResults = Program.PocoForm.GetSelectedObjects();
            TreeNode selectedNode = Program.PocoForm.trvServer.SelectedNode;

            if (exportResults.Count == 0 && selectedNode != null)
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

                if (dbObject != null)
                    exportResults.Add(dbObject);
            }

            return exportResults;
        }

        private List<ExportResult> WritePocoToFiles(List<IDbObjectTraverse> exportObjects)
        {
            List<ExportResult> exportResults = new List<ExportResult>();

            if (exportObjects.Count > 0)
            {
                string folder = txtFolder.Text;

                if (txtFolder.Text.IsFilePath())
                {
                    var fileName = txtFolder.Text;
                    StringBuilder sb = new StringBuilder();
                    Program.PocoForm.IterateDbObjects(exportObjects, sb);

                    if (fileName.EndsWith(".cs") == false)
                        fileName += ".cs";
                    foreach (char c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c.ToString(), string.Empty);

                    Exception error = null;
                    bool succeeded = WritePocoToFile(folder, fileName, sb.ToString(), true, ref error);

                    foreach (var dbObject in exportObjects)
                    {
                        exportResults.Add(new ExportResult()
                        {
                            ObjectName = string.Format("{0}.{1}.{2}", dbObject.Database.ToString(), dbObject.Schema, dbObject.Name),
                            ClassName = dbObject.ClassName,
                            FileName = fileName,
                            Succeeded = succeeded,
                            Error = error
                        });
                    }
                }
                else
                {
                    foreach (var dbObject in exportObjects)
                    {
                        StringBuilder sb = new StringBuilder();
                        Program.PocoForm.IterateDbObjects(dbObject, sb);

                        var fileName = dbObject.ClassName + ".cs";
                        foreach (char c in Path.GetInvalidFileNameChars())
                            fileName = fileName.Replace(c.ToString(), string.Empty);

                        Exception error = null;
                        bool succeeded = WritePocoToFile(folder, fileName, sb.ToString(), false, ref error);

                        exportResults.Add(new ExportResult()
                        {
                            ObjectName = string.Format("{0}.{1}.{2}", dbObject.Database.ToString(), dbObject.Schema, dbObject.Name),
                            ClassName = dbObject.ClassName,
                            FileName = fileName,
                            Succeeded = succeeded,
                            Error = error
                        });
                    }
                }
            }

            return exportResults;
        }

        private bool WritePocoToFile(string folder, string fileName, string content, bool isAppend, ref Exception error)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName == ".cs")
                    throw new Exception("File name isn't set");

                string path = folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + fileName;
                if (isAppend)
                    File.AppendAllText(path, content);
                else
                    File.WriteAllText(path, content);

                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        private class ExportResult
        {
            public string ObjectName { get; set; }
            public string ClassName { get; set; }
            public string FileName { get; set; }
            public bool Succeeded { get; set; }
            public Exception Error { get; set; }
        }

        #endregion Export

        private void txtFolder_Click(object sender, EventArgs e)
        {
            if (txtFolder.Text.IsDirectoryPath())
            {
                Process.Start(txtFolder.Text);
            }
            else if (txtFolder.Text.IsFilePath())
            {
                Process.Start(new FileInfo(txtFolder.Text).Directory.FullName);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Program.PropertyForm.SaveOptions();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = string.Empty;
            Application.DoEvents();

            if (txtFolder.Text.ToDirectoryInfo().Exists)
            {
                List<IDbObjectTraverse> exportObjects = GetExportObjects();
                if (exportObjects.Count == 0)
                    return;

                toolStripStatusLabel.Text = "Exporting...";
                toolStripStatusLabel.ForeColor = Color.Black;
                Application.DoEvents();

                List<ExportResult> exportResults = WritePocoToFiles(exportObjects);

                if (exportResults.Count > 0)
                {
                    var failed = exportResults.Where(r => r.Succeeded == false).ToList();

                    if (failed.Count == 0)
                    {
                        if (exportResults.Count > 1)
                            toolStripStatusLabel.Text = "All POCOs were exported successfully";
                        else
                            toolStripStatusLabel.Text = string.Format("Exported {0} successfully", exportResults[0].ObjectName);
                        toolStripStatusLabel.ForeColor = Color.Green;
                    }
                    else if (failed.Count == 1)
                    {
                        var exportResult = failed[0];
                        string fileName = exportResult.FileName;
                        if (fileName == ".cs")
                            fileName = string.Empty;
                        if (string.IsNullOrEmpty(fileName))
                            toolStripStatusLabel.Text = string.Format("Failed to export {0}", exportResult.ObjectName);
                        else
                            toolStripStatusLabel.Text = string.Format("Failed to export {0} to {1}", exportResult.ObjectName, fileName);
                        toolStripStatusLabel.ForeColor = Color.Red;
                    }
                    else if (failed.Count < exportResults.Count)
                    {
                        toolStripStatusLabel.Text = "Failed to export several POCOs";
                        toolStripStatusLabel.ForeColor = Color.Red;
                    }
                    else if (failed.Count == exportResults.Count)
                    {
                        toolStripStatusLabel.Text = "Failed to export all POCOs";
                        toolStripStatusLabel.ForeColor = Color.Red;
                    }
                }
            }
            else
            {
                toolStripStatusLabel.Text = "Failed to access directory";
            }
        }


        private TypeMappingForm typeMappingForm;

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            if (typeMappingForm == null)
                typeMappingForm = new TypeMappingForm();
            typeMappingForm.ShowDialog(this);
        }
    }

    internal class NodeTag
    {
        public POCODbType NodeType { get; set; }
        public IDbObject DbObject { get; set; }
    }
}