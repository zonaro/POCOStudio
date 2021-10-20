
namespace POCO_Studio
{
    partial class ContentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentForm));
            this.documentMap1 = new FastColoredTextBoxNS.DocumentMap();
            this.txtPocoEditor = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtPocoEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // documentMap1
            // 
            this.documentMap1.Dock = System.Windows.Forms.DockStyle.Right;
            this.documentMap1.ForeColor = System.Drawing.Color.Maroon;
            this.documentMap1.Location = new System.Drawing.Point(707, 0);
            this.documentMap1.Name = "documentMap1";
            this.documentMap1.Size = new System.Drawing.Size(93, 450);
            this.documentMap1.TabIndex = 0;
            this.documentMap1.Target = null;
            this.documentMap1.Text = "documentMap1";
            // 
            // txtPocoEditor
            // 
            this.txtPocoEditor.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.txtPocoEditor.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*" +
    "(?<range>:)\\s*(?<range>[^;]+);";
            this.txtPocoEditor.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.txtPocoEditor.BackBrush = null;
            this.txtPocoEditor.CharHeight = 15;
            this.txtPocoEditor.CharWidth = 7;
            this.txtPocoEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPocoEditor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.txtPocoEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPocoEditor.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPocoEditor.IsReplaceMode = false;
            this.txtPocoEditor.Location = new System.Drawing.Point(0, 0);
            this.txtPocoEditor.Name = "txtPocoEditor";
            this.txtPocoEditor.Paddings = new System.Windows.Forms.Padding(0);
            this.txtPocoEditor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtPocoEditor.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtPocoEditor.ServiceColors")));
            this.txtPocoEditor.Size = new System.Drawing.Size(707, 450);
            this.txtPocoEditor.TabIndex = 1;
            this.txtPocoEditor.Zoom = 100;
            // 
            // ContentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtPocoEditor);
            this.Controls.Add(this.documentMap1);
            this.Name = "ContentForm";
            this.Text = "POCO Document";
            ((System.ComponentModel.ISupportInitialize)(this.txtPocoEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.DocumentMap documentMap1;
        private FastColoredTextBoxNS.FastColoredTextBox txtPocoEditor;
    }
}