using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader.Logging;

namespace MelonLoader.Melons;

internal static class MelonFolderHandler
{
    private static bool _firstSpacer;
    private static List<string> _userLibDirs = [];
    private static List<string> _pluginDirs = [];
    private static List<string> _modDirs = [];

    internal enum ScanType
    {
        UserLibs,
        Plugins,
        Mods
    }

    internal static void ScanForFolders()
    {
        // Add Base Directories to Start of List
        _userLibDirs.Add(MelonEnvironment.UserLibsDirectory);
        _pluginDirs.Add(MelonEnvironment.PluginsDirectory);
        _modDirs.Add(MelonEnvironment.ModsDirectory);

        // Scan Base Folders
        ScanFolder(ScanType.UserLibs, MelonEnvironment.UserLibsDirectory, ref _userLibDirs, ref _pluginDirs, ref _modDirs);
        ScanFolder(ScanType.Plugins, MelonEnvironment.PluginsDirectory, ref _userLibDirs, ref _pluginDirs, ref _modDirs);
        ScanFolder(ScanType.Mods, MelonEnvironment.ModsDirectory, ref _userLibDirs, ref _pluginDirs, ref _modDirs);

        // Add Directories to Resolver
        foreach (string directory in _userLibDirs)
        {
            MelonUtils.AddNativeDLLDirectory(directory);
            Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
        }
#if NET6_0_OR_GREATER
        Resolver.MelonAssemblyResolver.AddSearchDirectory(MelonEnvironment.Il2CppAssembliesDirectory);
#endif
        foreach (string directory in _pluginDirs)
            Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
        foreach (string directory in _modDirs)
            Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
    }

    internal static void LoadMelons(ScanType type)
    {
        LogStart(type);

        bool hasWroteLine = false;
        List<MelonAssembly> melonAssemblies = new();

        if (type == ScanType.UserLibs)
            MelonPreprocessor.LoadFolders(_userLibDirs, false, ref hasWroteLine, ref melonAssemblies);

        int count = melonAssemblies.Count;
        string typeName = "UserLib".MakePlural(count);

        if (type == ScanType.Plugins)
        {
            MelonPreprocessor.LoadFolders(_pluginDirs, true, ref hasWroteLine, ref melonAssemblies);
            List<MelonPlugin> pluginsLoaded = LoadMelons<MelonPlugin>(melonAssemblies);
            if (hasWroteLine)
                MelonLogger.WriteSpacer();
            MelonBase.RegisterSorted(pluginsLoaded);

            count = MelonPlugin.RegisteredMelons.Count;
            typeName = MelonPlugin.TypeName.MakePlural(count);
        }

        if (type == ScanType.Mods)
        {
            MelonPreprocessor.LoadFolders(_modDirs, true, ref hasWroteLine, ref melonAssemblies);
            List<MelonMod> modsLoaded = LoadMelons<MelonMod>(melonAssemblies);
            if (hasWroteLine)
                MelonLogger.WriteSpacer();
            MelonBase.RegisterSorted(modsLoaded);

            count = MelonMod.RegisteredMelons.Count;
            typeName = MelonMod.TypeName.MakePlural(count);
        }

        if (hasWroteLine)
            MelonLogger.WriteLine(ColorARGB.Magenta);
        MelonLogger.Msg($"{count} {typeName} loaded.");

        if (_firstSpacer || (type == ScanType.Mods))
            MelonLogger.WriteSpacer();
        _firstSpacer = true;
    }

    private static void LogStart(ScanType type)
    {
        string typeName = Enum.GetName(typeof(ScanType), type);
        var loadingMsg = $"Loading {typeName}...";
        MelonLogger.WriteSpacer();
        MelonLogger.Msg(loadingMsg);
    }

    private static List<T> LoadMelons<T>(List<MelonAssembly> melonAssemblies)
        where T : MelonTypeBase<T>
    {
        // Load Melons from Assembly
        foreach (var asm in melonAssemblies)
            asm.LoadMelons();

        List<T> loadedMelons = new();
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

    private static void ScanFolder(ScanType scanType,
        string path,
        ref List<string> userLibDirectories,
        ref List<string> pluginDirectories,
        ref List<string> modDirectories)
    {
        string directoryName = Path.GetFileName(path);
        if (directoryName.StartsWith("~"))
            return;

        // Get Directories
        string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        if (directories.Length <= 0)
            return;

        // Parse Directories
        foreach (var dir in directories)
        {
            // Validate Path
            if (!Directory.Exists(dir))
                continue;

            directoryName = Path.GetFileName(dir);
            if (directoryName.StartsWith("~"))
                continue;

            List<string> dirList = scanType switch
            {
                ScanType.UserLibs => userLibDirectories,
                ScanType.Plugins => pluginDirectories,
                ScanType.Mods => modDirectories,
                _ => throw new ArgumentOutOfRangeException(nameof(scanType), scanType, null)
            };

            dirList.Add(dir);
            ScanFolder(scanType, dir, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
        }
    }
}