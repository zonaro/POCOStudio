
namespace POCO_Studio
{
    partial class PropertyForm
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
            this.GRID = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // GRID
            // 
            this.GRID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GRID.Location = new System.Drawing.Point(0, 0);
            this.GRID.Name = "GRID";
            this.GRID.Size = new System.Drawing.Size(800, 450);
            this.GRID.TabIndex = 0;
            // 
            // PropertyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GRID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PropertyForm";
            this.Text = "PropertyForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertyForm_FormClosing);
            this.Load += new System.EventHandler(this.PropertyForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid GRID;
    }
}