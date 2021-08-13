using System;
using System.Diagnostics;
using MelonLoader.InternalUtils;
using MelonLoader.MonoInternals;
using MonoMod.RuntimeDetour;

namespace MelonLoader
{
	internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance = null;

        private static int Initialize()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;

            Fixes.UnhandledException.Install(curDomain);

            if (!MonoLibrary.Setup()
                || !MonoAssemblyResolveManager.Setup())
                return 1;

            // Custom AssemblyResolve events to be Removed Later
            curDomain.AssemblyResolve += MelonCompatibilityLayer.AssemblyResolve;
            curDomain.AssemblyResolve += CompatibilityLayers.Melon_Resolver.AssemblyResolve;

            MelonUtils.Setup(curDomain);
            Fixes.ExtraCleanup.Run();

            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            Fixes.ForcedCultureInfo.Install();
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
            if (!SupportModule.Initialize())
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
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.Interface.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.Interface.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }
    }
}