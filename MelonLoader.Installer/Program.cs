using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = "MelonLoader Installer";
        internal static MainForm mainForm = null;
        internal static WebClient webClient = new WebClient();

        [STAThread]
        static void Main()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            webClient.Headers.Add("User-Agent", "Unity web player");
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.EnableVisualStyles();

            try
            {
                mainForm.comboBox1.Items.Clear();
                JArray data = (JArray)JsonConvert.DeserializeObject(webClient.DownloadString("https://api.github.com/repos/HerpDerpinstine/MelonLoader/releases"));
                if (data.Count > 0)
                {
                    foreach (var x in data)
                    {
                        string version = x["tag_name"].Value<string>();
                        if (mainForm.comboBox1.Items.Count <= 0)
                            mainForm.comboBox1.Items.Add("Latest (" + version + ")");
                        mainForm.comboBox1.Items.Add(version);
                    }
                }
                if (mainForm.comboBox1.Items.Count <= 0)
                    throw new Exception("Version List is Empty!");
                mainForm.comboBox1.SelectedIndex = 0;
                mainForm.comboBox1.SelectedItem = mainForm.comboBox1.Items[0];
                mainForm.Show();

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Get Version List; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Title);
                Application.Exit();
            }
        }

        internal static void Install(string dirpath, string selectedVersion, bool legacy_install)
        {
            if (!legacy_install)
                Install_Normal(dirpath, selectedVersion);
            else
                Install_Legacy(dirpath, selectedVersion);
        }

        private static void Install_Legacy(string dirpath, string selectedVersion)
        {
            if (selectedVersion.Equals("v0.1.0"))
                Install_Legacy_01(dirpath);
            else
                Install_Legacy_02(dirpath, selectedVersion);
        }

        private static void SetDisplayText(string text)
        {
            mainForm.Invoke(new Action(() => {
                mainForm.label2.Text = text; 
            }));
        }

        private static void SetPercentage(int percent)
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
            string maindir_path = Path.Combine(dirpath, "MelonLoader");
            if (legacy_install || string.IsNullOrEmpty(mainForm.CurrentVersion) || mainForm.CurrentVersion.Equals("v0.2.1") || mainForm.CurrentVersion.Equals("v0.2") || mainForm.CurrentVersion.Equals("v0.1.0"))
            {
                if (Directory.Exists(maindir_path))
                    Directory.Delete(maindir_path, true);
            }
            else
            {
                if (File.Exists(Path.Combine(maindir_path, "MelonLoader.dll")))
                    File.Delete(Path.Combine(maindir_path, "MelonLoader.dll"));
                if (File.Exists(Path.Combine(maindir_path, "MelonLoader.ModHandler.dll")))
                    File.Delete(Path.Combine(maindir_path, "MelonLoader.ModHandler.dll"));
                string depsdir_path = Path.Combine(maindir_path, "Dependencies");
                string[] files = Directory.GetFiles(depsdir_path, "*.dll", SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];
                        if (!string.IsNullOrEmpty(file))
                            File.Delete(file);
                    }
                }
                if (Directory.Exists(Path.Combine(depsdir_path, "SupportModules")))
                    Directory.Delete(Path.Combine(depsdir_path, "SupportModules"), true);
                string assemblygendir_path = Path.Combine(depsdir_path, "AssemblyGenerator");
                if (File.Exists(Path.Combine(assemblygendir_path, "MelonLoader.AssemblyGenerator.exe")))
                    File.Delete(Path.Combine(assemblygendir_path, "MelonLoader.AssemblyGenerator.exe"));
                if (Directory.Exists(Path.Combine(assemblygendir_path, "Il2CppDumper")))
                    Directory.Delete(Path.Combine(assemblygendir_path, "Il2CppDumper"), true);
                if (Directory.Exists(Path.Combine(assemblygendir_path, "Il2CppAssemblyUnhollower")))
                    Directory.Delete(Path.Combine(assemblygendir_path, "Il2CppAssemblyUnhollower"), true);
            }
        }

        private static void CreateDirectories(string dirpath, bool legacy_install)
        {
            Directory.CreateDirectory(Path.Combine(dirpath, "Logs"));
            if (!Directory.Exists(Path.Combine(dirpath, "Mods")))
                Directory.CreateDirectory(Path.Combine(dirpath, "Mods"));

            if (!legacy_install && !Directory.Exists(Path.Combine(dirpath, "PreloadMods")))
                Directory.CreateDirectory(Path.Combine(dirpath, "PreloadMods"));
        }

        private static void ExtractZip(string dirpath, Stream zipData)
        {
            using var zip = new ZipArchive(zipData);
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
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader.zip");
            SetDisplayText("Extracting MelonLoader...");
            SetPercentage(50);
            Cleanup(dirpath, false);
            ExtractZip(dirpath, zipdata);
            CreateDirectories(dirpath, false);
        }

        private static void Install_Legacy_02(string dirpath, string selectedVersion)
        {
            SetDisplayText("Downloading MelonLoader...");
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader" + (selectedVersion.Equals("v0.2") ? "_" : ".") + (File.Exists(Path.Combine(dirpath, "GameAssembly.dll")) ? "Il2Cpp" : "Mono") + ".zip");
            SetDisplayText("Extracting MelonLoader...");
            SetPercentage(50);
            Cleanup(dirpath, true);
            ExtractZip(dirpath, zipdata);
            CreateDirectories(dirpath, true);
        }

        private static void Install_Legacy_01(string dirpath)
        {
            SetDisplayText("Downloading MelonLoader...");
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MelonLoader.zip");

            SetDisplayText("Downloading Dependencies...");
            SetPercentage(25);
            using Stream zipdata2 = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MonoDependencies.zip");

            SetDisplayText("Extracting MelonLoader...");
            SetPercentage(50);
            Cleanup(dirpath, true);
            ExtractZip(dirpath, zipdata);

            SetDisplayText("Extracting Dependencies...");
            SetPercentage(75);
            ExtractZip(dirpath, zipdata2);

            CreateDirectories(dirpath, true);
        }

        internal static string GetUnityFileVersion(string exepath)
        {
            string file_version = FileVersionInfo.GetVersionInfo(exepath).FileVersion;
            return file_version.Substring(0, file_version.LastIndexOf('.'));
        }
    }
}