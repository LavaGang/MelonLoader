using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using bHapticsLib;
using System.Threading;
using HarmonyLib;
using MelonLoader.Resolver;
using MelonLoader.Utils;
using MelonLoader.InternalUtils;
using MelonLoader.Melons;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.Platforms;

[assembly: MelonLoader.PatchShield]

#pragma warning disable IDE0051 // Prevent the IDE from complaining about private unreferenced methods

namespace MelonLoader
{
    internal static class Core
    {
        private static bool _success = true;

        internal static HarmonyLib.Harmony HarmonyInstance;
        internal static bool Is_ALPHA_PreRelease = false;

        internal static int Initialize()
        {
#if ANDROID
            Java.JNI.Initialize(BootstrapInterop.Library.GetJavaVM());
            APKAssetManager.Initialize();
#endif

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

#if NET35
            Fixes.NetFramework.ServerCertificateValidation.Install();
#endif

            Assertions.LemonAssertMapping.Setup();
            HarmonyLogger.Setup();

#if !WINDOWS && !NET6_0_OR_GREATER
            // Using Process.Start can run Console..cctor
            // Since MonoMod's PlatformHelper (used by DetourHelper.Native) runs Process.Start to determine ARM/x86
            // platform, this causes the unpatched TermInfoReader to kick in before it can be patched and fixed when
            // installing the XTermFix below. To work around this, we can force the platform directly
            DetourHelper.Native = new DetourNativeMonoPosixPlatform(new DetourNativeX86Platform());
#endif

            HarmonyInstance = new HarmonyLib.Harmony(Properties.BuildInfo.Name);

#if !WINDOWS && !NET6_0_OR_GREATER
            Fixes.NetFramework.XTermFix.Install();
#endif

#if OSX
            Fixes.ProcessModulesFix.Install();
#endif

            MelonUtils.Setup(AppDomain.CurrentDomain);
            MelonAssemblyResolver.Setup();
            BootstrapInterop.SetDefaultConsoleTitleWithGameName(UnityInformationHandler.GameName, 
                UnityInformationHandler.GameVersion);

#if NET6_0_OR_GREATER

            if (LoaderConfig.Current.Loader.LaunchDebugger && MelonEnvironment.IsDotnetRuntime)
            {
#if WINDOWS
                MelonLogger.Msg("[Init] User requested debugger, attempting to launch now...");
                if (Debugger.Launch())
                {
                    MelonLogger.Msg("[Init] Done interacting with the debugger launch window, waiting for the debugger to be attached...");
                    while (!Debugger.IsAttached)
                    { }
                    MelonLogger.Msg("[Init] Detected a debugger, resuming initialization...");
                }
#else
                MelonLogger.Msg("[Init] User requested to wait until a debugger is attached, waiting now...");
                while (!Debugger.IsAttached)
                { }
                MelonLogger.Msg("[Init] Detected a debugger, resuming initialization...");
#endif
            }

            Environment.SetEnvironmentVariable("IL2CPP_INTEROP_DATABASES_LOCATION", MelonEnvironment.Il2CppAssembliesDirectory);
            MelonAssemblyResolver.AddSearchDirectory(MelonEnvironment.Il2CppAssembliesDirectory);

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

            Fixes.MonoMod.DetourContextDisposeFix.Install();

#if NET6_0_OR_GREATER
            // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //  NativeStackWalk.LogNativeStackTrace();

#if !ANDROID
            Fixes.Dotnet.DotnetAssemblyLoadContextFix.Install();
#endif
            Fixes.Dotnet.DotnetModHandlerRedirectionFix.Install();
#endif

            Fixes.ForcedCultureInfo.Install();
            Fixes.Harmony.InstancePatchFix.Install();

#if WINDOWS
            Fixes.ProcessFix.Install();
#endif

#if NET6_0_OR_GREATER
            Fixes.Il2CppInterop.Il2CppInteropExceptionLog.Install();

#if OSX
            Fixes.Il2CppInterop.Il2CppInteropMacFix.Install();
            Fixes.Dotnet.NativeLibraryFix.Install();
#endif

            Fixes.Il2CppInterop.Il2CppInteropFixes.Install();
#if !ANDROID
            Fixes.Il2CppInterop.Il2CppInteropGetFieldDefaultValueFix.Install();
#endif

            Fixes.Il2CppInterop.Il2CppICallInjector.Install();

#endif

            PatchShield.Install();

#if WINDOWS
            Fixes.WindowsUnhandledQuit.Install();
            MelonEvents.OnUpdate.Subscribe(Fixes.WindowsUnhandledQuit.Update, int.MaxValue);
#endif

            MelonPreferences.Load();

            MelonCompatibilityLayer.LoadModules();

            bHapticsManager.Connect(Properties.BuildInfo.Name, UnityInformationHandler.GameName);

            MelonFolderHandler.ScanForFolders();
            MelonFolderHandler.LoadMelons(MelonFolderHandler.ScanType.UserLibs);
            MelonFolderHandler.LoadMelons(MelonFolderHandler.ScanType.Plugins);

            MelonEvents.MelonHarmonyEarlyInit.Invoke();
            MelonEvents.OnPreInitialization.Invoke();

#if !NET6_0_OR_GREATER
            // Set up the Core.Start entrypoint which harmony patches the main Unity assembly as soon as possible and
            // unpatch it in our hooking method before calling the Core.Start method
            MonoCoreEntrypoint.Init();
#endif

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
            MelonFolderHandler.LoadMelons(MelonFolderHandler.ScanType.Mods);

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
                             $"v{Properties.BuildInfo.Version} " +
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

            foreach (var prefFile in MelonPreferences.PrefFiles)
                prefFile.FileWatcher.Destroy();
            MelonPreferences.DefaultFile.FileWatcher.Destroy();
            MelonPreferences.Save();

            HarmonyInstance.UnpatchSelf();
            bHapticsManager.Disconnect();

#if NET6_0_OR_GREATER
            Fixes.Il2CppInterop.Il2CppInteropFixes.Shutdown();
            Fixes.Il2CppInterop.Il2CppICallInjector.Shutdown();
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
}
