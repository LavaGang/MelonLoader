using System;
using System.Diagnostics;
using MelonLoader.InternalUtils;
using MelonLoader.MonoInternals;

namespace MelonLoader
{
	public static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;

        private static int Initialize()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            Fixes.UnhandledException.Install(curDomain);

            MelonUtils.Setup(curDomain);
            Assertions.LemonAssertMapping.Setup();

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

            MelonHandler.LoadMelonsFromDirectory<MelonPlugin>(MelonHandler.PluginsDirectory);
            MelonEvents.OnPreInitialization.Invoke();
            MelonHandler.OnPreInitialization(); // Remove this from here

            return 0;
        }

        private static int PreStart()
        {
            MelonEvents.OnApplicationEarlyStart.Invoke();
            MelonHandler.OnApplicationEarlyStart(); // Remove this from here
            return MelonStartScreen.LoadAndRun(Il2CppGameSetup);
        }

        private static int Il2CppGameSetup()
            => (MelonUtils.IsGameIl2Cpp()
                && !Il2CppAssemblyGenerator.Run())
                ? 1 : 0;

        private static int Start()
        {
            bHaptics.Start();

            MelonEvents.OnPreSupportModule.Invoke();

            if (!SupportModule.Setup())
                return 1;

            if (MelonUtils.IsGameIl2Cpp())
                HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyIl2CppMethodPatcher.TryResolve;
            MelonCompatibilityLayer.SetupModules(MelonCompatibilityLayer.SetupType.OnApplicationStart);
            AddUnityDebugLog();

            RegisterTypeInIl2Cpp.SetReady();

            MelonHandler.LoadMelonsFromDirectory<MelonMod>(MelonHandler.ModsDirectory);

            MelonHandler.OnPreSupportModule(); // Remove this from here
            MelonEvents.OnApplicationStart.Invoke();
            MelonHandler.OnApplicationStart_Plugins(); // Remove this from here
            MelonHandler.OnApplicationStart_Mods(); // Remove this from here
            //MelonStartScreen.DisplayModLoadIssuesIfNeeded();

            return 0;
        }

        internal static void OnApplicationLateStart()
        {
            MelonEvents.OnApplicationLateStart.Invoke();
            MelonHandler.OnApplicationLateStart_Plugins(); // Remove this from here
            MelonHandler.OnApplicationLateStart_Mods(); // Remove this from here
            MelonStartScreen.Finish();
        }

        internal static void Quit()
        {
            MelonEvents.OnApplicationQuit.Invoke();
            MelonHandler.OnApplicationQuit(); // Remove this from here
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