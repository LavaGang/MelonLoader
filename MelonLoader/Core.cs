using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
#pragma warning disable 0618

namespace MelonLoader
{
    internal static class Core
    {
        internal static string UserDataPath = null;

        static Core()
        {
            ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = MelonUtils.GetGameDirectory();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            UserDataPath = Path.Combine(MelonUtils.GetGameDirectory(), "UserData");
            if (!Directory.Exists(UserDataPath))
                Directory.CreateDirectory(UserDataPath);
            MelonPrefs.Setup();
            MelonPreferences.Load();
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
            MelonHandler.LoadMods();
            Main.LegacySupport();
            if ((MelonHandler._Plugins.Count > 0) || (MelonHandler._Mods.Count > 0))
                AddUnityDebugLog();
            MelonHandler.OnApplicationStart();
        }

        internal static void Quit()
        {
            MelonHandler.OnApplicationQuit();
            MelonHandler.SaveConfig();
            Harmony.HarmonyInstance.UnpatchAllInstances();
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
            if (args.Name.StartsWith("MelonLoader.ModHandler, Version=") || args.Name.StartsWith("MelonLoader, Version="))
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static bool QuitFix();
    }
}
