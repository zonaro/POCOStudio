using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace POCOGenerator
{
    public partial class MainForm : Form
    {

        DockPanel dockPanel = new DockPanel() { Dock = DockStyle.Fill };

        POCOGeneratorForm PocoForm = new POCOGeneratorForm();

        public MainForm()
        {
            InitializeComponent();
            this.Controls.Add(dockPanel);
            this.dockPanel.Theme = new VS2015DarkTheme();
            PocoForm.Show(this.dockPanel, DockState.DockLeft);
        }
    }
}
