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
            if (MelonUtils.IsGameIl2Cpp())
                HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyIl2CppMethodPatcher.TryResolve;
            HarmonyShield.Install();
            HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony("MelonLoader");
            HarmonyLib.HarmonyMethod GetCurrentCulturePrefixHarmonyMethod = new HarmonyLib.HarmonyMethod(typeof(Core).GetMethod("GetCurrentCulturePrefix", BindingFlags.NonPublic | BindingFlags.Static));
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentCulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), GetCurrentCulturePrefixHarmonyMethod); } catch (Exception ex) { MelonLogger.Warning("Thread.CurrentCulture Exception: " + ex.ToString()); }
            try { harmonyInstance.Patch(typeof(Thread).GetProperty("CurrentUICulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), GetCurrentCulturePrefixHarmonyMethod); } catch (Exception ex) { MelonLogger.Warning("Thread.CurrentUICulture Exception: " + ex.ToString()); }
            try { ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = MelonUtils.GameDirectory; } catch (Exception ex) { MelonLogger.Warning("AppDomainSetup.ApplicationBase Exception: " + ex.ToString()); }
            Directory.SetCurrentDirectory(MelonUtils.GameDirectory);
            try { ExtraCleanupCheck(MelonUtils.GameDirectory); } catch (Exception ex) { MelonLogger.Warning("Core.ExtraCleanupCheck Exception: " + ex.ToString()); }
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            MelonCompatibilityLayer.AddAssemblyResolvers();
        }

        private static void Initialize()
        {
            try { bHaptics_NativeLibrary.Load(); } catch (Exception ex) { MelonLogger.Warning("bHaptics_NativeLibrary.Load Exception: " + ex.ToString()); bHaptics.WasError = true; }
            MelonPreferences.Load();
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
            MelonPreferences.Save();
            //Harmony.HarmonyInstance.UnpatchAllInstances();
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
                || args.Name.StartsWith("0Harmony, Version=")
                || args.Name.StartsWith("Mono.Cecil, Version=")
                || args.Name.StartsWith("Mono.Cecil.Mdb, Version=")
                || args.Name.StartsWith("Mono.Cecil.Pdb, Version=")
                || args.Name.StartsWith("Mono.Cecil.Rocks, Version=")
                || args.Name.StartsWith("MonoMod.RuntimeDetour, Version=")
                || args.Name.StartsWith("MonoMod.Utils, Version="))
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