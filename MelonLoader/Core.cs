using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
#pragma warning disable 0618

namespace MelonLoader
{
    internal static class Core
    {
        internal static string GameDir = null;
        internal static string UserDataPath = null;

        static Core()
        {
            GameDir = MelonUtils.GetGameDirectory();
            ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = GameDir;
            Directory.SetCurrentDirectory(GameDir);
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolveHandler;
            CurrentCultureFix();
            UserDataPath = Path.Combine(GameDir, "UserData");
            if (!Directory.Exists(UserDataPath))
                Directory.CreateDirectory(UserDataPath);
            try { MelonPreferences.LegacyCheck(); } catch (Exception ex) { MelonLogger.Error("MelonPreferences.LegacyCheck Exception: " + ex.ToString()); throw ex; }
            try { MelonPreferences.Load(); } catch (Exception ex) { MelonLogger.Error("MelonPreferences.Load Exception: " + ex.ToString()); throw ex; }
            if (MelonPreferences.WasLegacyLoaded) try { MelonPreferences.Save(); } catch (Exception ex) { MelonLogger.Error("MelonPreferences.Save Exception: " + ex.ToString()); throw ex; } 
        }

        private static void Initialize()
        {
            MelonHandler.LoadPlugins();
            Main.LegacySupport();
            MelonHandler.OnPreInitialization();
        }

        private static void Start()
        {
            if (!SupportModule.Initialize())
                return;
            AddUnityDebugLog();
            MelonHandler.LoadMods();
            Main.LegacySupport();
            MelonHandler.OnApplicationStart();
        }

        internal static void Quit()
        {
            MelonHandler.OnApplicationQuit();
            MelonPreferences.Save();
            Harmony.HarmonyInstance.UnpatchAllInstances();
            MelonLogger.Flush();
            if (QuitFix())
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) => MelonLogger.Error((e.ExceptionObject as Exception).ToString());

        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("MelonLoader.ModHandler, Version=") || args.Name.StartsWith("MelonLoader, Version=") || args.Name.StartsWith("0Harmony, Version="))
                return typeof(Core).Assembly;
            string assembly_name = args.Name.Split(',')[0];
            string dll_name = (assembly_name + ".dll");
            string plugins_path = Path.Combine(MelonHandler.PluginsDirectory, dll_name);
            if (File.Exists(plugins_path))
                return Assembly.LoadFile(plugins_path);
            string mods_path = Path.Combine(MelonHandler.ModsDirectory, dll_name);
            if (File.Exists(mods_path))
                return Assembly.LoadFile(mods_path);
            return null;
        }

        private static bool GetCurrentCulturePrefix(ref CultureInfo __result) { __result = CultureInfo.InvariantCulture; return false; }
        private static void CurrentCultureFix()
        {
            Harmony.HarmonyInstance harmonyInstance = Harmony.HarmonyInstance.Create("CurrentCultureFix");
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentCulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), new Harmony.HarmonyMethod(typeof(Core).GetMethod("GetCurrentCulturePrefix", BindingFlags.NonPublic | BindingFlags.Static))); } catch (Exception ex) {}
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentUICulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), new Harmony.HarmonyMethod(typeof(Core).GetMethod("GetCurrentCulturePrefix", BindingFlags.NonPublic | BindingFlags.Static))); } catch (Exception ex) {}
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static bool QuitFix();
    }
}