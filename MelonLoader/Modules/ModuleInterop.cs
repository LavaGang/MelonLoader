using MelonLoader.Utils;
using System;

namespace MelonLoader.Modules
{
    internal static class ModuleInterop
    {
        internal static MelonEngineModule Engine { get; private set; }
        internal static MelonSupportModule Support { get; private set; }

        internal static void StartEngine()
        {
            Engine = ModuleManager.GetEngine();
            if (Engine == null)
            {
                MelonLogger.Warning("No Engine Module Found! Using Fallback Environment...");
                MelonEnvironment.PrintAppInfo();
                Stage2();
                Stage3(null);
                return;
            }

            MelonLogger.Msg($"Engine Module Found: {Engine.GetType().Assembly.Location}");
            Engine.Initialize();
        }

        internal static void StartSupport(string path)
        {
            Support = ModuleManager.GetSupport(path);
            if (Support == null)
                return;

            MelonLogger.Msg($"Support Module Found: {path}");
            Support.Initialize();
        }

        public static void Stage2()
        {
            try
            {
                Core.Stage2();
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to run Stage2 of MelonLoader");
                MelonLogger.Error(ex);
                throw new("Error at Stage2");
            }
        }

        public static void Stage3(string supportModulePath)
        {
            try
            {
                Core.Stage3(supportModulePath);
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to run Stage3 of MelonLoader");
                MelonLogger.Error(ex);
                throw new("Error at Stage3");
            }
        }
    }
}
