using MelonLoader.Fixes;
using MelonLoader.Utils;
using System;
using System.Text;
using HarmonyLib;
using MelonLoader.Interfaces;
using MelonLoader.Properties;

namespace MelonLoader
{
    public class Core
    {
        internal static Harmony HarmonyInstance;
        public static readonly bool IsAlpha = true;
        public static void Startup(string engineModulePath)
        {
            OsUtils.SetupWineCheck();
            MelonEnvironment.Initialize();
            MelonLaunchOptions.Load();
            
            if (MelonUtils.IsUnderWineOrSteamProton()) 
                Pastel.ConsoleExtensions.Disable();
            
            MelonDebug.Msg("MelonLoader.Core.Startup");

            UnhandledException.Install(AppDomain.CurrentDomain);
            UnhandledAssemblyResolve.Install();

            if (engineModulePath == null) 
                return;
            
            HarmonyInstance = new Harmony(BuildInfo.Name);
            
            MelonDebug.Msg($"Engine Module Path: {engineModulePath}");
            IEngineModule module = ModuleManager.LoadModule(engineModulePath);
            module.Initialize();
            
            MelonUtils.Setup(AppDomain.CurrentDomain);
        }

        public static void OnApplicationPreStart()
        {
            MelonDebug.Msg("MelonLoader.Core.OnApplicationPreStart");
        }

        public static void OnApplicationStart()
        {
            MelonDebug.Msg("MelonLoader.Core.OnApplicationStart");
        }
    }
}