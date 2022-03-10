using System;
using System.Diagnostics;
using MelonLoader.InternalUtils;
using MelonLoader.MonoInternals;

namespace MelonLoader
{
	internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;

        private static int Initialize()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            Fixes.UnhandledException.Install(curDomain);
            Fixes.ServerCertificateValidation.Install();

            MelonUtils.Setup(curDomain);
            Assertions.LemonAssertMapping.Setup();
            MelonHandler.Setup();

            if (!MonoLibrary.Setup()
                || !MonoResolveManager.Setup())
                return 1;

            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            Fixes.ForcedCultureInfo.Install();
            Fixes.InstancePatchFix.Install();
            Fixes.ProcessFix.Install();
            PatchShield.Install();

            MelonPreferences.Load();
            MelonLaunchOptions.Load();
            bHaptics.Load();

            MelonCompatibilityLayer.Setup();
            MelonCompatibilityLayer.SetupModules(MelonCompatibilityLayer.SetupType.OnPreInitialization);

            MelonHandler.LoadPlugins();
            MelonHandler.OnPreInitialization();

            return 0;
        }

        private static int PreStart()
        {
            MelonHandler.OnApplicationEarlyStart();
            return MelonStartScreen.LoadAndRun(Il2CppGameSetup);
        }

        private static int Il2CppGameSetup()
            => (MelonUtils.IsGameIl2Cpp()
                && !Il2CppAssemblyGenerator.Run())
                ? 1 : 0;

        private static int Start()
        {
            bHaptics.Start();

            MelonHandler.OnApplicationStart_Plugins();
            MelonHandler.LoadMods();
            MelonHandler.OnPreSupportModule();

            if (!SupportModule.Setup())
                return 1;

            MelonCompatibilityLayer.SetupModules(MelonCompatibilityLayer.SetupType.OnApplicationStart);
            AddUnityDebugLog();
            MelonHandler.OnApplicationStart_Mods();
            //MelonStartScreen.DisplayModLoadIssuesIfNeeded();

            return 0;
        }

        internal static void OnApplicationLateStart()
        {
            MelonHandler.OnApplicationLateStart_Plugins();
            MelonHandler.OnApplicationLateStart_Mods();
            MelonStartScreen.Finish();
        }

        internal static void Quit()
        {
            MelonHandler.OnApplicationQuit();
            MelonPreferences.Save();

            HarmonyInstance.UnpatchSelf();
            bHaptics.Quit();

            MelonLogger.Flush();

            if (MelonLaunchOptions.Core.QuitFix)
                Process.GetCurrentProcess().Kill();
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }
    }
}