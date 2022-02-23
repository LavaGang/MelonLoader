using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using Boardgame.Modding;

namespace MelonLoader.CompatibilityLayers
{
    internal static class Mono
    {
        internal static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(typeof(ModdingAPI).GetMethod("GetInstalledMods", BindingFlags.Public | BindingFlags.Instance),
                typeof(Mono).GetMethod("GetInstalledMods", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
        }

        private static bool GetInstalledMods(ref List<ModdingAPI.ModInformation> __result) { __result = Demeo_Module.ModInformation; return false; }
    }
}
