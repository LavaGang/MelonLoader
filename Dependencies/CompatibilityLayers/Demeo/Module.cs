using System.Collections.Generic;
using System.Reflection;
using Boardgame.Modding;
using Prototyping;
using MelonLoader.Modules;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonModule
    {
        private static Dictionary<MelonBase, ModdingAPI.ModInformation> ModInformation = new Dictionary<MelonBase, ModdingAPI.ModInformation>();

        public override void OnInitialize()
        {
            MelonEvents.OnApplicationStart.Subscribe(OnPreAppStart, int.MaxValue);
            MelonBase.OnMelonRegistered.Subscribe(ParseMelon, int.MaxValue);
            MelonBase.OnMelonUnregistered.Subscribe(OnUnregister, int.MaxValue);
        }

        private static void OnPreAppStart()
        {
            HarmonyLib.Harmony harmony = new("DemeoIntegration");

            harmony.Patch(Assembly.Load("Assembly-CSharp").GetType("Prototyping.RG").GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("InitFix", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            foreach (var m in MelonPlugin.RegisteredMelons)
            {
                ParseMelon(m);
            }
            foreach (var m in MelonMod.RegisteredMelons)
            {
                ParseMelon(m);
            }
        }

        private static void OnUnregister(MelonBase melon)
        {
            if (melon == null)
                return;

            if (!ModInformation.ContainsKey(melon))
                return;

            ModInformation.Remove(melon);

            if (ModdingAPI.ExternallyInstalledMods == null)
                ModdingAPI.ExternallyInstalledMods = new List<ModdingAPI.ModInformation>();
            else
                ModdingAPI.ExternallyInstalledMods.Remove(ModInformation[melon]);
        }

        private static void ParseMelon<T>(T melon) where T : MelonBase
        {

            if (melon == null)
                return;

            if (ModInformation.ContainsKey(melon))
                return;

            ModdingAPI.ModInformation info = new ModdingAPI.ModInformation();
            info.SetName(melon.Info.Name);
            info.SetVersion(melon.Info.Version);
            info.SetAuthor(melon.Info.Author);
            info.SetDescription(melon.Info.DownloadLink);
            info.SetIsNetworkCompatible(MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(melon.MelonAssembly.Assembly) == null);

            ModInformation.Add(melon, info);

            if (ModdingAPI.ExternallyInstalledMods == null)
                ModdingAPI.ExternallyInstalledMods = new List<ModdingAPI.ModInformation>();
            ModdingAPI.ExternallyInstalledMods.Add(info);
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