﻿namespace POCOGenerator
{
    partial class POCOGeneratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(POCOGeneratorForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trvServer = new System.Windows.Forms.TreeView();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCheckBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListDbObjects = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuPocoEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialogExport = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuTable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkTablesConnectedFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTablesConnectedToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTablesConnectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.txtPocoEditor = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.contextMenuPocoEditor.SuspendLayout();
            this.contextMenuTable.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trvServer);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 650;
            this.splitContainer1.Size = new System.Drawing.Size(1297, 994);
            this.splitContainer1.SplitterDistance = 478;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // trvServer
            // 
            this.trvServer.CheckBoxes = true;
            this.trvServer.ContextMenuStrip = this.contextMenu;
            this.trvServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvServer.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.trvServer.HideSelection = false;
            this.trvServer.ImageIndex = 0;
            this.trvServer.ImageList = this.imageListDbObjects;
            this.trvServer.Location = new System.Drawing.Point(0, 0);
            this.trvServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trvServer.Name = "trvServer";
            this.trvServer.SelectedImageIndex = 0;
            this.trvServer.Size = new System.Drawing.Size(478, 994);
            this.trvServer.TabIndex = 1;
            this.trvServer.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvServer_AfterCheck);
            this.trvServer.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.trvServer_DrawNode);
            this.trvServer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trvServer_MouseUp);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFilterToolStripMenuItem,
            this.filterSettingsToolStripMenuItem,
            this.clearCheckBoxesToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.contextMenu.Name = "contextMenuServerTree";
            this.contextMenu.Size = new System.Drawing.Size(169, 92);
            // 
            // removeFilterToolStripMenuItem
            // 
            this.removeFilterToolStripMenuItem.Name = "removeFilterToolStripMenuItem";
            this.removeFilterToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.removeFilterToolStripMenuItem.Text = "Remove Filter";
            this.removeFilterToolStripMenuItem.Click += new System.EventHandler(this.removeFilterToolStripMenuItem_Click);
            // 
            // filterSettingsToolStripMenuItem
            // 
            this.filterSettingsToolStripMenuItem.Name = "filterSettingsToolStripMenuItem";
            this.filterSettingsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.filterSettingsToolStripMenuItem.Text = "Filter Settings";
            this.filterSettingsToolStripMenuItem.Click += new System.EventHandler(this.filterSettingsToolStripMenuItem_Click);
            // 
            // clearCheckBoxesToolStripMenuItem
            // 
            this.clearCheckBoxesToolStripMenuItem.Name = "clearCheckBoxesToolStripMenuItem";
            this.clearCheckBoxesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.clearCheckBoxesToolStripMenuItem.Text = "Clear Checkboxes";
            this.clearCheckBoxesToolStripMenuItem.Click += new System.EventHandler(this.clearCheckBoxesToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // imageListDbObjects
            // 
            this.imageListDbObjects.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListDbObjects.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListDbObjects.ImageStream")));
            this.imageListDbObjects.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListDbObjects.Images.SetKeyName(0, "Server.gif");
            this.imageListDbObjects.Images.SetKeyName(1, "Database.gif");
            this.imageListDbObjects.Images.SetKeyName(2, "Folder.gif");
            this.imageListDbObjects.Images.SetKeyName(3, "Table.gif");
            this.imageListDbObjects.Images.SetKeyName(4, "View.gif");
            this.imageListDbObjects.Images.SetKeyName(5, "Procedure.gif");
            this.imageListDbObjects.Images.SetKeyName(6, "Function.gif");
            this.imageListDbObjects.Images.SetKeyName(7, "Column.gif");
            this.imageListDbObjects.Images.SetKeyName(8, "PK.gif");
            this.imageListDbObjects.Images.SetKeyName(9, "FK.gif");
            this.imageListDbObjects.Images.SetKeyName(10, "UK.gif");
            this.imageListDbObjects.Images.SetKeyName(11, "Index.gif");
            // 
            // contextMenuPocoEditor
            // 
            this.contextMenuPocoEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.selectAllToolStripMenuItem});
            this.contextMenuPocoEditor.Name = "contextMenuPocoEditor";
            this.contextMenuPocoEditor.Size = new System.Drawing.Size(123, 48);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // contextMenuTable
            // 
            this.contextMenuTable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkTablesConnectedFromToolStripMenuItem,
            this.checkTablesConnectedToToolStripMenuItem,
            this.checkTablesConnectedToolStripMenuItem,
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem,
            this.checkRecursivelyTablesConnectedToToolStripMenuItem,
            this.checkRecursivelyTablesConnectedToolStripMenuItem,
            this.refreshTableToolStripMenuItem});
            this.contextMenuTable.Name = "contextMenuTable";
            this.contextMenuTable.Size = new System.Drawing.Size(344, 158);
            // 
            // checkTablesConnectedFromToolStripMenuItem
            // 
            this.checkTablesConnectedFromToolStripMenuItem.Name = "checkTablesConnectedFromToolStripMenuItem";
            this.checkTablesConnectedFromToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkTablesConnectedFromToolStripMenuItem.Text = "Check Connected From This Table (FK This -> To)";
            this.checkTablesConnectedFromToolStripMenuItem.Click += new System.EventHandler(this.checkTablesConnectedFromToolStripMenuItem_Click);
            // 
            // checkTablesConnectedToToolStripMenuItem
            // 
            this.checkTablesConnectedToToolStripMenuItem.Name = "checkTablesConnectedToToolStripMenuItem";
            this.checkTablesConnectedToToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkTablesConnectedToToolStripMenuItem.Text = "Check Connected To This Table (FK From -> This)";
            this.checkTablesConnectedToToolStripMenuItem.Click += new System.EventHandler(this.checkTablesConnectedToToolStripMenuItem_Click);
            // 
            // checkTablesConnectedToolStripMenuItem
            // 
            this.checkTablesConnectedToolStripMenuItem.Name = "checkTablesConnectedToolStripMenuItem";
            this.checkTablesConnectedToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkTablesConnectedToolStripMenuItem.Text = "Check Connected From && To This Table";
            this.checkTablesConnectedToolStripMenuItem.Click += new System.EventHandler(this.checkTablesConnectedToolStripMenuItem_Click);
            // 
            // checkRecursivelyTablesConnectedFromToolStripMenuItem
            // 
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem.Name = "checkRecursivelyTablesConnectedFromToolStripMenuItem";
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem.Text = "Check Recursively Connected From This Table";
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem.Click += new System.EventHandler(this.checkRecursivelyTablesConnectedFromToolStripMenuItem_Click);
            // 
            // checkRecursivelyTablesConnectedToToolStripMenuItem
            // 
            this.checkRecursivelyTablesConnectedToToolStripMenuItem.Name = "checkRecursivelyTablesConnectedToToolStripMenuItem";
            this.checkRecursivelyTablesConnectedToToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkRecursivelyTablesConnectedToToolStripMenuItem.Text = "Check Recursively Connected To This Table";
            this.checkRecursivelyTablesConnectedToToolStripMenuItem.Click += new System.EventHandler(this.checkRecursivelyTablesConnectedToToolStripMenuItem_Click);
            // 
            // checkRecursivelyTablesConnectedToolStripMenuItem
            // 
            this.checkRecursivelyTablesConnectedToolStripMenuItem.Name = "checkRecursivelyTablesConnectedToolStripMenuItem";
            this.checkRecursivelyTablesConnectedToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.checkRecursivelyTablesConnectedToolStripMenuItem.Text = "Check Recursively Connected From && To This Table";
            this.checkRecursivelyTablesConnectedToolStripMenuItem.Click += new System.EventHandler(this.checkRecursivelyTablesConnectedToolStripMenuItem_Click);
            // 
            // refreshTableToolStripMenuItem
            // 
            this.refreshTableToolStripMenuItem.Name = "refreshTableToolStripMenuItem";
            this.refreshTableToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.refreshTableToolStripMenuItem.Text = "Refresh";
            this.refreshTableToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(814, 362);
            this.panelMain.TabIndex = 0;
            // 
            // txtPocoEditor
            // 
            this.txtPocoEditor.BackColor = System.Drawing.Color.White;
            this.txtPocoEditor.ContextMenuStrip = this.contextMenuPocoEditor;
            this.txtPocoEditor.DetectUrls = false;
            this.txtPocoEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPocoEditor.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPocoEditor.Location = new System.Drawing.Point(0, 0);
            this.txtPocoEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPocoEditor.Name = "txtPocoEditor";
            this.txtPocoEditor.ReadOnly = true;
            this.txtPocoEditor.Size = new System.Drawing.Size(814, 627);
            this.txtPocoEditor.TabIndex = 0;
            this.txtPocoEditor.Text = "";
            this.txtPocoEditor.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 603);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(814, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(89, 20);
            this.toolStripMenuItem1.Text = "Load Preview";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainer2.Panel1.Controls.Add(this.txtPocoEditor);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelMain);
            this.splitContainer2.Size = new System.Drawing.Size(814, 994);
            this.splitContainer2.SplitterDistance = 627;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // POCOGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1297, 994);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1281, 859);
            this.Name = "POCOGeneratorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POCO Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.POCOGeneratorForm_FormClosing);
            this.Load += new System.EventHandler(this.POCOGeneratorForm_Load);
            this.Shown += new System.EventHandler(this.POCOGeneratorForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.contextMenuPocoEditor.ResumeLayout(false);
            this.contextMenuTable.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuPocoEditor;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogExport;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCheckBoxesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuTable;
        private System.Windows.Forms.ToolStripMenuItem checkTablesConnectedFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkTablesConnectedToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkTablesConnectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkRecursivelyTablesConnectedFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkRecursivelyTablesConnectedToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkRecursivelyTablesConnectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshTableToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.RichTextBox txtPocoEditor;
        private System.Windows.Forms.Panel panelMain;
        internal System.Windows.Forms.TreeView trvServer;
        public System.Windows.Forms.ImageList imageListDbObjects;
    }
}