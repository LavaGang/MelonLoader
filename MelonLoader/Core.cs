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
        static Core()
        {
            try { MelonUtils.Setup(); } catch (Exception ex) { MelonLogger.Error("MelonUtils.Setup Exception: " + ex.ToString()); throw ex; }
            Harmony.HarmonyInstance harmonyInstance = Harmony.HarmonyInstance.Create("MelonLoader");
            Harmony.HarmonyMethod GetCurrentCulturePrefixHarmonyMethod = new Harmony.HarmonyMethod(typeof(Core).GetMethod("GetCurrentCulturePrefix", BindingFlags.NonPublic | BindingFlags.Static));
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentCulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), GetCurrentCulturePrefixHarmonyMethod); } catch (Exception ex) { MelonLogger.Warning("Thread.CurrentCulture Exception: " + ex.ToString()); }
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentUICulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), GetCurrentCulturePrefixHarmonyMethod); } catch (Exception ex) { MelonLogger.Warning("Thread.CurrentUICulture Exception: " + ex.ToString()); }
            try { ExtraCleanupCheck(MelonUtils.GameDirectory); } catch { }
            try { OverrideAppDomainBase(MelonUtils.GameDirectory); } catch { }
            Directory.SetCurrentDirectory(MelonUtils.GameDirectory);
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            MelonPreferences.Load();
            try { bHaptics_NativeLibrary.Load(); } catch (Exception ex) { MelonLogger.Warning("bHaptics_NativeLibrary.Load Exception: " + ex.ToString()); bHaptics.WasError = true; }
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
            try { bHaptics.Start(); } catch (Exception ex) { MelonLogger.Warning("bHaptics.Start Exception: " + ex.ToString()); bHaptics.WasError = true; }
            MelonHandler.OnApplicationStart_Plugins();
            MelonHandler.LoadMods();
            Main.LegacySupport();
            MelonHandler.OnApplicationStart_Mods();
            MelonHandler.OnApplicationLateStart_Plugins();
            MelonHandler.OnApplicationLateStart_Mods();
        }

        internal static void Quit()
        {
            MelonHandler.OnApplicationQuit();
            Preferences.IO.Watcher.Destroy();
            MelonPreferences.Save();
            Harmony.HarmonyInstance.UnpatchAllInstances();
            try { bHaptics.Quit(); } catch (Exception ex) { MelonLogger.Warning("bHaptics.Quit Exception: " + ex.ToString()); bHaptics.WasError = true; }
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

        private static void OverrideAppDomainBase(string basepath)
        {
            var property = typeof(AppDomain).GetProperty("FusionStore", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
                return;
            AppDomainSetup appDomainBase = ((AppDomainSetup)property.GetValue(AppDomain.CurrentDomain, new object[0]));
            appDomainBase.ApplicationBase = basepath;
        }

        private static void ExtraCleanupCheck(string destination)
        {
            ExtraCleanupCheck2(destination);
            ExtraCleanupCheck2(MelonHandler.ModsDirectory);
            ExtraCleanupCheck2(MelonHandler.PluginsDirectory);
            ExtraCleanupCheck2(MelonUtils.UserDataDirectory);
        }
        private static void ExtraCleanupCheck2(string destination)
        {
            string main_dll = Path.Combine(destination, "MelonLoader.dll");
            if (File.Exists(main_dll))
                File.Delete(main_dll);
            string main2_dll = Path.Combine(destination, "MelonLoader.ModHandler.dll");
            if (File.Exists(main2_dll))
                File.Delete(main2_dll);
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) => MelonLogger.Error((e.ExceptionObject as Exception).ToString());

        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("MelonLoader.ModHandler, Version=")
                || args.Name.StartsWith("MelonLoader, Version=")
                || args.Name.StartsWith("0Harmony, Version="))
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static bool QuitFix();
    }
}