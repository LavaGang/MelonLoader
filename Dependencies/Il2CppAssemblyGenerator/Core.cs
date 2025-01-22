﻿using MelonLoader.Il2CppAssemblyGenerator.Packages;
using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;
using MelonLoader.Modules;
using MelonLoader.Properties;
using MelonLoader.Utils;
using System.IO;
using System.Net.Http;

namespace MelonLoader.Il2CppAssemblyGenerator;

internal class Core : MelonModule
{
    internal static string BasePath = null;
    internal static string GameAssemblyPath = null;
    internal static string ManagedPath = null;

    internal static HttpClient webClient = null;

    internal static ExecutablePackage cpp2il = null;
    internal static Cpp2IL_StrippedCodeRegSupport cpp2il_scrs = null;

    internal static Packages.Il2CppInterop il2cppinterop = null;
    internal static UnityDependencies unitydependencies = null;
    internal static DeobfuscationMap deobfuscationMap = null;
    internal static DeobfuscationRegex deobfuscationRegex = null;

    internal static bool AssemblyGenerationNeeded = false;

    internal static MelonLogger.Instance Logger;

    public override void OnInitialize()
    {
        Logger = LoggerInstance;

        webClient = new();
        webClient.DefaultRequestHeaders.Add("User-Agent", $"{BuildInfo.Name} v{BuildInfo.Version}");

        AssemblyGenerationNeeded = LoaderConfig.Current.UnityEngine.ForceRegeneration;

        var gameAssemblyName = "GameAssembly";
        if (MelonUtils.IsUnix)
            gameAssemblyName += ".so";
        if (MelonUtils.IsWindows)
            gameAssemblyName += ".dll";
        if (MelonUtils.IsMac)
            gameAssemblyName += ".dylib";

        GameAssemblyPath = Path.Combine(MelonEnvironment.GameRootDirectory, gameAssemblyName);
        ManagedPath = MelonEnvironment.MelonManagedDirectory;
        BasePath = MelonEnvironment.Il2CppAssemblyGeneratorDirectory;
    }

    private static int Run()
    {
        Config.Initialize();

        if (!LoaderConfig.Current.UnityEngine.ForceOfflineGeneration)
            RemoteAPI.Contact();

        var cpp2IL_netcore = new Cpp2IL();
        cpp2il = MelonUtils.IsWindows
            && (cpp2IL_netcore.VersionSem < Cpp2IL.NetCoreMinVersion)
            ? new Cpp2IL_NetFramework()
            : cpp2IL_netcore;
        cpp2il_scrs = new Cpp2IL_StrippedCodeRegSupport(cpp2il);

        il2cppinterop = new Packages.Il2CppInterop();
        unitydependencies = new UnityDependencies();
        deobfuscationMap = new DeobfuscationMap();
        deobfuscationRegex = new DeobfuscationRegex();

        Logger.Msg($"Using Cpp2IL Version: {(string.IsNullOrEmpty(cpp2il.Version) ? "null" : cpp2il.Version)}");
        Logger.Msg($"Using Il2CppInterop Version = {(string.IsNullOrEmpty(il2cppinterop.Version) ? "null" : il2cppinterop.Version)}");
        Logger.Msg($"Using Unity Dependencies Version = {(string.IsNullOrEmpty(unitydependencies.Version) ? "null" : unitydependencies.Version)}");
        Logger.Msg($"Using Deobfuscation Regex = {(string.IsNullOrEmpty(deobfuscationRegex.Regex) ? "null" : deobfuscationRegex.Regex)}");

        if (!cpp2il.Setup()
            || !cpp2il_scrs.Setup()
            || !il2cppinterop.Setup()
            || !unitydependencies.Setup()
            || !deobfuscationMap.Setup())
            return 1;

        deobfuscationRegex.Setup();

        string CurrentGameAssemblyHash;
        Logger.Msg("Checking GameAssembly...");
        MelonDebug.Msg($"Last GameAssembly Hash: {Config.Values.GameAssemblyHash}");
        MelonDebug.Msg($"Current GameAssembly Hash: {CurrentGameAssemblyHash = MelonUtils.ComputeSimpleSHA512Hash(GameAssemblyPath)}");

        if (string.IsNullOrEmpty(Config.Values.GameAssemblyHash)
            || !Config.Values.GameAssemblyHash.Equals(CurrentGameAssemblyHash))
            AssemblyGenerationNeeded = true;

        if (!AssemblyGenerationNeeded)
        {
            Logger.Msg("Assembly is up to date. No Generation Needed.");
            return 0;
        }
        Logger.Msg("Assembly Generation Needed!");

        cpp2il.Cleanup();
        il2cppinterop.Cleanup();

        if (!cpp2il.Execute())
        {
            cpp2il.Cleanup();
            return 1;
        }

        if (!il2cppinterop.Execute())
        {
            cpp2il.Cleanup();
            il2cppinterop.Cleanup();
            return 1;
        }

        OldFiles_Cleanup();
        OldFiles_LAM();

        cpp2il.Cleanup();
        il2cppinterop.Cleanup();

        Logger.Msg("Assembly Generation Successful!");
        deobfuscationRegex.Save();
        Config.Values.GameAssemblyHash = CurrentGameAssemblyHash;
        Config.Save();

        return 0;
    }

    private static void OldFiles_Cleanup()
    {
        if (Config.Values.OldFiles.Count <= 0)
            return;
        for (var i = 0; i < Config.Values.OldFiles.Count; i++)
        {
            var filename = Config.Values.OldFiles[i];
            var filepath = Path.Combine(MelonEnvironment.Il2CppAssembliesDirectory, filename);
            if (File.Exists(filepath))
            {
                Logger.Msg("Deleting " + filename);
                File.Delete(filepath);
            }
        }
        Config.Values.OldFiles.Clear();
    }

    private static void OldFiles_LAM()
    {
        var filepathtbl = Directory.GetFiles(il2cppinterop.OutputFolder);
        var il2CppAssembliesDirectory = MelonEnvironment.Il2CppAssembliesDirectory;
        for (var i = 0; i < filepathtbl.Length; i++)
        {
            var filepath = filepathtbl[i];
            var filename = Path.GetFileName(filepath);
            Logger.Msg("Moving " + filename);
            Config.Values.OldFiles.Add(filename);
            var newfilepath = Path.Combine(il2CppAssembliesDirectory, filename);
            if (File.Exists(newfilepath))
                File.Delete(newfilepath);
            Directory.CreateDirectory(il2CppAssembliesDirectory);
            File.Move(filepath, newfilepath);
        }
        Config.Save();
    }
}