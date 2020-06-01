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
