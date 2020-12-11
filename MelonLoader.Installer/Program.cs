using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Windows.Forms;

namespace MelonLoader
{
    internal static class Program
    {
        internal static MainForm mainForm = null;
        internal static WebClient webClient = null;
        internal static WebClient webClient_update = null;
        internal static bool Closing = false;
        internal static string Repo_API_Installer = "https://api.github.com/repos/LavaGang/MelonLoader.Installer/releases";
        internal static string Repo_API_MelonLoader = "https://api.github.com/repos/LavaGang/MelonLoader/releases";
        internal static string Download_MelonLoader = "https://github.com/LavaGang/MelonLoader/releases/download";
        internal static string Link_Discord = "https://discord.gg/2Wn3N2P";
        internal static string Link_Twitter = "https://twitter.com/lava_gang";
        internal static string Link_GitHub = "https://github.com/LavaGang";
        internal static string Link_Wiki = "https://melonwiki.xyz";
        internal static string Link_Update = "https://github.com/LavaGang/MelonLoader.Installer/releases/latest";

        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | (SecurityProtocolType)3072;
            webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Unity web player");
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs info) => { SetCurrentPercentage(info.ProgressPercentage); SetTotalPercentage(info.ProgressPercentage / 2); };
            Config.Load();
        }

        [STAThread]
        private static void Main()
        {
            FileNameCheck();

            // Add Command Line Options

            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        private static void FileNameCheck()
        {
            string exe_fullpath = Process.GetCurrentProcess().MainModule.FileName;
            string exe_path = Path.GetDirectoryName(exe_fullpath);
            string exe_name = Path.GetFileNameWithoutExtension(exe_fullpath);
            if (!exe_name.EndsWith(".tmp"))
            {
                string tmp_exe_path = Path.Combine(exe_path, (exe_name + ".tmp.exe"));
                if (File.Exists(tmp_exe_path))
                    File.Delete(tmp_exe_path);
                return;
            }
            string new_exe_path = Path.Combine(exe_path, (Path.GetFileNameWithoutExtension(exe_name) + ".exe"));
            if (File.Exists(new_exe_path))
                File.Delete(new_exe_path);
            File.Copy(exe_fullpath, new_exe_path);
            Process.Start(new_exe_path);
            Process.GetCurrentProcess().Kill();
        }

        internal static void SetCurrentOperation(string op)
        {
            mainForm.Invoke(new Action(() =>
            {
                mainForm.Output_Current_Operation.Text = op;
                mainForm.Output_Current_Operation.ForeColor = System.Drawing.SystemColors.Highlight;
                mainForm.Output_Current_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
                mainForm.Output_Total_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
                SetCurrentPercentage(0);
            }));
        }

        internal static void LogError(string msg)
        {
            TempFileCache.ClearCache();
            OperationError();
            File.WriteAllText(Directory.GetCurrentDirectory() + $@"\MLInstaller_{DateTime.Now:yy-M-dd_HH-mm-ss.fff}.log", msg);
#if DEBUG
            FinishingMessageBox(msg, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
            FinishingMessageBox("INTERNAL FAILURE! Please upload the created log to #melonloader-support on Discord.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }

        internal static void OperationError()
        {
            mainForm.Invoke(new Action(() =>
            {
                mainForm.Output_Current_Operation.Text = "ERROR!";
                mainForm.Output_Current_Operation.ForeColor = System.Drawing.Color.Red;
                mainForm.Output_Current_Progress_Display.Style = MetroFramework.MetroColorStyle.Red;
                mainForm.Output_Total_Progress_Display.Style = MetroFramework.MetroColorStyle.Red;
            }));
        }

        internal static void OperationSuccess()
        {
            mainForm.Invoke(new Action(() =>
            {
                mainForm.Output_Current_Operation.Text = "SUCCESS!";
                mainForm.Output_Current_Operation.ForeColor = System.Drawing.Color.Lime;
                mainForm.Output_Current_Progress_Display.Value = 100;
                mainForm.Output_Current_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
                mainForm.Output_Total_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
                mainForm.Output_Current_Progress_Text.Text = mainForm.Output_Current_Progress_Display.Value.ToString();
                mainForm.Output_Total_Progress_Display.Value = mainForm.Output_Current_Progress_Display.Value;
                mainForm.Output_Total_Progress_Text.Text = mainForm.Output_Current_Progress_Display.Value.ToString();
            }));
        }

        internal static void SetCurrentPercentage(int percentage)
        {
            mainForm.Invoke(new Action(() =>
            {
                mainForm.Output_Current_Progress_Display.Value = percentage;
                mainForm.Output_Current_Progress_Text.Text = mainForm.Output_Current_Progress_Display.Value.ToString();
            }));
        }

        internal static void SetTotalPercentage(int percentage)
        {
            mainForm.Invoke(new Action(() =>
            {
                mainForm.Output_Total_Progress_Display.Value = percentage;
                mainForm.Output_Total_Progress_Text.Text = mainForm.Output_Total_Progress_Display.Value.ToString();
            }));
        }

        internal static void FinishingMessageBox(string msg, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            mainForm.Invoke(new Action(() =>
            {
                MessageBox.Show(msg, "MelonLoader Installer", buttons, icon);
                if ((icon != MessageBoxIcon.Error) && Config.CloseAfterCompletion)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                mainForm.BackToHome();
            }));
        }

        internal static string GetFilePathFromShortcut(string shortcut_path)
        {
            string shortcut_extension = Path.GetExtension(shortcut_path);
            if (shortcut_extension.Equals(".lnk"))
                return GetFilePathFromLNK(shortcut_path);
            else if (shortcut_extension.Equals(".url"))
                return GetFilePathFromURL(shortcut_path);
            return null;
        }
        private static string GetFilePathFromLNK(string shortcut_path) => ((IWshRuntimeLibrary.IWshShortcut)new IWshRuntimeLibrary.WshShell().CreateShortcut(shortcut_path)).TargetPath;
        private static string GetFilePathFromURL(string shortcut_path)
        {
            string[] file_lines = File.ReadAllLines(shortcut_path);
            if (file_lines.Length <= 0)
                return null;
            string urlstring = file_lines.First(x => (!string.IsNullOrEmpty(x) && x.StartsWith("URL=")));
            if (string.IsNullOrEmpty(urlstring))
                return null;
            urlstring = urlstring.Substring(4);
            if (string.IsNullOrEmpty(urlstring))
                return null;
            if (urlstring.StartsWith("steam://rungameid/"))
                return SteamHandler.GetFilePathFromAppId(urlstring.Substring(18));
            return null;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) => MessageBox.Show((e.ExceptionObject as Exception).ToString());
    }
}