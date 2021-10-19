using InnerLibs;
using InnerLibs.MicroORM;
using POCOGenerator;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace POCO_Studio
{
    public partial class ConnectionDialog : Form
    {
        public ConnectionDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public SqlServerConnectionsStringParser Parser = new SqlServerConnectionsStringParser();

        private void ConnectionstringBox_TextChanged(object sender, EventArgs e)
        {

        }


        public SqlServerConnectionsStringParser LoadConnectionString(string ConnectionString)
        {
            try
            {
                Parser ??= new SqlServerConnectionsStringParser();
                Parser.ConnectionString = ConnectionString;
                ServerBox.Text = Parser.Server;
                UserBox.Text = Parser.UserID;
                PasswordBox.Text = Parser.Password;
                DatabaseBox.Text = Parser.InitialCatalog;
                SecurityBox.Checked = Parser.IntegratedSecurity;
                ConnectionstringBox.Text = Parser.ConnectionString;
            }
            catch
            {

            }

            return Parser;

        }

        private void ServerBox_TextChanged(object sender, EventArgs e)
        {
            Parser.Server = ServerBox.Text;
            ConnectionstringBox.Text = Parser.ToString();
        }

        private void UserBox_TextChanged(object sender, EventArgs e)
        {
            Parser.UserID = UserBox.Text;
            ConnectionstringBox.Text = Parser.ToString();
        }

        private void PasswordBox_TextChanged(object sender, EventArgs e)
        {
            Parser.Password = PasswordBox.Text;
            ConnectionstringBox.Text = Parser.ToString();
        }

        private void DatabaseBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Parser.InitialCatalog = DatabaseBox.Text;
            ConnectionstringBox.Text = Parser.ToString();
        }

        private void DatabaseBox_DropDown(object sender, EventArgs e)
        {
            try
            {
                DatabaseBox.Items.Clear();
                using (var conn = new SqlConnection(ConnectionstringBox.Text))
                {
                    var x = conn.RunSQLArray<string>($"SELECT name from sysdatabases").ToArray();
                    DatabaseBox.Items.AddRange(x);
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.toolStripStatusLabel.Text = ex.ToFullExceptionString();
            }
        }

        private void SecurityBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.SecurityBox.Checked)
            {
                groupBox1.Enabled = false;
                Parser.RemoveIfExist("User ID", "Password");
                Parser.IntegratedSecurity = true;
            }
            else
            {
                groupBox1.Enabled = true;
                Parser.UserID = UserBox.Text;
                Parser.Password = PasswordBox.Text;
                Parser.RemoveIfExist("Integrated Security");
            }

            ConnectionstringBox.Text = Parser.ToString();
        }

        private void ConnectionstringBox_Leave(object sender, EventArgs e)
        {
            Reparse();
        }

        private void ConnectionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Reparse();
        }

        private void Reparse()
        {
            var old = ConnectionstringBox.Text;
            try
            {
                ConnectionstringBox.Text = Parser.Parse(ConnectionstringBox.Text);
                UserBox.Text = Parser.UserID;
                PasswordBox.Text = Parser.Password;
                SecurityBox.Checked = Parser.IntegratedSecurity;
                ServerBox.Text = Parser.Server;
                DatabaseBox.Text = Parser.InitialCatalog;
            }
            catch { ConnectionstringBox.Text = old; }
        }

        private void ConnectionDialog_Shown(object sender, EventArgs e)
        {
            Reparse();
        }
    }
}