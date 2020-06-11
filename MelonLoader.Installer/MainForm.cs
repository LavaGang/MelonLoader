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

            this.comboBox1.AutoSize = false;
            this.comboBox1.SelectedIndex = 0;
            this.comboBox1.Text = (string)comboBox1.Items[comboBox1.SelectedIndex];
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.textBox1.AutoSize = false;

            this.label1.AutoSize = false;
            this.label1.Height = 2;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Unity Game (*.exe)|*.exe";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        textBox1.Text = filePath;
                        button2.Enabled = true;
                    }
                }
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {

        }
    }
}
