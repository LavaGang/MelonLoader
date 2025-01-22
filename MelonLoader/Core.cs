﻿using bHapticsLib;
using MelonLoader.InternalUtils;
using MelonLoader.Properties;
using MelonLoader.Resolver;
using MelonLoader.Utils;
using System;
using System.Diagnostics;
using System.Threading;

[assembly: MelonLoader.PatchShield]

#pragma warning disable IDE0051 // Prevent the IDE from complaining about private unreferenced methods

namespace MelonLoader;

internal static class Core
{
    private static bool _success = true;

    internal static HarmonyLib.Harmony HarmonyInstance;
    internal static bool Is_ALPHA_PreRelease = false;

    internal static int Initialize()
    {
        // The config should be set before running anything else due to static constructors depending on it
        // Don't ask me how this works, because I don't know either. -slxdy
        var config = new LoaderConfig();
        BootstrapInterop.Library.GetLoaderConfig(ref config);
        LoaderConfig.Current = config;

        MelonLaunchOptions.Load();

#if NET35
        // Disabled for now because of issues
        //Net20Compatibility.TryInstall();
#endif

        MelonUtils.SetupWineCheck();

        if (MelonUtils.IsUnderWineOrSteamProton())
            Pastel.ConsoleExtensions.Disable();

        Fixes.UnhandledException.Install(AppDomain.CurrentDomain);
        Fixes.ServerCertificateValidation.Install();
        Assertions.LemonAssertMapping.Setup();

        MelonUtils.Setup(AppDomain.CurrentDomain);
        BootstrapInterop.SetDefaultConsoleTitleWithGameName(UnityInformationHandler.GameName,
            UnityInformationHandler.GameVersion);

        MelonAssemblyResolver.Setup();

#if NET6_0_OR_GREATER

        if (LoaderConfig.Current.Loader.LaunchDebugger && MelonEnvironment.IsDotnetRuntime)
        {
            MelonLogger.Msg("[Init] User requested debugger, attempting to launch now...");
            Debugger.Launch();
        }

        Environment.SetEnvironmentVariable("IL2CPP_INTEROP_DATABASES_LOCATION", MelonEnvironment.Il2CppAssembliesDirectory);

#else

        try
        {
            if (!MonoLibrary.Setup())
            {
                _success = false;
                return 1;
            }
        }
        catch (Exception ex)
        {
            MelonDebug.Msg($"[MonoLibrary] Caught Exception: {ex}");
            _success = false;
            return 1;
        }

#endif

        HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);
        Fixes.DetourContextDisposeFix.Install();

#if NET6_0_OR_GREATER
        // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //  NativeStackWalk.LogNativeStackTrace();

        Fixes.DotnetAssemblyLoadContextFix.Install();
        Fixes.DotnetModHandlerRedirectionFix.Install();
#endif

        Fixes.ForcedCultureInfo.Install();
        Fixes.InstancePatchFix.Install();
        Fixes.ProcessFix.Install();

#if NET6_0_OR_GREATER
        Fixes.AsmResolverFix.Install();
        Fixes.Il2CppInteropFixes.Install();
        Fixes.Il2CppICallInjector.Install();
#endif

        PatchShield.Install();

        MelonPreferences.Load();

        MelonCompatibilityLayer.LoadModules();

        bHapticsManager.Connect(BuildInfo.Name, UnityInformationHandler.GameName);

        MelonHandler.LoadUserlibs(MelonEnvironment.UserLibsDirectory);
        MelonHandler.LoadMelonsFromDirectory<MelonPlugin>(MelonEnvironment.PluginsDirectory);

        MelonEvents.MelonHarmonyEarlyInit.Invoke();
        MelonEvents.OnPreInitialization.Invoke();

        return 0;
    }

    private static int PreSetup()
    {
#if NET6_0_OR_GREATER
        if (_success)
            _success = Il2CppAssemblyGenerator.Run();
#endif

        return _success ? 0 : 1;
    }

    internal static bool Start()
    {
        MelonEvents.OnApplicationEarlyStart.Invoke();
        MelonStartScreen.LoadAndRun(PreSetup);

        if (!_success)
            return false;

        MelonEvents.OnPreModsLoaded.Invoke();
        MelonHandler.LoadMelonsFromDirectory<MelonMod>(MelonEnvironment.ModsDirectory);

        MelonEvents.OnPreSupportModule.Invoke();
        if (!SupportModule.Setup())
            return false;

        AddUnityDebugLog();

#if NET6_0_OR_GREATER
        RegisterTypeInIl2Cpp.SetReady();
        RegisterTypeInIl2CppWithInterfaces.SetReady();
#endif

        MelonEvents.MelonHarmonyInit.Invoke();
        MelonEvents.OnApplicationStart.Invoke();

        return true;
    }

    internal static string GetVersionString()
    {
        var lemon = LoaderConfig.Current.Loader.Theme == LoaderConfig.CoreConfig.LoaderTheme.Lemon;
        var versionStr = $"{(lemon ? "Lemon" : "Melon")}Loader " +
                         $"v{BuildInfo.Version} " +
                         $"{(Is_ALPHA_PreRelease ? "ALPHA Pre-Release" : "Open-Beta")}";
        return versionStr;
    }

    internal static void WelcomeMessage()
    {
        //if (MelonDebug.IsEnabled())
        MelonLogger.WriteSpacer();

        MelonLogger.MsgDirect("------------------------------");
        MelonLogger.MsgDirect(GetVersionString());
        MelonLogger.MsgDirect($"OS: {MelonUtils.GetOSVersion()}");
        MelonLogger.MsgDirect($"Hash Code: {MelonUtils.HashCode}");
        MelonLogger.MsgDirect("------------------------------");
        var typeString = MelonUtils.IsGameIl2Cpp() ? "Il2cpp" : MelonUtils.IsOldMono() ? "Mono" : "MonoBleedingEdge";
        MelonLogger.MsgDirect($"Game Type: {typeString}");
        var archString = MelonUtils.IsGame32Bit() ? "x86" : "x64";
        MelonLogger.MsgDirect($"Game Arch: {archString}");
        MelonLogger.MsgDirect("------------------------------");
        MelonLogger.MsgDirect("Command-Line: ");
        foreach (var pair in MelonLaunchOptions.InternalArguments)
            if (string.IsNullOrEmpty(pair.Value))
                MelonLogger.MsgDirect($"   {pair.Key}");
            else
                MelonLogger.MsgDirect($"   {pair.Key} = {pair.Value}");
        MelonLogger.MsgDirect("------------------------------");
        MelonEnvironment.PrintEnvironment();
    }

    internal static void Quit()
    {
        MelonDebug.Msg("[ML Core] Received Quit Request! Shutting down...");

        MelonPreferences.Save();

        HarmonyInstance.UnpatchSelf();
        bHapticsManager.Disconnect();

#if NET6_0_OR_GREATER
        Fixes.Il2CppInteropFixes.Shutdown();
        Fixes.Il2CppICallInjector.Shutdown();
#endif

        Thread.Sleep(200);

        if (LoaderConfig.Current.Loader.ForceQuit)
            Process.GetCurrentProcess().Kill();
    }

    private static void AddUnityDebugLog()
    {
        var msg = "~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Game Developers!   ~";
        var line = new string('-', msg.Length);
        SupportModule.Interface.UnityDebugLog(line);
        SupportModule.Interface.UnityDebugLog(msg);
        SupportModule.Interface.UnityDebugLog(line);
    }
}