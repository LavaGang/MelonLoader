using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using Boardgame.Modding;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonCompatibilityLayer.Module
    {
        private static List<ModdingAPI.ModInformation> ModInformation = new List<ModdingAPI.ModInformation>();

        public override void Setup()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");
            harmony.Patch(typeof(ModdingAPI).GetMethod("GetInstalledMods", BindingFlags.Public | BindingFlags.Instance),
                typeof(Demeo_Module).GetMethod("GetInstalledMods", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            MelonCompatibilityLayer.AddRefreshPluginsEvent(Refresh);
            MelonCompatibilityLayer.AddRefreshModsEvent(Refresh);
            Refresh();
        }

        private static void Refresh()
        {
            ModInformation.Clear();
            ParseMelons(MelonHandler.Plugins);
            ParseMelons(MelonHandler.Mods);
        }

        private static void ParseMelons<T>(List<T> melons) where T : MelonBase
        {
            if (melons.Count <= 0)
                return;

            melons.Sort((T left, T right) => string.Compare(left.Info.Name, right.Info.Name));

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
            }
        }

        private static bool GetInstalledMods(ref List<ModdingAPI.ModInformation> __result) { __result = ModInformation; return false; }
    }
}