using MelonLoader.Fixes;
using MelonLoader.Utils;
using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader.Interfaces;
using MelonLoader.Melons;
using MelonLoader.Properties;
using System.Drawing;

namespace MelonLoader
{
    public static class Core
    {
        internal static Harmony HarmonyInstance;

        public static void Startup(string engineModulePath)
        {
            HarmonyInstance = new Harmony(BuildInfo.Name);

            MelonUtils.Setup();
            MelonEnvironment.Initialize();
            MelonLaunchOptions.Load();
            
            if (MelonUtils.IsWineOrProton)
                Pastel.ConsoleExtensions.Disable();

            UnhandledException.Install(AppDomain.CurrentDomain);
            UnhandledAssemblyResolve.Install();

            if (!string.IsNullOrEmpty(engineModulePath))
            {
                MelonDebug.Msg($"Engine Module Path: {engineModulePath}");
                IEngineModule module = ModuleManager.LoadModule(engineModulePath);
                module.Initialize();
            }

            WelcomeMessage();

            // Test
            MelonHandler.LoadMelonsFromAssembly<MelonPlugin>(Assembly.GetExecutingAssembly());
        }

        private static void WelcomeMessage()
        {
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"{BuildInfo.Name} v{BuildInfo.Version}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"OS: {MelonUtils.OSVersion}");
            MelonLogger.MsgDirect($"Hash Code: {MelonUtils.HashCode}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"Runtime: {((ModuleManager.EngineModule != null) ? ModuleManager.EngineModule.GameInfo.RuntimeName : MelonEnvironment.OurRuntimeName)}");
            MelonLogger.MsgDirect($"Arch: {(MelonUtils.Is32Bit ? "x86" : "x86_64")}");
            MelonLogger.MsgDirect("------------------------------");

            MelonEnvironment.PrintEnvironment();

            if (ModuleManager.EngineModule != null)
            {
                EngineModuleInfo engineModuleInfo = ModuleManager.EngineModule.GameInfo;

                MelonLogger.WriteLine(Color.Magenta);
                MelonLogger.Msg($"Game Name: {engineModuleInfo.GameName}");
                MelonLogger.Msg($"Game Developer: {engineModuleInfo.GameDeveloper}");
                MelonLogger.Msg($"Engine Name: {engineModuleInfo.EngineName}");
                MelonLogger.Msg($"Engine Version: {engineModuleInfo.EngineVersion}");
                MelonLogger.Msg($"Game Version: {engineModuleInfo.GameVersion}");
                MelonLogger.WriteLine(Color.Magenta);
                MelonLogger.WriteSpacer();
            }
        }

        public static void OnApplicationPreStart()
        {
            MelonEvents.InternalInvokeOnApplicationPreStart();
        }

        public static void OnApplicationStart()
        {
            MelonEvents.InternalInvokeOnApplicationStart();
        }
    }
}