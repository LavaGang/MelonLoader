using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
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
