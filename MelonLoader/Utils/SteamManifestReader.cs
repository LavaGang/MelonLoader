using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
#pragma warning disable CA1416

namespace MelonLoader.Utils;

public static class SteamManifestReader
{
    public static string GetInstallPathFromAppId(string appid)
    {
        if (!MelonUtils.IsWindows
            || MelonUtils.IsUnderWineOrSteamProton())
            return null;

        if (string.IsNullOrEmpty(appid))
            return null;

        var steaminstallpath = GetSteamInstallPath();
        if (string.IsNullOrEmpty(steaminstallpath) || !Directory.Exists(steaminstallpath))
            return null;

        var steamappspath = Path.Combine(steaminstallpath, "steamapps");
        if (!Directory.Exists(steamappspath))
            return null;

        var appmanifestfilename = "appmanifest_" + appid + ".acf";
        var appmanifestpath = Path.Combine(steamappspath, appmanifestfilename);
        var installdir = ReadAppManifestInstallDir(appmanifestpath);
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
        if (!MelonUtils.IsWindows
            || MelonUtils.IsUnderWineOrSteamProton())
            return null;

        if (!File.Exists(appmanifestpath))
            return null;

        var file_lines = File.ReadAllLines(appmanifestpath);
        if (file_lines.Length <= 0)
            return null;

        string output = null;
        foreach (var line in file_lines)
        {
            var match = new Regex(@"""installdir""\s+""(.+)""").Match(line);
            if (!match.Success)
                continue;

            output = match.Groups[1].Value;
            break;
        }

        return output;
    }

    private static string ReadLibraryFolders(string appmanifestfilename, ref string steamappspath)
    {
        if (!MelonUtils.IsWindows
            || MelonUtils.IsUnderWineOrSteamProton())
            return null;

        var libraryfoldersfilepath = Path.Combine(steamappspath, "libraryfolders.vdf");
        if (!File.Exists(libraryfoldersfilepath))
            return null;
        var file_lines = File.ReadAllLines(libraryfoldersfilepath);
        if (file_lines.Length <= 0)
            return null;
        string output = null;
        foreach (var line in file_lines)
        {
            var match = new Regex(@"""\d+""\s+""(.+)""").Match(line);
            if (!match.Success)
                continue;

            var steamappspath2 = Path.Combine(match.Groups[1].Value.Replace(":\\\\", ":\\"), "steamapps");
            if (!Directory.Exists(steamappspath2))
                continue;

            var installdir = ReadAppManifestInstallDir(Path.Combine(steamappspath2, appmanifestfilename));
            if (string.IsNullOrEmpty(installdir))
                continue;

            steamappspath = steamappspath2;
            output = installdir;
        }

        return output;
    }

    private static string GetSteamInstallPath()
    {
        if (!MelonUtils.IsWindows
            || MelonUtils.IsUnderWineOrSteamProton())
            return null;

        var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");
        key ??= Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam");
        if (key == null)
            return null;

        var installpathobj = key.GetValue("InstallPath");
        return installpathobj?.ToString();
    }
}
