﻿using MelonLoader.Melons;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader;

public static class MelonHandler
{
    /// <summary>
    /// Directory of Plugins.
    /// </summary>
    [Obsolete("Use MelonEnvironment.PluginsDirectory instead. This will be removed in a future version.", true)]
    public static string PluginsDirectory => MelonEnvironment.PluginsDirectory;

    /// <summary>
    /// Directory of Mods.
    /// </summary>
    [Obsolete("Use MelonEnvironment.ModsDirectory instead. This will be removed in a future version.", true)]
    public static string ModsDirectory => MelonEnvironment.ModsDirectory;

    internal static void Setup()
    {
        if (!Directory.Exists(MelonEnvironment.PluginsDirectory))
            Directory.CreateDirectory(MelonEnvironment.PluginsDirectory);

        if (!Directory.Exists(MelonEnvironment.ModsDirectory))
            Directory.CreateDirectory(MelonEnvironment.ModsDirectory);
    }

    public static void LoadUserlibs(string path)
        => MelonFolderHandler.Scan(MelonFolderHandler.eScanType.UserLibs, path);

    public static void LoadMelonsFromDirectory<T>(string path)
        where T : MelonTypeBase<T>
        => MelonFolderHandler.Scan(
            (typeof(T) == typeof(MelonMod)) ? MelonFolderHandler.eScanType.Mods : MelonFolderHandler.eScanType.Plugins,
            path);

    #region Obsolete Members
    /// <summary>
    /// List of Plugins.
    /// </summary>
    [Obsolete("Use 'MelonPlugin.RegisteredMelons' instead. This will be removed in a future version.", true)]
    public static List<MelonPlugin> Plugins => [.. MelonTypeBase<MelonPlugin>.RegisteredMelons];

    /// <summary>
    /// List of Mods.
    /// </summary>
    [Obsolete("Use 'MelonMod.RegisteredMelons' instead. This will be removed in a future version.", true)]
    public static List<MelonMod> Mods => [.. MelonTypeBase<MelonMod>.RegisteredMelons];

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromFile(string filelocation, bool isPlugin) => LoadFromFile(filelocation);

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromByteArray(byte[] filedata, string filelocation) => LoadFromByteArray(filedata, filepath: filelocation);

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromByteArray(byte[] filedata, string filelocation, bool isPlugin) => LoadFromByteArray(filedata, filepath: filelocation);

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromAssembly(Assembly asm, string filelocation, bool isPlugin) => LoadFromAssembly(asm, filelocation);

    [Obsolete("Use 'MelonBase.Hash' instead. This will be removed in a future version.", true)]
    public static string GetMelonHash(MelonBase melonBase)
        => melonBase.Hash;

    [Obsolete("Use 'MelonBase.RegisteredMelons.Exists(1)' instead. This will be removed in a future version.", true)]
    public static bool IsMelonAlreadyLoaded(string name)
        => MelonBase._registeredMelons.Exists(x => x.Info.Name == name);

    [Obsolete("Use 'MelonPlugin.RegisteredMelons.Exists(1)' instead. This will be removed in a future version.", true)]
    public static bool IsPluginAlreadyLoaded(string name)
        => MelonTypeBase<MelonPlugin>._registeredMelons.Exists(x => x.Info.Name == name);

    [Obsolete("Use 'MelonMod.RegisteredMelons.Exists(1)' instead. This will be removed in a future version.", true)]
    public static bool IsModAlreadyLoaded(string name)
        => MelonTypeBase<MelonMod>._registeredMelons.Exists(x => x.Info.Name == name);

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromFile(string filepath, string symbolspath = null)
    {
        var asm = MelonAssembly.LoadMelonAssembly(filepath);
        if (asm == null)
            return;

        MelonBase.RegisterSorted(asm.LoadedMelons);
    }

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
    {
        var asm = MelonAssembly.LoadRawMelonAssembly(filepath, filedata, symbolsdata);
        if (asm == null)
            return;

        MelonBase.RegisterSorted(asm.LoadedMelons);
    }

    [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead. This will be removed in a future version.", true)]
    public static void LoadFromAssembly(Assembly asm, string filepath = null)
    {
        var ma = MelonAssembly.LoadMelonAssembly(filepath, asm);
        if (ma == null)
            return;

        MelonBase.RegisterSorted(ma.LoadedMelons);
    }
    #endregion
}
