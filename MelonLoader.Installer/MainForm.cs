using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace MelonLoader.Installer
{
    public partial class MainForm : Form
    {
        private string CurrentVersion = null;

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
                            string curVersion = "v0.0.0"; // Get Version from ModHandler
                            string selectedVersion = (((comboBox1.SelectedIndex == 0) && (comboBox1.Items.Count > 1)) ? (string)comboBox1.Items[1] : (string)comboBox1.Items[comboBox1.SelectedIndex]);
                            if (curVersion.Equals(selectedVersion))
                                button2.Text = "RE-INSTALL";
                            else
                                button2.Text = "INSTALL";
                            //CurrentVersion = curVersion;
                        }
                        else
                            button2.Text = "INSTALL";
                    }
                }
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    string dirpath = Path.GetDirectoryName(textBox1.Text);

                    Invoke(new Action(() => {
                        // mainForm.label1.Text = "Downloading..."; 
                    }));

                    var tempFile = Path.GetTempFileName();
                    using Stream zipdata = new WebClient().OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.zip");

                    Invoke(new Action(() => {
                        // mainForm.label1.Text = "Extracting...";
                    }));

                    if (File.Exists(Path.Combine(dirpath, "Mono.Cecil.dll")))
                        File.Delete(Path.Combine(dirpath, "Mono.Cecil.dll"));
                    if (File.Exists(Path.Combine(dirpath, "version.dll")))
                        File.Delete(Path.Combine(dirpath, "version.dll"));
                    if (File.Exists(Path.Combine(dirpath, "winmm.dll")))
                        File.Delete(Path.Combine(dirpath, "winmm.dll"));
                    if (Directory.Exists(Path.Combine(dirpath, "Logs")))
                        Directory.Delete(Path.Combine(dirpath, "Logs"), true);
                    if (Directory.Exists(Path.Combine(dirpath, "MelonLoader")))
                        Directory.Delete(Path.Combine(dirpath, "MelonLoader"), true);

                    using var zip = new ZipArchive(zipdata);
                    foreach (var zipArchiveEntry in zip.Entries)
                    {
                        string filepath = Path.Combine(dirpath, zipArchiveEntry.FullName);
                        if (File.Exists(filepath))
                            File.Delete(filepath);
                    }

                    zip.ExtractToDirectory(dirpath);

                    Directory.CreateDirectory(Path.Combine(dirpath, "Logs"));
                    if (!Directory.Exists(Path.Combine(dirpath, "Mods")))
                        Directory.CreateDirectory(Path.Combine(dirpath, "Mods"));
                    if (!Directory.Exists(Path.Combine(dirpath, "PreloadMods")))
                        Directory.CreateDirectory(Path.Combine(dirpath, "PreloadMods"));

                    Close();
                    MessageBox.Show("Installation Successful!", Program.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Close();
                    MessageBox.Show("Installation failed; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Program.Title);
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
    }
}
