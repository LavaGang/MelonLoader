using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = "MelonLoader Installer";

        [STAThread]
        static void Main()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            Application.EnableVisualStyles();
            mainForm.Show();

            string filePath = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Unity Game (*.exe)|*.exe";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                var processThread = new Thread(() =>
                {
                    try
                    {
                        string dirpath = Path.GetDirectoryName(filePath);

                        mainForm.Invoke(new Action(() => { mainForm.label1.Text = "Downloading..."; }));
                        var tempFile = Path.GetTempFileName();
                        using Stream zipdata = new WebClient().OpenRead("https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.zip");

                        mainForm.Invoke(new Action(() => { mainForm.label1.Text = "Extracting..."; }));
                        if (File.Exists(Path.Combine(dirpath, "Mono.Cecil.dll")))
                            File.Delete(Path.Combine(dirpath, "Mono.Cecil.dll"));
                        if (File.Exists(Path.Combine(dirpath, "version.dll")))
                            File.Delete(Path.Combine(dirpath, "version.dll"));
                        if (File.Exists(Path.Combine(dirpath, "winmm.dll")))
                            File.Delete(Path.Combine(dirpath, "winmm.dll"));
                        if (Directory.Exists(Path.Combine(dirpath, "Logs")))
                            Directory.Delete(Path.Combine(dirpath, "Logs"), true);

                        /*
                        string MelonLoaderFolderPath = Path.Combine(dirpath, "MelonLoader");
                        if (Directory.Exists(MelonLoaderFolderPath))
                        {
                            if (Directory.Exists(Path.Combine(MelonLoaderFolderPath, "Documentation")))
                                Directory.Delete(Path.Combine(MelonLoaderFolderPath, "Documentation"), true);
                            if (Directory.Exists(Path.Combine(MelonLoaderFolderPath, "Mono")))
                                Directory.Delete(Path.Combine(MelonLoaderFolderPath, "Mono"), true);
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.dll")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.dll"));
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.ModHandler.dll")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.ModHandler.dll"));
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Il2Cpp.dll")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Il2Cpp.dll"));
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.GeneratorProcess.exe")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.GeneratorProcess.exe"));
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Mono.dll")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Mono.dll"));
                            if (File.Exists(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Mono.Pre2017.dll")))
                                File.Delete(Path.Combine(MelonLoaderFolderPath, "MelonLoader.Support.Mono.Pre2017.dll"));
                        }
                        */
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

                        mainForm.Invoke(new Action(() =>
                        {
                            mainForm.Close();
                            MessageBox.Show("Installation Successful!", Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            Application.Exit();
                        }));
                    }
                    catch (Exception ex)
                    {
                        mainForm.Invoke(new Action(() =>
                        {
                            mainForm.Close();
                            MessageBox.Show("Installation failed; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Title);
                            Application.Exit();
                        }));
                    }
                });
                
                processThread.Start();
            } else
                Application.Exit();
            
            Application.Run(mainForm);
        }
    }
}