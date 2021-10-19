
namespace POCO_Studio
{
    partial class ConnectionDialog
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
            this.ServerBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DatabaseBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UserBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SecurityBox = new System.Windows.Forms.CheckBox();
            this.ConnectionstringBox = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelBT = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerBox
            // 
            this.ServerBox.Location = new System.Drawing.Point(73, 38);
            this.ServerBox.Name = "ServerBox";
            this.ServerBox.Size = new System.Drawing.Size(223, 23);
            this.ServerBox.TabIndex = 0;
            this.ServerBox.TextChanged += new System.EventHandler(this.ServerBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "DataBase";
            // 
            // DatabaseBox
            // 
            this.DatabaseBox.FormattingEnabled = true;
            this.DatabaseBox.Location = new System.Drawing.Point(73, 210);
            this.DatabaseBox.Name = "DatabaseBox";
            this.DatabaseBox.Size = new System.Drawing.Size(223, 23);
            this.DatabaseBox.TabIndex = 4;
            this.DatabaseBox.DropDown += new System.EventHandler(this.DatabaseBox_DropDown);
            this.DatabaseBox.SelectedIndexChanged += new System.EventHandler(this.DatabaseBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "User";
            // 
            // UserBox
            // 
            this.UserBox.Location = new System.Drawing.Point(78, 22);
            this.UserBox.Name = "UserBox";
            this.UserBox.Size = new System.Drawing.Size(188, 23);
            this.UserBox.TabIndex = 5;
            this.UserBox.TextChanged += new System.EventHandler(this.UserBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PasswordBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.UserBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(78, 51);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '*';
            this.PasswordBox.Size = new System.Drawing.Size(188, 23);
            this.PasswordBox.TabIndex = 7;
            this.PasswordBox.TextChanged += new System.EventHandler(this.PasswordBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Password";
            // 
            // SecurityBox
            // 
            this.SecurityBox.AutoSize = true;
            this.SecurityBox.Location = new System.Drawing.Point(171, 67);
            this.SecurityBox.Name = "SecurityBox";
            this.SecurityBox.Size = new System.Drawing.Size(125, 19);
            this.SecurityBox.TabIndex = 8;
            this.SecurityBox.Text = "Integrated Security";
            this.SecurityBox.UseVisualStyleBackColor = true;
            this.SecurityBox.CheckedChanged += new System.EventHandler(this.SecurityBox_CheckedChanged);
            // 
            // ConnectionstringBox
            // 
            this.ConnectionstringBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ConnectionstringBox.Location = new System.Drawing.Point(0, 0);
            this.ConnectionstringBox.Name = "ConnectionstringBox";
            this.ConnectionstringBox.Size = new System.Drawing.Size(308, 23);
            this.ConnectionstringBox.TabIndex = 9;
            this.ConnectionstringBox.TextChanged += new System.EventHandler(this.ConnectionstringBox_TextChanged);
            this.ConnectionstringBox.Leave += new System.EventHandler(this.ConnectionstringBox_Leave);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(221, 271);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 10;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelBT
            // 
            this.CancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBT.Location = new System.Drawing.Point(140, 271);
            this.CancelBT.Name = "CancelBT";
            this.CancelBT.Size = new System.Drawing.Size(75, 23);
            this.CancelBT.TabIndex = 11;
            this.CancelBT.Text = "Cancel";
            this.CancelBT.UseVisualStyleBackColor = true;
            this.CancelBT.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBT;
            this.ClientSize = new System.Drawing.Size(308, 306);
            this.Controls.Add(this.CancelBT);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.ConnectionstringBox);
            this.Controls.Add(this.SecurityBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DatabaseBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServerBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConnectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open SQL Server DataBase";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionDialog_FormClosing);
            this.Shown += new System.EventHandler(this.ConnectionDialog_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ServerBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox DatabaseBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UserBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox SecurityBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelBT;
        public System.Windows.Forms.TextBox ConnectionstringBox;
    }
}