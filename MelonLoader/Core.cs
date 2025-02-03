using System;
using System.Diagnostics;
using System.Threading;
using MelonLoader.Resolver;
using MelonLoader.Utils;
using MelonLoader.InternalUtils;
using MelonLoader.Properties;
using MelonLoader.Melons;
using MelonLoader.Modules;
using System.Net;

[assembly: MelonLoader.PatchShield]

#pragma warning disable IDE0051 // Prevent the IDE from complaining about private unreferenced methods

namespace MelonLoader
{
    internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance;

        // Runtime Initialization
        internal static void Stage1()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            MelonEnvironment.Initialize(AppDomain.CurrentDomain);

            // The config should be set before running anything else due to static constructors depending on it
            // Don't ask me how this works, because I don't know either. -slxdy
            var config = new LoaderConfig();
            BootstrapInterop.Library.GetLoaderConfig(ref config);
            LoaderConfig.Current = config;

            MelonLaunchOptions.Load();

#if NET35

            // Disabled for now because of issues
            //Net20Compatibility.TryInstall();

#elif NET6_0_OR_GREATER

            if (LoaderConfig.Current.Loader.LaunchDebugger && MelonEnvironment.IsDotnetRuntime)
            {
                MelonLogger.Msg("[Init] User requested debugger, attempting to launch now...");
                Debugger.Launch();
            }

            // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //  NativeStackWalk.LogNativeStackTrace();

#endif

            if (OsUtils.IsWineOrProton())
                Pastel.ConsoleExtensions.Disable();

            Fixes.UnhandledException.Install(AppDomain.CurrentDomain);
            Fixes.ServerCertificateValidation.Install();

            Assertions.LemonAssertMapping.Setup();
            MelonAssemblyResolver.Setup();

            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            Fixes.DetourContextDisposeFix.Install();
            Fixes.ForcedCultureInfo.Install();
            Fixes.InstancePatchFix.Install();
            Fixes.ProcessFix.Install();

#if NET6_0_OR_GREATER
            Fixes.AsmResolverFix.Install();
            Fixes.DotnetAssemblyLoadContextFix.Install();
            Fixes.DotnetModHandlerRedirectionFix.Install();
#endif

            PatchShield.Install();
            MelonPreferences.Load();
        }

        // After Engine Module Initializes
        internal static void Stage2()
        {
            MelonEnvironment.WelcomeMessage();

            ModuleFolderHandler.ScanForFolders();
            ModuleFolderHandler.LoadMelons(ModuleFolderHandler.eScanType.UserLibs);
            ModuleFolderHandler.LoadMelons(ModuleFolderHandler.eScanType.Plugins);

            MelonEvents.MelonHarmonyEarlyInit.Invoke();
            MelonEvents.OnPreInitialization.Invoke();
        }

        // Application Start
        internal static void Stage3(string supportModulePath)
        {
            MelonEvents.OnApplicationEarlyStart.Invoke();

            MelonEvents.OnPreModsLoaded.Invoke();
            ModuleFolderHandler.LoadMelons(ModuleFolderHandler.eScanType.Mods);

            MelonEvents.OnPreSupportModule.Invoke();

            ModuleInterop.StartSupport(supportModulePath);

            MelonEvents.MelonHarmonyInit.Invoke();
            MelonEvents.OnApplicationStart.Invoke();
        }

        internal static void Quit()
        {
            MelonDebug.Msg("[ML Core] Received Quit Request! Shutting down...");
            
            // Run Quit Callback

            MelonPreferences.Save();

            HarmonyInstance.UnpatchSelf();

            Thread.Sleep(200);

            if (LoaderConfig.Current.Loader.ForceQuit)
                Process.GetCurrentProcess().Kill();
        }
    }
}