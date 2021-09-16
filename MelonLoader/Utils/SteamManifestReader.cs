using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace MelonLoader.Utils
{
    public static class SteamManifestReader
    {
        public static string GetInstallPathFromAppId(string appid)
        {
            if (string.IsNullOrEmpty(appid))
                return null;

            string steaminstallpath = GetSteamInstallPath();
            if (string.IsNullOrEmpty(steaminstallpath) || !Directory.Exists(steaminstallpath))
                return null;

            string steamappspath = Path.Combine(steaminstallpath, "steamapps");
            if (!Directory.Exists(steamappspath))
                return null;

            string appmanifestfilename = ("appmanifest_" + appid + ".acf");
            string appmanifestpath = Path.Combine(steamappspath, appmanifestfilename);
            string installdir = ReadAppManifestInstallDir(appmanifestpath);
            if (string.IsNullOrEmpty(installdir))
            {
                installdir = ReadLibraryFolders(appmanifestfilename, ref steamappspath);
                if (string.IsNullOrEmpty(installdir))
                    return null;
            }

            return installdir;
        }

        private static string ReadAppManifestInstallDir(string appmanifestpath)
        {
            if (!File.Exists(appmanifestpath))
                return null;

            string[] file_lines = File.ReadAllLines(appmanifestpath);
            if (file_lines.Length <= 0)
                return null;

            string output = null;
            foreach (string line in file_lines)
            {
                Match match = new Regex(@"""installdir""\s+""(.+)""").Match(line);
                if (!match.Success)
                    continue;

                output = match.Groups[1].Value;
                break;
            }
            return output;
        }

        private static string ReadLibraryFolders(string appmanifestfilename, ref string steamappspath)
        {
            string libraryfoldersfilepath = Path.Combine(steamappspath, "libraryfolders.vdf");
            if (!File.Exists(libraryfoldersfilepath))
                return null;
            string[] file_lines = File.ReadAllLines(libraryfoldersfilepath);
            if (file_lines.Length <= 0)
                return null;
            string output = null;
            foreach (string line in file_lines)
            {
                Match match = new Regex(@"""\d+""\s+""(.+)""").Match(line);
                if (!match.Success)
                    continue;

                string steamappspath2 = Path.Combine(match.Groups[1].Value.Replace(":\\\\", ":\\"), "steamapps");
                if (!Directory.Exists(steamappspath2))
                    continue;

                string installdir = ReadAppManifestInstallDir(Path.Combine(steamappspath2, appmanifestfilename));
                if (string.IsNullOrEmpty(installdir))
                    continue;

                steamappspath = steamappspath2;
                output = installdir;
            }
            return output;
        }

        private static string GetSteamInstallPath()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam");
            if (key == null)
                return null;

            object installpathobj = key.GetValue("InstallPath");
            if (installpathobj == null)
                return null;

            return installpathobj.ToString();
        }
    }
}
