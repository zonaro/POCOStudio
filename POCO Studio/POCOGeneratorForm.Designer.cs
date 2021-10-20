namespace POCOGenerator
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
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCheckBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListDbObjects = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuTable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkTablesConnectedFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTablesConnectedToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTablesConnectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkRecursivelyTablesConnectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trvServer = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.contextMenu.SuspendLayout();
            this.contextMenuTable.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
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
            // trvServer
            // 
            this.trvServer.CheckBoxes = true;
            this.trvServer.ContextMenuStrip = this.contextMenu;
            this.trvServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvServer.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.trvServer.HideSelection = false;
            this.trvServer.ImageIndex = 0;
            this.trvServer.ImageList = this.imageListDbObjects;
            this.trvServer.Location = new System.Drawing.Point(0, 25);
            this.trvServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trvServer.Name = "trvServer";
            this.trvServer.SelectedImageIndex = 0;
            this.trvServer.Size = new System.Drawing.Size(281, 190);
            this.trvServer.TabIndex = 1;
            this.trvServer.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvServer_AfterCheck);
            this.trvServer.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.trvServer_DrawNode);
            this.trvServer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trvServer_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(281, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(91, 22);
            this.toolStripButton1.Text = "Open Database";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // POCOGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 215);
            this.Controls.Add(this.trvServer);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "POCOGeneratorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Databases";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.POCOGeneratorForm_FormClosing);
            this.Load += new System.EventHandler(this.POCOGeneratorForm_Load);
            this.Shown += new System.EventHandler(this.POCOGeneratorForm_Shown);
            this.contextMenu.ResumeLayout(false);
            this.contextMenuTable.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        public System.Windows.Forms.ImageList imageListDbObjects;
        internal System.Windows.Forms.TreeView trvServer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}