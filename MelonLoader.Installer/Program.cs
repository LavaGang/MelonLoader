using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Net;
using System.Text;
using TinyJSON;

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
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.EnableVisualStyles();

            try
            {
                //mainForm.comboBox1.Items.Clear();

                // Get Version List from GitHub
                // https://api.github.com/repos/HerpDerpinstine/MelonLoader/releases

                if (mainForm.comboBox1.Items.Count <= 0)
                    throw new Exception("Version List is Empty!");

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

        private static void Install_SetDisplayText(string text)
        {
            mainForm.Invoke(new Action(() => {
                mainForm.button2.Text = text; 
            }));
        }

        private static void Install_SetPercentage(float percent)
        {
            mainForm.Invoke(new Action(() => {

            }));
        }

        private static void Install_Cleanup(string dirpath)
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

        private static void Install_CreateDirectories(string dirpath, bool legacy_install)
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
            Install_SetDisplayText("Downloading MelonLoader...");
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader.zip");
            Install_SetDisplayText("Extracting MelonLoader...");
            Install_SetPercentage(50f);
            Install_Cleanup(dirpath);
            ExtractZip(dirpath, zipdata);
            Install_CreateDirectories(dirpath, false);
        }

        private static void Install_Legacy_02(string dirpath, string selectedVersion)
        {
            Install_SetDisplayText("Downloading MelonLoader...");
            bool is_il2cpp = File.Exists(Path.Combine(dirpath, "GameAssembly.dll"));
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/" + selectedVersion + "/MelonLoader" + (selectedVersion.Equals("v0.2") ? "_" : ".") + (is_il2cpp ? "Il2Cpp" : "Mono") + ".zip");
            Install_SetDisplayText("Extracting MelonLoader...");
            Install_SetPercentage(50f);
            Install_Cleanup(dirpath);
            ExtractZip(dirpath, zipdata);
            Install_CreateDirectories(dirpath, true);
        }

        private static void Install_Legacy_01(string dirpath)
        {
            Install_SetDisplayText("Downloading MelonLoader...");
            using Stream zipdata = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MelonLoader.zip");

            Install_SetDisplayText("Downloading Mono Dependencies...");
            Install_SetPercentage(25f);
            using Stream zipdata2 = webClient.OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v0.1.0/MonoDependencies.zip");

            Install_SetDisplayText("Extracting MelonLoader...");
            Install_SetPercentage(50f);
            Install_Cleanup(dirpath);
            ExtractZip(dirpath, zipdata);

            Install_SetDisplayText("Extracting Mono Dependencies...");
            Install_SetPercentage(75f);
            ExtractZip(dirpath, zipdata2);

            Install_CreateDirectories(dirpath, true);
        }

        internal static string GetUnityFileVersion(string exepath)
        {
            string file_version = FileVersionInfo.GetVersionInfo(exepath).FileVersion;
            return file_version.Substring(0, file_version.LastIndexOf('.'));
        }
    }
}