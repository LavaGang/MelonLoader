using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using Boardgame.Modding;
using RGCommon;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonCompatibilityLayer.Module
    {
        private static string ConnectionString;
        private static Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation> ModInformation = new Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation>();

        public override void Setup()
        {
            MonoInternals.MonoResolveManager.GetAssemblyResolveInfo("Demeo").Override = typeof(Demeo_Module).Assembly;

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");

            harmony.Patch(typeof(ModdingAPI).GetMethod("GetInstalledMods", BindingFlags.Public | BindingFlags.Instance),
                typeof(Demeo_Module).GetMethod("GetInstalledMods", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            harmony.Patch(typeof(GameStateMachine).GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("GetConnectionString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            MelonCompatibilityLayer.AddRefreshPluginsEvent(Refresh);
            MelonCompatibilityLayer.AddRefreshModsEvent(Refresh);
            Refresh();
        }

        private static void Refresh()
        {
            ConnectionString = VersionInformation.MajorMinorVersion;
            ModInformation.Clear();
            ParseMelons(MelonHandler.Plugins);
            ParseMelons(MelonHandler.Mods);
        }

        private static void ParseMelons<T>(List<T> melons) where T : MelonBase
        {
            if (melons.Count <= 0)
                return;

            for (int i = 0; i < melons.Count; i++)
            {
                T melon = melons[i];
                if (melon == null)
                    continue;

                ModInformation.Add(new ModdingAPI.ModInformation()
                {
                    name = melon.Info.Name,
                    version = melon.Info.Version,
                    author = melon.Info.Author,
                    description = melon.Info.DownloadLink
                });

                if (MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(melon.Assembly) != null)
                    ConnectionString += $", {melon.Info.Name} v{melon.Info.Version}";
            }

            //ModInformation.Sort();
        }

        private static bool GetInstalledMods(ref Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation> __result) { __result = ModInformation; return false; }
        private static bool GetConnectionString(ref string __result) { __result = ConnectionString; return false; }
    }
}