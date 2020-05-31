using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MelonLoader.Installer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Text = Program.Title;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
