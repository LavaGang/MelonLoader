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
        internal static string VersionToDownload = "0.2.1";
        internal static string Title = ("MelonLoader Installer for v" + VersionToDownload + " Open-Beta");

        [STAThread]
        static void Main()
        {
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
                        using var zipdata = new WebClient().OpenRead(
                            ("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v" + VersionToDownload +
                             "/" + (File.Exists(Path.Combine(dirpath, "GameAssembly.dll"))
                                 ? "MelonLoader.Il2Cpp.zip"
                                 : "MelonLoader.Mono.zip")));

                        mainForm.Invoke(new Action(() => { mainForm.label1.Text = "Extracting..."; }));
                        if (File.Exists(Path.Combine(dirpath, "version.dll")))
                            File.Delete(Path.Combine(dirpath, "version.dll"));
                        if (File.Exists(Path.Combine(dirpath, "winmm.dll")))
                            File.Delete(Path.Combine(dirpath, "winmm.dll"));
                        if (Directory.Exists(Path.Combine(dirpath, "MelonLoader")))
                            Directory.Delete(Path.Combine(dirpath, "MelonLoader"), true);
                        if (Directory.Exists(Path.Combine(dirpath, "Logs")))
                            Directory.Delete(Path.Combine(dirpath, "Logs"), true);
                        if (!Directory.Exists(Path.Combine(dirpath, "Mods")))
                            Directory.CreateDirectory(Path.Combine(dirpath, "Mods"));

                        using var zip = new ZipArchive(zipdata);
                        foreach (var zipArchiveEntry in zip.Entries)
                        {
                            string filepath = Path.Combine(dirpath, zipArchiveEntry.FullName);
                            if (File.Exists(filepath))
                                File.Delete(filepath);
                        }

                        zip.ExtractToDirectory(dirpath);
                        
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