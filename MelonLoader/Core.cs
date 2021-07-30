using MelonLoader.InternalUtils;
using System;
using System.Diagnostics;
using MelonLoader.Attributes;
using MelonLoader.CompatibilityLayers;
using MelonLoader.Melons;
using MelonLoader.Preferences;
using MelonLoader.Utils;

namespace MelonLoader
{
	internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;

        private static int Initialize()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            Fixes.UnhandledException.Install(curDomain);
            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            Fixes.ForcedCultureInfo.Install();
            MelonUtils.Setup(curDomain);
            Fixes.ExtraCleanup.Run();

            MelonPreferences.Load();
            MelonLaunchOptions.Load();
            MelonCompatibilityLayer.Setup();

            PatchShield.Install();

            bHaptics.Load();

            MelonCompatibilityLayer.SetupModules(MelonCompatibilityLayer.SetupType.OnPreInitialization);

            MelonHandler.LoadPlugins();
            MelonHandler.OnPreInitialization();

            return 0;
        }

        private static int PreStart()
        {
            if (!MelonUtils.IsGameIl2Cpp())
                GameVersionHandler.Setup();

            MelonHandler.OnApplicationEarlyStart();

            if (MelonUtils.IsGameIl2Cpp())
            {
                if (!Il2CppAssemblyGenerator.Run())
                    return 1;
                
                HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyIl2CppMethodPatcher.TryResolve;

                GameVersionHandler.Setup();
            }

            return 0;
        }

        private static int Start()
        {
            if (!SupportModule.SupportModule.Initialize())
                return 1;

            AddUnityDebugLog();
            bHaptics.Start();

            MelonCompatibilityLayer.SetupModules(MelonCompatibilityLayer.SetupType.OnApplicationStart);
            MelonHandler.OnApplicationStart_Plugins();
            MelonHandler.LoadMods();
            MelonHandler.OnApplicationStart_Mods();

            MelonHandler.OnApplicationLateStart_Plugins();
            MelonHandler.OnApplicationLateStart_Mods();

            return 0;
        }

        internal static void Quit()
        {
            MelonHandler.OnApplicationQuit();
            MelonPreferences.Save();

            HarmonyInstance.UnpatchAll();
            bHaptics.Quit();

            MelonLogger.Flush();

            if (MelonLaunchOptions.Core.QuitFix)
                Process.GetCurrentProcess().Kill();
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }
    }
}