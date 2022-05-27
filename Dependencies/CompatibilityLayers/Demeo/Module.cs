using System;
using System.Collections.Generic;
using System.Reflection;
using Boardgame.Modding;
using HarmonyLib;
using Prototyping;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonCompatibilityLayer.Module
    {
        public override void Setup()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");
            harmony.Patch(Assembly.Load("Assembly-CSharp").GetType("Prototyping.RG").GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("InitFix", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            MelonCompatibilityLayer.AddRefreshPluginsEvent(Refresh);
            MelonCompatibilityLayer.AddRefreshModsEvent(Refresh);
            Refresh();
        }

        private static void Refresh()
        {
            if (ModdingAPI.ExternallyInstalledMods == null)
                ModdingAPI.ExternallyInstalledMods = new List<ModdingAPI.ModInformation>();
            else
                ModdingAPI.ExternallyInstalledMods.Clear();
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
                info.SetIsNetworkCompatible(MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(melon.Assembly) == null);
                ModdingAPI.ExternallyInstalledMods.Add(info);
            }
        }

        private static bool InitFix()
        {
            if (MotherbrainGlobalVars.IsRunningOnDesktop)
                RG.SetVrMode(false);
            else
                RG.SetVrMode(RG.XRDeviceIsPresent());
            return true;
        }
    }
}