using System.Collections.Generic;
using System.Reflection;
using Boardgame.Modding;
using MelonLoader;
using RGCommon;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonCompatibilityLayer.Module
    {
        internal static string ConnectionString;
        internal static List<ModdingAPI.ModInformation> ModInformation = new List<ModdingAPI.ModInformation>();

        public override void Setup()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");

            harmony.Patch(typeof(GameStateMachine).GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("GetConnectionString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            if (MelonUtils.IsGameIl2Cpp())
                Il2Cpp.Patch(harmony);
            else
                Mono.Patch(harmony);
            
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

                ModdingAPI.ModInformation info = new ModdingAPI.ModInformation();
                info.SetName(melon.Info.Name);
                info.SetVersion(melon.Info.Version);
                info.SetAuthor(melon.Info.Author);
                info.SetDescription(melon.Info.DownloadLink);

                ModInformation.Add(info);

                if (MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(melon.Assembly) != null)
                    ConnectionString += $", {melon.Info.Name} v{melon.Info.Version}";
            }

            ModInformation.Sort((ModdingAPI.ModInformation left, ModdingAPI.ModInformation right) => string.Compare(left.GetName(), right.GetName()));
        }

        private static bool GetConnectionString(ref string __result) { __result = ConnectionString; return false; }
    }
}