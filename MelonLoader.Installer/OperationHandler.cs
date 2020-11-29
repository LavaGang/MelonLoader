using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;
#pragma warning disable 0168

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
                Program.LogError(ex.ToString());
                return;
            }
            Program.SetTotalPercentage(50);
            if (Program.Closing)
                return;
            string repo_hash_url = Program.Download_MelonLoader + "/" + selected_version + "/MelonLoader." + ((!legacy_version && is_x86) ? "x86" : "x64") + ".sha512";
            string repo_hash = null;
            try { repo_hash = Program.webClient.DownloadString(repo_hash_url); } catch (Exception ex) { repo_hash = null; }
            if (string.IsNullOrEmpty(repo_hash))
            {
                Program.LogError("Failed to get SHA512 Hash from Repo!");
                return;
            }
            if (Program.Closing)
                return;
            SHA512Managed sha512 = new SHA512Managed();
            byte[] checksum = sha512.ComputeHash(File.ReadAllBytes(temp_path));
            if ((checksum == null) || (checksum.Length <= 0))
            {
                Program.LogError("Failed to get SHA512 Hash from Temp File!");
                return;
            }
            string file_hash = BitConverter.ToString(checksum).Replace("-", string.Empty);
            if (string.IsNullOrEmpty(file_hash))
            {
                Program.LogError("Failed to get SHA512 Hash from Temp File!");
                return;
            }
            if (!file_hash.Equals(repo_hash))
            {
                Program.LogError("SHA512 Hash from Temp File does not match Repo Hash!");
                return;
            }
            Program.SetCurrentOperation("Extracting MelonLoader...");
            try
            {
                string MelonLoader_Folder = Path.Combine(destination, "MelonLoader");
                if (Directory.Exists(MelonLoader_Folder))
                    Directory.Delete(MelonLoader_Folder, true);
                string proxy_path = null;
                if (GetExistingProxyPath(destination, out proxy_path))
                    File.Delete(proxy_path);
                if (legacy_version && (Program.mainForm.CurrentInstalledVersion.CompareTo("0.3.0") >= 0))
                    DowngradeMelonPreferences(destination);
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
                Program.LogError(ex.ToString());
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
                Program.LogError(ex.ToString());
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
                Program.LogError(ex.ToString());
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

        private static void DowngradeMelonPreferences(string destination)
        {
            string userdatapath = Path.Combine(destination, "UserData");
            string newfilepath = Path.Combine(userdatapath, "modprefs.ini");
            if (File.Exists(newfilepath))
                File.Delete(newfilepath);
            string oldfilepath = Path.Combine(userdatapath, "MelonPreferences.cfg");
            if (!File.Exists(oldfilepath))
                return;
            string filestr = File.ReadAllText(oldfilepath);
            if (string.IsNullOrEmpty(filestr))
                return;
            IniFile iniFile = new IniFile(newfilepath);
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return;
            TomlTable model = docsyn.ToModel();
            if (model.Count <= 0)
                return;
            foreach (KeyValuePair<string, object> keypair in model)
            {
                string category_name = keypair.Key;
                TomlTable tbl = (TomlTable)keypair.Value;
                if (tbl.Count <= 0)
                    continue;
                foreach (KeyValuePair<string, object> tblkeypair in tbl)
                {
                    string name = tblkeypair.Key;
                    if (string.IsNullOrEmpty(name))
                        continue;
                    TomlObject obj = TomlObject.ToTomlObject(tblkeypair.Value);
                    if (obj == null)
                        continue;
                    if (obj.Kind == ObjectKind.String)
                        iniFile.SetString(category_name, name, ((TomlString)obj).Value);
                    else if (obj.Kind == ObjectKind.Boolean)
                        iniFile.SetBool(category_name, name, ((TomlBoolean)obj).Value);
                    else if (obj.Kind == ObjectKind.Integer)
                        iniFile.SetInt(category_name, name, (int)((TomlInteger)obj).Value);
                    else if (obj.Kind == ObjectKind.Float)
                        iniFile.SetFloat(category_name, name, (float)((TomlFloat)obj).Value);
                }
            }
            File.Delete(oldfilepath);
        }
    }
}