using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using LightJson;
using MelonLoader.AssemblyGenerator;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = "MelonLoader Installer";
        private static string Version = "1.0.4";
        internal static MainForm mainForm = null;
        internal static WebClient webClient = new WebClient();

        [STAThread]
        static void Main()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            webClient.Headers.Add("User-Agent", "Unity web player");
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            //ParseCommandLine();
            Install_GUI();
        }

        //private static void ParseCommandLine()
        //{

        //}

        static void Install_GUI()
        {
            try
            {
                mainForm = new MainForm();
                mainForm.comboBox1.Items.Clear();
                JsonArray data = (JsonArray)JsonValue.Parse(webClient.DownloadString("https://api.github.com/repos/HerpDerpinstine/MelonLoader/releases")).AsJsonArray;
                if (data.Count > 0)
                {
                    foreach (var x in data)
                    {
                        string version = x["tag_name"].AsString;
                        if (mainForm.comboBox1.Items.Count <= 0)
                            mainForm.comboBox1.Items.Add("Latest (" + version + ")");
                        mainForm.comboBox1.Items.Add(version);
                    }
                }
                if (mainForm.comboBox1.Items.Count <= 0)
                    throw new Exception("Version List is Empty!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Get Version List; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Title);
                Application.Exit();
            }

            mainForm.comboBox1.SelectedIndex = 0;
            mainForm.comboBox1.SelectedItem = mainForm.comboBox1.Items[0];
            mainForm.Show();
            Application.Run(mainForm);
        }

        internal static void Install(string dirpath, string selectedVersion, bool legacy_install)
        {
            if (!legacy_install)
                Install_Normal(dirpath, selectedVersion);
            else
                Install_Legacy(dirpath, selectedVersion);
            TempFileCache.ClearCache();
        }

        private static void Install_Legacy(string dirpath, string selectedVersion)
        {
            if (selectedVersion.Equals("v0.1.0"))
                Install_Legacy_01(dirpath);
            else
                Install_Legacy_02(dirpath, selectedVersion);
        }

        internal static void SetDisplayText(string text)
        {
            mainForm.Invoke(new Action(() => {
                mainForm.label2.Text = text; 
            }));
        }

        internal static void SetPercentage(int percent)
        {
            mainForm.Invoke(new Action(() => {
                mainForm.progressBar1.Value = percent;
                mainForm.label3.Text = percent.ToString() + "%";
            }));
        }

        private static void Cleanup(string dirpath, bool legacy_install)
        {
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
        }

        private static void CreateDirectories(string dirpath, string selectedVersion, bool legacy_install)
        {
            Directory.CreateDirectory(Path.Combine(dirpath, "Logs"));
            if (!Directory.Exists(Path.Combine(dirpath, "Mods")))
                Directory.CreateDirectory(Path.Combine(dirpath, "Mods"));
            if (!legacy_install && !selectedVersion.Equals("v0.2.2") && !Directory.Exists(Path.Combine(dirpath, "Plugins")))
                Directory.CreateDirectory(Path.Combine(dirpath, "Plugins"));
        }

        private static void ExtractZip(string dirpath, string tempFile)
        {
            using var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
            using var zip = new ZipArchive(stream);
            foreach (var zipArchiveEntry in zip.Entries)
            {
                string filepath = Path.Combine(dirpath, zipArchiveEntry.FullName);
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
            zip.ExtractToDirectory(dirpath);
        }

        private static void Install_Normal(string dirpath, string selectedVersion)
        {
            SetDisplayText("Downloading MelonLoader...");
            string tempfilepath = TempFileCache.CreateFile();
            webClient.DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader.zip", tempfilepath);
            SetDisplayText("Extracting MelonLoader...");
            SetPercentage(50);
            Cleanup(dirpath, false);
            ExtractZip(dirpath, tempfilepath);
            CreateDirectories(dirpath, selectedVersion, false);
        }

        private static void Install_Legacy_02(string dirpath, string selectedVersion)
        {
            SetDisplayText("Downloading MelonLoader...");
            bool is_02 = selectedVersion.Equals("v0.2");
            string tempfilepath = TempFileCache.CreateFile();
            webClient.DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader" + (is_02 ? "_" : ".") + (File.Exists(Path.Combine(dirpath, "GameAssembly.dll")) ? "Il2Cpp" : "Mono") + ".zip", tempfilepath);
            
            SetDisplayText("Extracting MelonLoader...");
            if (is_02)
                SetPercentage(20);
            else
                SetPercentage(50);
            Cleanup(dirpath, true);
            ExtractZip(dirpath, tempfilepath);

            if (is_02)
            {
                string AssemblyGenerator_Folder = Path.Combine(Path.Combine(dirpath, "MelonLoader"), "AssemblyGenerator");
                string Il2CppDumper_Folder = Path.Combine(AssemblyGenerator_Folder, "Il2CppDumper");
                string Il2CppAssemblyUnhollower_Folder = Path.Combine(AssemblyGenerator_Folder, "Il2CppAssemblyUnhollower");
                string UnityDependencies_Folder = Path.Combine(Il2CppAssemblyUnhollower_Folder, "UnityDependencies");

                SetDisplayText("Downloading Il2CppDumper...");
                SetPercentage(40);
                string tempfilepath2 = TempFileCache.CreateFile();
                webClient.DownloadFile("https://github.com/Perfare/Il2CppDumper/releases/download/v6.2.1/Il2CppDumper-v6.2.1.zip", tempfilepath2);

                SetDisplayText("Downloading Il2CppUnhollower...");
                SetPercentage(50);
                string tempfilepath3 = TempFileCache.CreateFile();
                webClient.DownloadFile("https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.4.3.0/Il2CppAssemblyUnhollower.0.4.3.0.zip", tempfilepath3);

                SetDisplayText("Downloading Dependencies...");
                SetPercentage(60);
                string tempfilepath4 = TempFileCache.CreateFile();
                webClient.DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/raw/master/BaseLibs/UnityDependencies/" + mainForm.UnityVersion + ".zip", tempfilepath4);

                SetDisplayText("Extracting Il2CppDumper...");
                SetPercentage(70);
                ExtractZip(Il2CppDumper_Folder, tempfilepath2);

                SetDisplayText("Extracting Il2CppUnhollower...");
                SetPercentage(80);
                ExtractZip(Il2CppAssemblyUnhollower_Folder, tempfilepath3);

                SetDisplayText("Extracting Dependencies...");
                SetPercentage(90);
                ExtractZip(UnityDependencies_Folder, tempfilepath4);
            }

            CreateDirectories(dirpath, selectedVersion, true);
        }

        private static void Install_Legacy_01(string dirpath)
        {
            SetDisplayText("Downloading MelonLoader...");
            string tempfilepath = TempFileCache.CreateFile();
            webClient.DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MelonLoader.zip", tempfilepath);

            SetDisplayText("Downloading Dependencies...");
            SetPercentage(48);
            string tempfilepath2 = TempFileCache.CreateFile();
            webClient.DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MonoDependencies.zip", tempfilepath2);

            SetDisplayText("Extracting MelonLoader...");
            SetPercentage(64);
            Cleanup(dirpath, true);
            ExtractZip(dirpath, tempfilepath);

            SetDisplayText("Extracting Dependencies...");
            SetPercentage(80);
            ExtractZip(dirpath, tempfilepath2);

            CreateDirectories(dirpath, "v0.1.0", true);
        }

        internal static string GetUnityFileVersion(string exepath)
        {
            string file_version = FileVersionInfo.GetVersionInfo(exepath).FileVersion;
            return file_version.Substring(0, file_version.LastIndexOf('.'));
        }

        internal static void CheckForUpdates()
        {
            new Thread(() =>
            {
                try
                {
                    string response = webClient.DownloadString("https://github.com/HerpDerpinstine/MelonLoader/raw/master/MelonLoader.Installer/version.txt");
                    if (string.IsNullOrEmpty(response))
                        return;
                    if (!Version.Equals(response))
                    {
                        DialogResult msgresult = MessageBox.Show("A New Version of the Installer is Available!", Program.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (msgresult == DialogResult.OK)
                            Process.Start("https://github.com/HerpDerpinstine/MelonLoader/releases/latest");
                    }
                }
                catch (Exception e) { }
            }).Start();
        }
    }
}