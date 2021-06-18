﻿using System;

namespace MelonLoader
{
    internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;

        static Core()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            Fixes.UnhandledException.Run(curDomain);
            Fixes.InvariantCurrentCulture.Install();

            try { MelonUtils.Setup(); } catch (Exception ex) { MelonLogger.Error("MelonUtils.Setup Exception: " + ex.ToString()); throw ex; }

            Fixes.ApplicationBase.Run(curDomain);
            Fixes.ExtraCleanup.Run();

            MelonPreferences.Load();
            MelonLaunchOptions.Load();
            MelonCompatibilityLayer.Setup(curDomain);
#if !__ANDROID__
            PatchShield.Install();
#endif
        }

        private static int Initialize()
        {
            Il2CppAssemblyGenerator.Load();
            // bHaptics.Load();

            MelonHandler.LoadPlugins();
            MelonHandler.OnPreInitialization();

            return 0;
        }

        private static int PreStart()
        {
            MelonHandler.OnApplicationEarlyStart();

            if (!Il2CppAssemblyGenerator.Run())
                return 1;

            if (MelonUtils.IsGameIl2Cpp())
                HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyIl2CppMethodPatcher.TryResolve;

            return 0;
        }

        private static int Start()
        {
            if (!SupportModule.Initialize())
                return 1;

            AddUnityDebugLog();
            // bHaptics.Start();

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
            Fixes.QuitFix.Run();
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }
    }
}