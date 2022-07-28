using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using MelonLoader.InternalUtils;
using MelonLoader.MonoInternals;
using MelonLoader.Utils;
using System.IO;
using System.Runtime.InteropServices;

namespace MelonLoader
{
	internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;
        internal static bool Is_ALPHA_PreRelease = false;

        internal static NativeLibrary.StringDelegate WineGetVersion;

        internal static int Initialize()
        {
            MelonLaunchOptions.Load();

            if (MelonLaunchOptions.Core.UserWantsDebugger && MelonEnvironment.IsDotnetRuntime)
            {
                Console.WriteLine("[Init] User requested debugger, attempting to launch now...");
                Debugger.Launch();
            }

            MelonEnvironment.MelonLoaderDirectory = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
            MelonEnvironment.GameRootDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            SetupWineCheck();

            if (MelonUtils.IsUnderWineOrSteamProton())
                Pastel.ConsoleExtensions.Disable();

            ManagedAnalyticsBlocker.Install();

            AppDomain curDomain = AppDomain.CurrentDomain;
            Fixes.DotnetLoadFromManagedFolderFix.Install();
            Fixes.UnhandledException.Install(curDomain);
            Fixes.ServerCertificateValidation.Install();

            MelonUtils.Setup(curDomain);

            Assertions.LemonAssertMapping.Setup();
            MelonHandler.Setup();

            try
            {
                if (!MonoLibrary.Setup()
                    || !MonoResolveManager.Setup())
                    return 1;
            } catch(SecurityException)
            {
                MelonDebug.Msg("[MonoLibrary] Caught SecurityException, assuming not running under mono and continuing with init");
            }

            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

#if NET6_0
            NativeStackWalk.DumpStack();
            Fixes.DotnetAssemblyLoadContextFix.Install();
            Fixes.DotnetModHandlerRedirectionFix.Install();
#endif
            Fixes.ForcedCultureInfo.Install();
            Fixes.InstancePatchFix.Install();
            Fixes.ProcessFix.Install();
            PatchShield.Install();

            MelonPreferences.Load();
            bHaptics.Load();

            MelonCompatibilityLayer.LoadModules();

            MelonHandler.LoadMelonsFromDirectory<MelonPlugin>(MelonHandler.PluginsDirectory);
            MelonEvents.OnPreInitialization.Invoke();

            return 0;
        }

        internal static int PreStart()
        {
            MelonEvents.OnApplicationEarlyStart.Invoke();
            return MelonStartScreen.LoadAndRun(Il2CppGameSetup);
        }

        private static int Il2CppGameSetup()
            => Il2CppAssemblyGenerator.Run() ? 0 : 1;

        internal static int Start()
        {
            bHaptics.Start();

            MelonEvents.OnPreSupportModule.Invoke();

            if (!SupportModule.Setup())
                return 1;

            MelonEvents.OnPreApplicationStart.Invoke(); // Modules should subscribe to this with the lowest priority, preferably int.MinValue

            AddUnityDebugLog();

            RegisterTypeInIl2Cpp.SetReady();

            MelonHandler.LoadMelonsFromDirectory<MelonMod>(MelonHandler.ModsDirectory);

            MelonEvents.OnApplicationStart.Invoke();
            //MelonStartScreen.DisplayModLoadIssuesIfNeeded();

            return 0;
        }

        internal static string GetVersionString()
        {
            var lemon = MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON;
            var versionStr = $"{(lemon ? "Lemon" : "Melon")}Loader " +
                $"v{BuildInfo.Version} " +
                $"{(Is_ALPHA_PreRelease ? "ALPHA Pre-Release" : "Open-Beta")}";
            return versionStr;
        }

        internal static void WelcomeMessage()
        {
            //if (MelonDebug.IsEnabled())
            //    MelonLogger.WriteSpacer();

            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg(GetVersionString());
            MelonLogger.Msg($"OS: {GetOSVersion()}");
            MelonLogger.Msg($"Hash Code: {MelonUtils.HashCode}");
            MelonLogger.Msg("------------------------------");
            var typeString = MelonUtils.IsGameIl2Cpp() ? "Il2cpp" : MelonUtils.IsOldMono() ? "Mono" : "MonoBleedingEdge";
            MelonLogger.Msg($"Game Type: {typeString}");
            var archString = MelonUtils.IsGame32Bit() ? "x86" : "x64";
            MelonLogger.Msg($"Game Arch: {archString}");
            MelonLogger.Msg("------------------------------");

            MelonEnvironment.PrintEnvironment();
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        internal static extern uint RtlGetVersion(out OsVersionInfo versionInformation); // return type should be the NtStatus enum

        [StructLayout(LayoutKind.Sequential)]
        internal struct OsVersionInfo
        {
            private readonly uint OsVersionInfoSize;

            internal readonly uint MajorVersion;
            internal readonly uint MinorVersion;

            internal readonly uint BuildNumber;

            private readonly uint PlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal readonly string CSDVersion;
        }

        internal static string GetOSVersion()
        {
            if (MelonUtils.IsUnderWineOrSteamProton())
                return $"Wine {WineGetVersion()}";
            RtlGetVersion(out OsVersionInfo versionInformation);
            var minor = versionInformation.MinorVersion;
            var build = versionInformation.BuildNumber;

            string versionString = "";

            switch (versionInformation.MajorVersion)
            {
                case 4:
                    versionString = "Windows 95/98/Me/NT";
                    break;
                case 5:
                    if (minor == 0)
                        versionString = "Windows 2000";
                    if (minor == 1)
                        versionString = "Windows XP";
                    if (minor == 2)
                        versionString = "Windows 2003";
                    break;
                case 6:
                    if (minor == 0)
                        versionString = "Windows Vista";
                    if (minor == 1)
                        versionString = "Windows 7";
                    if (minor == 2)
                        versionString = "Windows 8";
                    if (minor == 3)
                        versionString = "Windows 8.1";
                    break;
                case 10:
                    if (build >= 22000)
                        versionString = "Windows 11";
                    else
                        versionString = "Windows 10";
                    break;
                default:
                    versionString = "Unknown";
                    break;
            }

            return $"{versionString}";
        }

        internal static void OnApplicationLateStart()
        {
            MelonEvents.OnApplicationLateStart.Invoke();
        }

        internal static void Quit()
        {
            MelonDebug.Msg("[ML Core] Received Quit from Support Module. Shutting down...");

            MelonEvents.OnApplicationQuit.Invoke();
            MelonPreferences.Save();

            HarmonyInstance.UnpatchSelf();
            bHaptics.Quit();

            MelonLogger.Flush();
            MelonLogger.Close();

            System.Threading.Thread.Sleep(200);

            if (MelonLaunchOptions.Core.QuitFix)
                Process.GetCurrentProcess().Kill();
        }

        private static void SetupWineCheck()
        {
            IntPtr dll = NativeLibrary.LoadLibrary("ntdll.dll");
            IntPtr wine_get_version_proc = NativeLibrary.GetProcAddress(dll, "wine_get_version");
            if (wine_get_version_proc == IntPtr.Zero)
                return;

            WineGetVersion = (NativeLibrary.StringDelegate)Marshal.GetDelegateForFunctionPointer(
                wine_get_version_proc, 
                typeof(NativeLibrary.StringDelegate)
                );
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }
    }
}