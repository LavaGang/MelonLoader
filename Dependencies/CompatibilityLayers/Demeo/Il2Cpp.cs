using System.Reflection;
using MelonLoader;
using Boardgame.Modding;

namespace MelonLoader.CompatibilityLayers
{
    internal static class Il2Cpp
    {
        internal static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(typeof(ModdingAPI).GetMethod("GetInstalledMods", BindingFlags.Public | BindingFlags.Instance),
                typeof(Il2Cpp).GetMethod("GetInstalledMods", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
        }

        private static bool GetInstalledMods(ref Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation> __result)
        {
            Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation> info = new Il2CppSystem.Collections.Generic.List<ModdingAPI.ModInformation>();
            Demeo_Module.ModInformation.ForEach(x => info.Add(x));
            __result = info;
            return false;
        }
    }
}
