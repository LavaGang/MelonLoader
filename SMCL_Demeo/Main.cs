using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using Boardgame.Modding;
#pragma warning disable 0618

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
        }

        public override void RefreshMods()
        {
            ModInformation.Clear();
            MelonMod[] mods = MelonHandler.Mods.ToArray();
            if (mods.Length <= 0)
                return;
            for (int i = 0; i < mods.Length; i++)
            {
                MelonMod mod = mods[i];
                if (mod == null)
                    continue;
                ModInformation.Add(new ModdingAPI.ModInformation()
                {
                    name = mod.Info.Name,
                    version = mod.Info.Version,
                    author = mod.Info.Author,
                    description = mod.Info.DownloadLink
                });
            }
        }

        private static bool GetInstalledMods(ref List<ModdingAPI.ModInformation> __result) { __result = ModInformation; return false; }
    }
}