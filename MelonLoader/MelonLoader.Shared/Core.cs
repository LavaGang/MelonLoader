using MelonLoader.Fixes;
using MelonLoader.Utils;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    public class Core
    {
        public static void Startup(string engineModulePath)
        {
            MelonEnvironment.Initialize();
            MelonDebug.Msg("MelonLoader.Core.Startup");

            UnhandledException.Install(AppDomain.CurrentDomain);
            UnhandledAssemblyResolve.Install();

            if (engineModulePath != null)
            {
                MelonDebug.Msg($"Engine Module Path: {engineModulePath}");
                var module = ModuleManager.LoadModule(engineModulePath);
                module.Initialize();
            }
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