using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MelonLoader.LightJson;
#pragma warning disable 0168

namespace MelonLoader
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        private int GameArch = 0;
        internal Version CurrentInstalledVersion = null;

        public MainForm()
        {
            InitializeComponent();
            InstallerVersion.Text = "Installer v" + BuildInfo.Version;
            PageManager.SelectedIndex = 0;
            Automated_Arch_Selection.SelectedIndex = 0;
            Settings_Theme_Selection.SelectedIndex = Config.Theme;
            Settings_AutoUpdateInstaller.Checked = Config.AutoUpdateInstaller;
            Settings_CloseAfterCompletion.Checked = Config.CloseAfterCompletion;
            PageManager.Controls.Clear();
            PageManager.Controls.Add(Tab_PleaseWait);
            Automated_Install.Size = new Size(419, 44);
            ManualZip_Install.Size = new Size(419, 44);
            Tab_Error.Text = Tab_Automated.Text;
            Output_Current_Progress_Display.Value = 0;
            Output_Current_Progress_Text.Text = "0";
            Output_Total_Progress_Display.Value = 0;
            Output_Total_Progress_Text.Text = "0";
        }

        private void SelectUnityGame()
        {
            // Add Shortcut Support

            using (OpenFileDialog opd = new OpenFileDialog())
            {
                opd.Filter = "Unity Game (*.exe)|*.exe";
                opd.RestoreDirectory = true;
                opd.Multiselect = false;
                if ((opd.ShowDialog() != DialogResult.OK)
                    || string.IsNullOrEmpty(opd.FileName))
                    return;
                Automated_UnityGame_Display.Text = opd.FileName;
                ManualZip_UnityGame_Display.Text = opd.FileName;
                Automated_Install.Enabled = true;
                CheckUnityGame();
            }
        }

        private void SelectZipArchive()
        {
            using (OpenFileDialog opd = new OpenFileDialog())
            {
                opd.Filter = "MelonLoader Zip Archive (*.zip)|*.zip";
                opd.RestoreDirectory = true;
                opd.Multiselect = false;
                if ((opd.ShowDialog() != DialogResult.OK)
                    || string.IsNullOrEmpty(opd.FileName))
                    return;
                ManualZip_ZipArchive_Display.Text = opd.FileName;
                if (!string.IsNullOrEmpty(ManualZip_UnityGame_Display.Text)
                && !ManualZip_UnityGame_Display.Text.Equals("Please Select your Unity Game..."))
                    ManualZip_Install.Enabled = true;
            }
        }

        private void CheckUnityGame()
        {
            if (string.IsNullOrEmpty(Automated_UnityGame_Display.Text) || Automated_UnityGame_Display.Text.Equals("Please Select your Unity Game..."))
                return;
            byte[] filedata = File.ReadAllBytes(Automated_UnityGame_Display.Text);
            if ((filedata == null)
                || (filedata.Length <= 0)
                || (BitConverter.ToUInt16(filedata, (BitConverter.ToInt32(filedata, 60) + 4)) != 34404))
                GameArch = 0;
            else
                GameArch = 1;
            if (!string.IsNullOrEmpty(ManualZip_ZipArchive_Display.Text)
                && !ManualZip_ZipArchive_Display.Text.Equals("Please Select your MelonLoader Zip Archive..."))
                ManualZip_Install.Enabled = true;
            if (Automated_Arch_AutoDetect.Checked)
            {
                Automated_Arch_Selection.SelectedIndex = GameArch;
                Automated_Arch_Selection.Select();
            }
            string folder_path = Path.Combine(Path.GetDirectoryName(Automated_UnityGame_Display.Text), "MelonLoader");
            string legacy_file_path = Path.Combine(folder_path, "MelonLoader.ModHandler.dll");
            string file_path = Path.Combine(folder_path, "MelonLoader.dll");
            if (File.Exists(legacy_file_path) || File.Exists(file_path))
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo((File.Exists(legacy_file_path) ? legacy_file_path : file_path));
                CurrentInstalledVersion = new Version(fileVersionInfo.ProductVersion);
                Automated_Install.Size = new Size(206, 44);
                ManualZip_Install.Size = new Size(206, 44);
                Automated_Uninstall.Visible = true;
                ManualZip_Uninstall.Visible = true;
                ManualZip_Install.Text = "RE-INSTALL";
                Version selected_ver = new Version(Automated_Version_Selection.Text.Substring(1));
                int compare_ver = selected_ver.CompareTo(CurrentInstalledVersion);
                if (compare_ver < 0)
                    Automated_Install.Text = "DOWNGRADE";
                else if (compare_ver > 0)
                    Automated_Install.Text = "UPDATE";
                else
                    Automated_Install.Text = "RE-INSTALL";
            }
            else
            {
                ManualZip_Install.Text = "INSTALL";
                Automated_Install.Text = "INSTALL";
                Automated_Install.Size = new Size(419, 44);
                ManualZip_Install.Size = new Size(419, 44);
                Automated_Uninstall.Visible = false;
                ManualZip_Uninstall.Visible = false;
            }
        }

        private void CheckForInstallerUpdate()
        {
            string response = null;
            try { response = Program.webClient_update.DownloadString(Program.Repo_API_Installer); } catch (Exception ex) { response = null; }
            if (string.IsNullOrEmpty(response))
            {
                GetReleases();
                return;
            }
            JsonArray data = JsonValue.Parse(response).AsJsonArray;
            if (data.Count <= 0)
            {
                GetReleases();
                return;
            }
            JsonValue release = data[0];
            JsonArray assets = release["assets"].AsJsonArray;
            if (assets.Count <= 0)
            {
                GetReleases();
                return;
            }
            string version = release["tag_name"].AsString;
            if (version.Equals(BuildInfo.Version))
            {
                GetReleases();
                return;
            }
            Invoke(new Action(() => { InstallerUpdateNotice.Visible = true; }));
            if (!Config.AutoUpdateInstaller)
            {
                GetReleases();
                return;
            }
            OperationHandler.CurrentOperation = OperationHandler.Operations.INSTALLER_UPDATE;
            Invoke(new Action(() => {
                Tab_PleaseWait.Text = "UPDATE   ";
                PleaseWait_Text.Text = "Downloading Update...";
            }));
            string downloadurl = assets[0]["browser_download_url"].AsString;
            string temp_path = TempFileCache.CreateFile();
            try { Program.webClient.DownloadFileAsync(new Uri(downloadurl), temp_path); while (Program.webClient.IsBusy) { } }
            catch (Exception ex)
            {
                TempFileCache.ClearCache();
                GetReleases();
                return;
            }
            if (Program.Closing)
                return;

            // Get SHA512 Hash from Repo

            // Get SHA512 Hash from Downloaded File

            // Compare

            string exe_path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string tmp_file_path = Path.Combine(exe_path, (Path.GetFileNameWithoutExtension(downloadurl) + ".tmp.exe"));
            if (File.Exists(tmp_file_path))
                File.Delete(tmp_file_path);
            File.Move(temp_path, tmp_file_path);
            Process.Start(tmp_file_path);
            Process.GetCurrentProcess().Kill();
        }

        private void GetReleases()
        {
            Invoke(new Action(() => {
                Tab_PleaseWait.Text = Tab_Automated.Text;
                PleaseWait_Text.Text = "Getting List of Releases from GitHub...";
                PleaseWait_Text.Location = new Point(105, 79);
                PleaseWait_Text.Size = new Size(250, 22);
                int current_index = PageManager.SelectedIndex;
                PageManager.Controls.Clear();
                PageManager.Controls.Add(Tab_PleaseWait);
                PageManager.Controls.Add(Tab_ManualZip);
                PageManager.Controls.Add(Tab_Settings);
                PageManager.SelectedIndex = current_index;
                PageManager.Cursor = Cursors.Hand;
            }));
            ParseReleasesURL();
            Invoke(new Action(() =>
            {
                int current_index = PageManager.SelectedIndex;
                PageManager.Controls.Clear();
                if (Automated_Version_Selection.Items.Count <= 0)
                    PageManager.Controls.Add(Tab_Error);
                else
                {
                    PageManager.Controls.Add(Tab_Automated);
                    Automated_Version_Selection.SelectedIndex = 0;
                }
                PageManager.Controls.Add(Tab_ManualZip);
                PageManager.Controls.Add(Tab_Settings);
                PageManager.SelectedIndex = current_index;
            }));
        }

        private void ThemeChanged(object sender, EventArgs e)
        {
            bool lightmode = (Settings_Theme_Selection.SelectedIndex == 1);
            Config.Theme = (lightmode ? 1 : 0);
            MetroFramework.MetroThemeStyle themeStyle = (lightmode ? MetroFramework.MetroThemeStyle.Light : MetroFramework.MetroThemeStyle.Dark);
            StyleManager.Style = (lightmode ? MetroFramework.MetroColorStyle.Green : MetroFramework.MetroColorStyle.Red);
            StyleManager.Theme = themeStyle;
            Theme = themeStyle;
            Style = (lightmode ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green);
            Link_Discord.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            Link_Twitter.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            Link_Wiki.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            Link_GitHub.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            Link_GitHub.Image = (lightmode ? Properties.Resources.GitHub_Light : Properties.Resources.GitHub_Dark);
            ML_Logo.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            ML_Text.BackColor = (lightmode ? Color.White : Color.FromArgb(17, 17, 17));
            Settings_Theme_Text.Theme = themeStyle;
            Settings_Theme_Selection.Theme = themeStyle;
            InstallerVersion.Theme = themeStyle;
            PageManager.Style = (lightmode ? MetroFramework.MetroColorStyle.Green : MetroFramework.MetroColorStyle.Red);
            PageManager.Theme = themeStyle;
            Tab_Automated.Theme = themeStyle;
            Tab_ManualZip.Theme = themeStyle;
            Tab_Settings.Theme = themeStyle;
            Tab_Error.Theme = themeStyle;
            Settings_AutoUpdateInstaller.Style = (lightmode ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green);
            Settings_AutoUpdateInstaller.Theme = themeStyle;
            Settings_CloseAfterCompletion.Style = (lightmode ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green);
            Settings_CloseAfterCompletion.Theme = themeStyle;
            Error_Error.Theme = themeStyle;
            Error_Text.Theme = themeStyle;
            Automated_UnityGame_Text.Theme = themeStyle;
            Automated_UnityGame_Select.Theme = themeStyle;
            Automated_UnityGame_Display.BackColor = (lightmode ? Color.White : Color.FromArgb(34, 34, 34));
            Automated_UnityGame_Display.ForeColor = (lightmode ? Color.Black : Color.FromArgb(204, 204, 204));
            Automated_Version_Text.Theme = themeStyle;
            Automated_Version_Selection.Theme = themeStyle;
            Automated_Version_Latest.Style = (lightmode ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green);
            Automated_Version_Latest.Theme = themeStyle;
            Automated_Arch_Text.Theme = themeStyle;
            Automated_Arch_Selection.Theme = themeStyle;
            Automated_Arch_AutoDetect.Style = (lightmode ? MetroFramework.MetroColorStyle.Red : MetroFramework.MetroColorStyle.Green);
            Automated_Arch_AutoDetect.Theme = themeStyle;
            Automated_Divider.Theme = themeStyle;
            ManualZip_Divider.Theme = themeStyle;
            Automated_Install.Theme = themeStyle;
            Automated_Uninstall.Theme = themeStyle;
            Tab_Output.Theme = themeStyle;
            ManualZip_Install.Theme = themeStyle;
            ManualZip_Uninstall.Theme = themeStyle;
            ManualZip_UnityGame_Text.Theme = themeStyle;
            ManualZip_UnityGame_Select.Theme = themeStyle;
            ManualZip_UnityGame_Display.BackColor = (lightmode ? Color.White : Color.FromArgb(34, 34, 34));
            ManualZip_UnityGame_Display.ForeColor = (lightmode ? Color.Black : Color.FromArgb(204, 204, 204));
            ManualZip_ZipArchive_Text.Theme = themeStyle;
            ManualZip_ZipArchive_Select.Theme = themeStyle;
            ManualZip_ZipArchive_Display.BackColor = (lightmode ? Color.White : Color.FromArgb(34, 34, 34));
            ManualZip_ZipArchive_Display.ForeColor = (lightmode ? Color.Black : Color.FromArgb(204, 204, 204));
            InstallerUpdateNotice.Theme = themeStyle;
            InstallerUpdateNotice.ForeColor = (lightmode ? Color.Red : Color.Green);
            Output_Divider.Theme = themeStyle;
            Output_Current_Text.Theme = themeStyle;
            Output_Current_Operation.Theme = themeStyle;
            Output_Current_Progress_Display.Theme = themeStyle;
            Output_Current_Progress_Text.Theme = themeStyle;
            Output_Current_Progress_Text_Label.Theme = themeStyle;
            Output_Total_Text.Theme = themeStyle;
            Output_Total_Progress_Display.Theme = themeStyle;
            Output_Total_Progress_Text.Theme = themeStyle;
            Output_Total_Progress_Text_Label.Theme = themeStyle;
            Tab_PleaseWait.Theme = themeStyle;
            PleaseWait_PleaseWait.Theme = themeStyle;
            PleaseWait_Text.Theme = themeStyle;
            Automated_x64Only.Theme = themeStyle;
        }

        private void ParseReleasesURL()
        {
            string response = null;
            try { response = Program.webClient.DownloadString(Program.Repo_API_MelonLoader); } catch (Exception ex) { response = null; }
            if (string.IsNullOrEmpty(response))
                return;
            JsonArray data = JsonValue.Parse(response).AsJsonArray;
            if (data.Count <= 0)
                return;
            Invoke(new Action(() => Automated_Version_Selection.Items.Clear()));
            foreach (JsonValue release in data)
            {
                JsonArray assets = release["assets"].AsJsonArray;
                if (assets.Count <= 0)
                    continue;
                string version = release["tag_name"].AsString;
                Invoke(new Action(() => Automated_Version_Selection.Items.Add(version)));
            }
        }

        private void Automated_Version_Latest_CheckedChanged(object sender, EventArgs e)
        {
            Automated_Version_Selection.Enabled = !Automated_Version_Latest.Checked;
            if (Automated_Version_Selection.Enabled || (Automated_Version_Selection.Items.Count <= 0))
                return;
            Automated_Version_Selection.SelectedIndex = 0;
            Automated_Version_Selection.Select();
        }

        private void Automated_Version_Selection_SelectedValueChanged(object sender, EventArgs e)
        {
            bool legacy_version = (Automated_Version_Selection.Text.Equals("v0.2") || Automated_Version_Selection.Text.StartsWith("v0.2.") || Automated_Version_Selection.Text.Equals("v0.1.0"));
            Automated_x64Only.Visible = legacy_version;
            Automated_Arch_Selection.Visible = !legacy_version;
            Automated_Arch_AutoDetect.Visible = !legacy_version;
            if ((CurrentInstalledVersion == null) || string.IsNullOrEmpty(Automated_Version_Selection.Text))
                Automated_Install.Text = "INSTALL";
            else
            {
                Version selected_ver = new Version(Automated_Version_Selection.Text.Substring(1));
                int compare_ver = selected_ver.CompareTo(CurrentInstalledVersion);
                if (compare_ver < 0)
                    Automated_Install.Text = "DOWNGRADE";
                else if (compare_ver > 0)
                    Automated_Install.Text = "UPDATE";
                else
                    Automated_Install.Text = "RE-INSTALL";
            }
        }

        private void Automated_Arch_AutoDetect_CheckedChanged(object sender, EventArgs e)
        {
            Automated_Arch_Selection.Enabled = !Automated_Arch_AutoDetect.Checked;
            if (Automated_Arch_Selection.Enabled)
                return;
            Automated_Arch_Selection.SelectedIndex = GameArch;
            Automated_Arch_Selection.Select();
        }

        private void Automated_Install_Click(object sender, EventArgs e)
        {
            if ((CurrentInstalledVersion == null) || string.IsNullOrEmpty(Automated_Version_Selection.Text))
            {
                OperationHandler.CurrentOperation = OperationHandler.Operations.INSTALL;
                Tab_Output.Text = "INSTALL  ";
            }
            else
            {
                Version selected_ver = new Version(Automated_Version_Selection.Text.Substring(1));
                int compare_ver = selected_ver.CompareTo(CurrentInstalledVersion);
                if (compare_ver < 0)
                {
                    OperationHandler.CurrentOperation = OperationHandler.Operations.DOWNGRADE;
                    Tab_Output.Text = "DOWNGRADE   ";
                }
                else if (compare_ver > 0)
                {
                    OperationHandler.CurrentOperation = OperationHandler.Operations.UPDATE;
                    Tab_Output.Text = "UPDATE   ";
                }
                else
                {
                    OperationHandler.CurrentOperation = OperationHandler.Operations.REINSTALL;
                    Tab_Output.Text = "RE-INSTALL   ";
                }
            }
            bool legacy_version = (Automated_Version_Selection.Text.Equals("v0.2") || Automated_Version_Selection.Text.StartsWith("v0.2.") || Automated_Version_Selection.Text.Equals("v0.1.0"));
            new Thread(() => { OperationHandler.Automated_Install(Path.GetDirectoryName(Automated_UnityGame_Display.Text), Automated_Version_Selection.Text, (legacy_version ? false : (Automated_Arch_Selection.SelectedIndex == 0)), legacy_version); }).Start();
            Program.SetTotalPercentage(0);
            PageManager.Cursor = Cursors.Default;
            TabBeforeOperation = PageManager.SelectedIndex;
            PageManager.Controls.Clear();
            PageManager.Controls.Add(Tab_Output);
        }

        private void ManualZip_Install_Click(object sender, EventArgs e)
        {
            if (CurrentInstalledVersion == null)
            {
                OperationHandler.CurrentOperation = OperationHandler.Operations.INSTALL;
                Tab_Output.Text = "INSTALL  ";
            }
            else
            {
                OperationHandler.CurrentOperation = OperationHandler.Operations.REINSTALL;
                Tab_Output.Text = "RE-INSTALL   ";
            }
            new Thread(() => { OperationHandler.ManualZip_Install(ManualZip_ZipArchive_Display.Text, Path.GetDirectoryName(Automated_UnityGame_Display.Text)); }).Start();
            Program.SetTotalPercentage(0);
            PageManager.Cursor = Cursors.Default;
            TabBeforeOperation = PageManager.SelectedIndex;
            PageManager.Controls.Clear();
            PageManager.Controls.Add(Tab_Output);
        }

        private void ClickedUninstall()
        {
            DialogResult result = MessageBox.Show("Are you sure you wish to Uninstall MelonLoader?", "MelonLoader Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            OperationHandler.CurrentOperation = OperationHandler.Operations.UNINSTALL;
            Tab_Output.Text = "UN-INSTALL   ";
            new Thread(() => { OperationHandler.Uninstall(Path.GetDirectoryName(Automated_UnityGame_Display.Text)); }).Start();
            Program.SetTotalPercentage(0);
            PageManager.Cursor = Cursors.Default;
            TabBeforeOperation = PageManager.SelectedIndex;
            PageManager.Controls.Clear();
            PageManager.Controls.Add(Tab_Output);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Closing = true;
            if ((Program.webClient != null) && Program.webClient.IsBusy)
                Program.webClient.CancelAsync();
            if ((Program.webClient_update != null) && Program.webClient_update.IsBusy)
                Program.webClient_update.CancelAsync();
            if (OperationHandler.CurrentOperation != OperationHandler.Operations.NONE)
                Thread.Sleep(1000);
            TempFileCache.ClearCache();
            Program.OperationError();
            if (OperationHandler.CurrentOperation <= OperationHandler.Operations.INSTALLER_UPDATE)
                return;

            // Sanatize Operation

            MessageBox.Show((OperationHandler.CurrentOperationName + " was Cancelled!"), "MelonLoader Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private int TabBeforeOperation = 0;
        internal void BackToHome()
        {
            CheckUnityGame();
            PageManager.Controls.Clear();
            PageManager.Controls.Add(Tab_Automated);
            PageManager.Controls.Add(Tab_ManualZip);
            PageManager.Controls.Add(Tab_Settings);
            PageManager.Cursor = Cursors.Hand;
            PageManager.SelectedIndex = TabBeforeOperation;
            PageManager.Select();
            Program.SetTotalPercentage(0);
            OperationHandler.CurrentOperation = OperationHandler.Operations.NONE;
        }

        private void Main_Load(object sender, EventArgs e) => new Thread(CheckForInstallerUpdate).Start();
        private void Error_Retry_Click(object sender, EventArgs e) => new Thread(GetReleases).Start();
        private void Link_Discord_Click(object sender, EventArgs e) => Process.Start(Program.Link_Discord);
        private void Link_Twitter_Click(object sender, EventArgs e) => Process.Start(Program.Link_Twitter);
        private void Link_GitHub_Click(object sender, EventArgs e) => Process.Start(Program.Link_GitHub);
        private void Link_Wiki_Click(object sender, EventArgs e) => Process.Start(Program.Link_Wiki);
        private void InstallerUpdateNotice_Click(object sender, EventArgs e) => Process.Start(Program.Link_Update);
        private void Settings_AutoUpdateInstaller_CheckedChanged(object sender, EventArgs e) => Config.AutoUpdateInstaller = Settings_AutoUpdateInstaller.Checked;
        private void Settings_CloseAfterCompletion_CheckedChanged(object sender, EventArgs e) => Config.CloseAfterCompletion = Settings_CloseAfterCompletion.Checked;
        private void ManualZip_UnityGame_Select_Click(object sender, EventArgs e) => SelectUnityGame();
        private void Automated_UnityGame_Select_Click(object sender, EventArgs e) => SelectUnityGame();
        private void Automated_Uninstall_Click(object sender, EventArgs e) => ClickedUninstall();
        private void ManualZip_Uninstall_Click(object sender, EventArgs e) => ClickedUninstall();
        private void ManualZip_ZipArchive_Select_Click(object sender, EventArgs e) => SelectZipArchive();
    }
}