using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace MelonLoader.Installer
{
    public partial class MainForm : Form
    {
        internal string CurrentVersion = null;

        public MainForm()
        {
            InitializeComponent();
            Text = Program.Title;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            comboBox1.AutoSize = false;
            comboBox1.SelectedIndex = 0;
            comboBox1.Text = (string)comboBox1.Items[comboBox1.SelectedIndex];
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            textBox1.AutoSize = false;

            label1.AutoSize = false;
            label1.Height = 2;
            label1.BorderStyle = BorderStyle.Fixed3D;
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
                        // Check if Game Selected actually is a Unity Game

                        textBox1.Text = filePath;
                        button2.Enabled = true;

                        string existingFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(filePath), "MelonLoader"), "MelonLoader.ModHandler.dll");
                        if (File.Exists(existingFilePath))
                        {
                            string file_version = FileVersionInfo.GetVersionInfo(existingFilePath).FileVersion;
                            if (file_version.IndexOf(".0") >= 0)
                                file_version = file_version.Substring(0, file_version.IndexOf(".0"));
                            CurrentVersion = "v" + file_version;
                            string selectedVersion = (((comboBox1.SelectedIndex == 0) && (comboBox1.Items.Count > 1)) ? (string)comboBox1.Items[1] : (string)comboBox1.Items[comboBox1.SelectedIndex]);
                            if (CurrentVersion.Equals(selectedVersion))
                                button2.Text = "RE-INSTALL";
                            else
                                button2.Text = "INSTALL";
                        }
                        else
                        {
                            CurrentVersion = null;
                            button2.Text = "INSTALL";
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            comboBox1.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;

            progressBar1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;

            new Thread(() =>
            {
                try
                {
                    string dirpath = Path.GetDirectoryName(textBox1.Text);
                    string selectedVersion = (((comboBox1.SelectedIndex == 0) && (comboBox1.Items.Count > 1)) ? (string)comboBox1.Items[1] : (string)comboBox1.Items[comboBox1.SelectedIndex]);
                    bool legacy_install = (selectedVersion.Equals("v0.2.1") || selectedVersion.Equals("v0.2") || selectedVersion.Equals("v0.1.0"));

                    Program.Install(dirpath, selectedVersion, legacy_install);

                    Program.SetDisplayText("SUCCESS!");
                    Program.SetPercentage(100);
                    MessageBox.Show("Installation Successful!", Program.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Close();
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Program.SetDisplayText("ERROR!");
                    MessageBox.Show("Installation failed; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Program.Title);
                    Close();
                    Application.Exit();
                }
            }).Start();
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (button2.Enabled && !string.IsNullOrEmpty(CurrentVersion))
            {
                string selectedVersion = (((comboBox1.SelectedIndex == 0) && (comboBox1.Items.Count > 1)) ? (string)comboBox1.Items[1] : (string)comboBox1.Items[comboBox1.SelectedIndex]);
                if (CurrentVersion.Equals(selectedVersion))
                    button2.Text = "RE-INSTALL";
                else
                    button2.Text = "INSTALL";
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Check for Installer Updates
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ((e.CloseReason == CloseReason.WindowsShutDown) || (e.CloseReason == CloseReason.UserClosing) || (e.CloseReason == CloseReason.TaskManagerClosing))
                Process.GetCurrentProcess().Kill();
        }
    }
}
