﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace MelonLoader
{
    class OperationHandler
    {
        private static string[] ProxyNames = { "version", "winmm", "winhttp" };

        internal enum Operations
        {
            NONE,
            INSTALLER_UPDATE,
            INSTALL,
            UNINSTALL,
            REINSTALL,
            UPDATE,
            DOWNGRADE,
        }
        internal static Operations CurrentOperation = Operations.NONE;
        internal static string CurrentOperationName
        {
            get
            {
                string opname = null;
                switch (CurrentOperation)
                {
                    case Operations.DOWNGRADE:
                        opname = "Downgrade";
                        break;
                    case Operations.UPDATE:
                        opname = "Update";
                        break;
                    case Operations.UNINSTALL:
                        opname = "Uninstall";
                        break;
                    case Operations.REINSTALL:
                        opname = "Reinstall";
                        break;
                    case Operations.INSTALL:
                    case Operations.INSTALLER_UPDATE:
                    case Operations.NONE:
                    default:
                        opname = "Install";
                        break;
                }
                return opname;
            }
        }

        internal static void Automated_Install(string destination, string selected_version, bool is_x86, bool legacy_version)
        {
            Program.SetCurrentOperation("Downloading MelonLoader...");
            string downloadurl = Program.Download_MelonLoader + "/" + selected_version + "/MelonLoader." + ((!legacy_version && is_x86) ? "x86" : "x64") + ".zip";
            string temp_path = TempFileCache.CreateFile();
            try { Program.webClient.DownloadFileAsync(new Uri(downloadurl), temp_path); while (Program.webClient.IsBusy) { } }
            catch (Exception ex)
            {
                TempFileCache.ClearCache();
                Program.OperationError();
                Program.FinishingMessageBox(ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Program.SetTotalPercentage(50);
            if (Program.Closing)
                return;

            // SHA512 Check Downloaded File

            Program.SetCurrentOperation("Extracting MelonLoader...");
            try
            {
                string MelonLoader_Folder = Path.Combine(destination, "MelonLoader");
                if (Directory.Exists(MelonLoader_Folder))
                    Directory.Delete(MelonLoader_Folder, true);
                string proxy_path = null;
                if (GetExistingProxyPath(destination, out proxy_path))
                    File.Delete(proxy_path);

                using FileStream stream = new FileStream(temp_path, FileMode.Open, FileAccess.Read);
                using ZipArchive zip = new ZipArchive(stream);
                int total_entry_count = zip.Entries.Count;
                for (int i = 0; i < total_entry_count; i++)
                {
                    if (Program.Closing)
                        break;
                    int percentage = ((i / total_entry_count) * 100);
                    Program.SetCurrentPercentage(percentage);
                    Program.SetTotalPercentage((50 + (percentage / 2)));
                    ZipArchiveEntry entry = zip.Entries[i];
                    string fullPath = Path.GetFullPath(Path.Combine(destination, entry.FullName));
                    if (!fullPath.StartsWith(destination))
                        throw new IOException("Extracting Zip entry would have resulted in a file outside the specified destination directory.");
                    string filename = Path.GetFileName(fullPath);
                    if (filename.Length != 0)
                    {
                        if (!legacy_version && filename.Equals("version.dll"))
                        {
                            foreach (string proxyname in ProxyNames)
                            {
                                string new_proxy_path = Path.Combine(destination, (proxyname + ".dll"));
                                if (File.Exists(new_proxy_path))
                                    continue;
                                fullPath = Path.GetFullPath(new_proxy_path);
                                break;
                            }
                        }
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        using FileStream targetStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
                        using Stream entryStream = entry.Open();
                        entryStream.CopyTo(targetStream);
                        continue;
                    }
                    if (entry.Length != 0)
                        throw new IOException("Zip entry name ends in directory separator character but contains data.");
                    Directory.CreateDirectory(fullPath);
                }
            }
            catch (Exception ex)
            {
                TempFileCache.ClearCache();
                Program.OperationError();
                Program.FinishingMessageBox(ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Program.Closing)
                return;
            TempFileCache.ClearCache();
            Program.OperationSuccess();
            Program.FinishingMessageBox((CurrentOperationName + " was Successful!"), MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        internal static void ManualZip_Install(string zip_path, string destination)
        {
            Program.SetCurrentOperation("Extracting Zip Archive...");
            try
            {
                string MelonLoader_Folder = Path.Combine(destination, "MelonLoader");
                if (Directory.Exists(MelonLoader_Folder))
                    Directory.Delete(MelonLoader_Folder, true);
                string proxy_path = null;
                if (GetExistingProxyPath(destination, out proxy_path))
                    File.Delete(proxy_path);
                using FileStream stream = new FileStream(zip_path, FileMode.Open, FileAccess.Read);
                using ZipArchive zip = new ZipArchive(stream);
                int total_entry_count = zip.Entries.Count;
                for (int i = 0; i < total_entry_count; i++)
                {
                    if (Program.Closing)
                        break;
                    int percentage = ((i / total_entry_count) * 100);
                    Program.SetCurrentPercentage(percentage);
                    Program.SetTotalPercentage(percentage);
                    ZipArchiveEntry entry = zip.Entries[i];
                    string fullPath = Path.GetFullPath(Path.Combine(destination, entry.FullName));
                    if (!fullPath.StartsWith(destination))
                        throw new IOException("Extracting Zip entry would have resulted in a file outside the specified destination directory.");
                    string filename = Path.GetFileName(fullPath);
                    if (filename.Length != 0)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        using FileStream targetStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
                        using Stream entryStream = entry.Open();
                        entryStream.CopyTo(targetStream);
                        continue;
                    }
                    if (entry.Length != 0)
                        throw new IOException("Zip entry name ends in directory separator character but contains data.");
                    Directory.CreateDirectory(fullPath);
                }
            }
            catch (Exception ex)
            {
                Program.OperationError();
                Program.FinishingMessageBox(ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Program.Closing)
                return;
            Program.OperationSuccess();
            Program.FinishingMessageBox((CurrentOperationName + " was Successful!"), MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        internal static void Uninstall(string destination)
        {
            Program.SetCurrentOperation("Uninstalling MelonLoader...");
            try
            {
                string MelonLoader_Folder = Path.Combine(destination, "MelonLoader");
                if (Directory.Exists(MelonLoader_Folder))
                    Directory.Delete(MelonLoader_Folder, true);
                string proxy_path = null;
                if (GetExistingProxyPath(destination, out proxy_path))
                    File.Delete(proxy_path);
            }
            catch (Exception ex)
            {
                Program.OperationError();
                Program.FinishingMessageBox(ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Program.Closing)
                return;
            Program.mainForm.CurrentInstalledVersion = null;
            Program.OperationSuccess();
            Program.FinishingMessageBox((CurrentOperationName + " was Successful!"), MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private static bool GetExistingProxyPath(string destination, out string proxy_path)
        {
            proxy_path = null;
            foreach (string proxy in ProxyNames)
            {
                string new_proxy_path = Path.Combine(destination, (proxy + ".dll"));
                if (!File.Exists(new_proxy_path))
                    continue;
                FileVersionInfo fileinfo = FileVersionInfo.GetVersionInfo(new_proxy_path);
                if (fileinfo == null)
                    continue;
                if (!string.IsNullOrEmpty(fileinfo.LegalCopyright) && fileinfo.LegalCopyright.Contains("Microsoft"))
                    continue;
                proxy_path = new_proxy_path;
                break;
            }
            return !string.IsNullOrEmpty(proxy_path);
        }
    }
}