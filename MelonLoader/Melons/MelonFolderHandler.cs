using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons;

internal class MelonFolderHandler
{
    private static bool firstSpacer = false;

    internal enum eScanType
    {
        UserLibs,
        Plugins,
        Mods,
    }

    internal static void Scan(eScanType type, string path)
    {
        // Get Full Directory Path
        path = Path.GetFullPath(path);

        // Log Loading Message
        LogStart(path, type);

        // Find Melon Folders
        List<string> userLibDirs = [];
        List<string> pluginDirs = [];
        List<string> modDirs = [];
        FindMelonFolders(type, path, ref userLibDirs, ref pluginDirs, ref modDirs);

        // Load UserLib Assemblies
        var hasWroteLine = false;
        List<MelonAssembly> userLibAssemblies = [];
        MelonPreprocessor.LoadFolders(userLibDirs, type == eScanType.UserLibs, ref hasWroteLine, ref userLibAssemblies);

        if (type != eScanType.UserLibs)
        {
            // Load Plugins
            List<MelonAssembly> pluginAssemblies = [];
            MelonPreprocessor.LoadFolders(pluginDirs, true, ref hasWroteLine, ref pluginAssemblies);
            var pluginsLoaded =
                    LoadMelons<MelonPlugin>(pluginAssemblies);

            // Load Mods
            List<MelonMod> modsLoaded = null;
            if (type == eScanType.Mods)
            {
                List<MelonAssembly> modAssemblies = [];
                MelonPreprocessor.LoadFolders(modDirs, true, ref hasWroteLine, ref modAssemblies);
                modsLoaded = LoadMelons<MelonMod>(modAssemblies);
            }

            // Register and Sort Plugins
            if (hasWroteLine)
                MelonLogger.WriteSpacer();
            MelonBase.RegisterSorted(pluginsLoaded);

            // Register and Sort Mods
            if (type == eScanType.Mods)
                MelonBase.RegisterSorted(modsLoaded);
        }

        // Log Count
        var count = (type == eScanType.UserLibs)
            ? userLibAssemblies.Count
            : ((type == eScanType.Mods) ? MelonMod.RegisteredMelons.Count : MelonPlugin.RegisteredMelons.Count);
        var typeName = (type == eScanType.UserLibs)
            ? "UserLib".MakePlural(count)
            : ((type == eScanType.Mods) ? MelonMod.TypeName.MakePlural(count) : MelonPlugin.TypeName.MakePlural(count));

        if (hasWroteLine)
            MelonLogger.WriteLine(Color.Magenta);
        MelonLogger.Msg($"{count} {typeName} loaded.");

        // Final Spacer
        if (firstSpacer || (type == eScanType.Mods))
            MelonLogger.WriteSpacer();
        firstSpacer = true;
    }

    private static void LogStart(string path, eScanType type)
    {
        var typeName = Enum.GetName(typeof(eScanType), type);
        var loadingMsg = $"Loading {typeName} from '{path}'...";
        MelonLogger.WriteSpacer();
        MelonLogger.Msg(loadingMsg);
    }

    private static List<T> LoadMelons<T>(List<MelonAssembly> melonAssemblies)
        where T : MelonTypeBase<T>
    {
        // Load Melons from Assembly
        foreach (var asm in melonAssemblies)
            asm.LoadMelons();

        List<T> loadedMelons = [];
        foreach (var asm in melonAssemblies)
            foreach (var m in asm.LoadedMelons)
            {
                // Validate Type
                if (m is T t)
                {
                    loadedMelons.Add(t);
                    continue;
                }

                // Log Failure
                MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{m.MelonAssembly.Location}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {MelonTypeBase<T>.TypeName}. Make sure it's in the right folder.");
            }

        return loadedMelons;
    }

    private static void FindMelonFolders(
        eScanType scanType,
        string path,
        ref List<string> userLibDirectories,
        ref List<string> pluginDirectories,
        ref List<string> modDirectories)
    {
        // Validate Path
        if (!Directory.Exists(path))
            return;

        // Scan Directories
        ScanFolder(scanType, path, ref userLibDirectories, ref pluginDirectories, ref modDirectories);

        if (scanType == eScanType.UserLibs)
            userLibDirectories.Add(path);
        else
        {
            if (scanType == eScanType.Mods)
                modDirectories.Add(path);
            else
                pluginDirectories.Add(path);
        }

        // Add Directories to Resolver
        foreach (var directory in userLibDirectories)
        {
            MelonUtils.AddNativeDLLDirectory(directory);
            Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
        }

        if (scanType != eScanType.UserLibs)
        {
            foreach (var directory in pluginDirectories)
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            foreach (var directory in modDirectories)
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
        }
    }

    private static void ScanFolder(eScanType scanType,
        string path,
        ref List<string> userLibDirectories,
        ref List<string> pluginDirectories,
        ref List<string> modDirectories)
    {
        // Get Directories
        var directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        if ((directories == null)
            || (directories.Length <= 0))
            return;

        // Parse Directories
        foreach (var dir in directories)
        {
            // Validate Path
            if (!Directory.Exists(dir))
                continue;

            // Validate Manifest
            var manifestPath = Path.Combine(dir, "manifest.json");
            if (!File.Exists(manifestPath))
                continue;

            // Check for Deeper UserLibs
            var userLibsPath = Path.Combine(dir, "UserLibs");
            if (Directory.Exists(userLibsPath))
            {
                userLibDirectories.Add(userLibsPath);
                ScanFolder(eScanType.UserLibs, userLibsPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
            }

            // Is UserLibs Scan?
            if (scanType == eScanType.UserLibs)
                userLibDirectories.Add(dir); // Add to Directories List
            else
            {
                // Check for Deeper Melon Folder
                var melonPath = Path.Combine(dir, "Plugins");
                if (Directory.Exists(melonPath))
                {
                    pluginDirectories.Add(melonPath);
                    ScanFolder(eScanType.Plugins, melonPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                }

                if (scanType == eScanType.Mods)
                {
                    melonPath = Path.Combine(dir, "Mods");
                    if (Directory.Exists(melonPath))
                    {
                        modDirectories.Add(melonPath);
                        ScanFolder(scanType, melonPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                    }
                }

                // Add to Directories List
                if (scanType == eScanType.Mods)
                    modDirectories.Add(dir);
                else
                    pluginDirectories.Add(dir);
            }
        }
    }
}