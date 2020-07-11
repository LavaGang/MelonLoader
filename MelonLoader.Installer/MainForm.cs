using MelonLoader.AssemblyGenerator;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MetroFramework;

namespace MelonLoader.Installer
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        internal string CurrentVersion = null;
        internal string UnityVersion = null;

        public MainForm()
        {
            InitializeComponent();
            Text = Program.Title;
            cbVersions.SelectedIndex = 0;
        }

        private void MainForm_Load(object sender, EventArgs e) => Program.CheckForUpdates();

        private void btnSelect_Click_1(object sender, EventArgs e)
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

                        tbPath.Text = filePath;
                        btnInstall.Enabled = true;
                        UnityVersion = Program.GetUnityVersion(filePath);

                        string existingFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(filePath), "MelonLoader"), "MelonLoader.ModHandler.dll");
                        if (File.Exists(existingFilePath))
                        {
                            string file_version = FileVersionInfo.GetVersionInfo(existingFilePath).FileVersion;
                            if (file_version.IndexOf(".0") >= 0)
                                file_version = file_version.Substring(0, file_version.IndexOf(".0"));
                            CurrentVersion = "v" + file_version;
                            string selectedVersion = (((cbVersions.SelectedIndex == 0) && (cbVersions.Items.Count > 1)) ? (string)cbVersions.Items[1] : (string)cbVersions.Items[cbVersions.SelectedIndex]);
                            if (CurrentVersion.Equals(selectedVersion))
                                btnInstall.Text = "RE-INSTALL";
                            else
                                btnInstall.Text = "INSTALL";
                        }
                        else
                        {
                            CurrentVersion = null;
                            btnInstall.Text = "INSTALL";
                        }
                    }
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ((e.CloseReason == CloseReason.WindowsShutDown) || (e.CloseReason == CloseReason.UserClosing) || (e.CloseReason == CloseReason.TaskManagerClosing))
            {
                TempFileCache.ClearCache();
                Process.GetCurrentProcess().Kill();
            }
        }


        private void cbVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnInstall.Enabled && !string.IsNullOrEmpty(CurrentVersion))
            {
                string selectedVersion = (((cbVersions.SelectedIndex == 0) && (cbVersions.Items.Count > 1)) ? (string)cbVersions.Items[1] : (string)cbVersions.Items[cbVersions.SelectedIndex]);
                if (CurrentVersion.Equals(selectedVersion))
                    btnInstall.Text = "RE-INSTALL";
                else
                    btnInstall.Text = "INSTALL";
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            cbVersions.Visible = false;
            tbPath.Visible = false;
            btnSelect.Visible = false;
            btnInstall.Visible = false;

            progInstall.Visible = true;
            lblProgressInfo.Visible = true;
            lblProgressPer.Visible = true;

            new Thread(() =>
            {
                try
                {
                    string dirpath = Path.GetDirectoryName(tbPath.Text);
                    string selectedVersion = (((cbVersions.SelectedIndex == 0) && (cbVersions.Items.Count > 1)) ? (string)cbVersions.Items[1] : (string)cbVersions.Items[cbVersions.SelectedIndex]);
                    bool legacy_install = (selectedVersion.Equals("v0.2.1") || selectedVersion.Equals("v0.2") || selectedVersion.Equals("v0.1.0"));

                    Program.Install(dirpath, selectedVersion, legacy_install);

                    Program.SetDisplayText("SUCCESS!");
                    MessageBox.Show("Installation Successful!", Program.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Close();
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    TempFileCache.ClearCache();
                    Program.SetDisplayText("ERROR!");
                    MessageBox.Show("Installation failed; upload the created log to #melonloader-support on discord", Program.Title);
                    File.WriteAllText(Directory.GetCurrentDirectory() + $@"\MLInstaller_{DateTime.Now:yy-M-dd_HH-mm-ss.fff}.log", ex.ToString());
                    Close();
                    Application.Exit();
                }
            }).Start();
        }

    }
}
