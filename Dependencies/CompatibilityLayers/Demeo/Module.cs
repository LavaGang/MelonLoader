using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using Boardgame.Modding;
using RGCommon;
using System.Text;
using MelonLoader.Modules;
using System;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonModule
    {
        private static List<ModdingAPI.ModInformation> ModInformation = new List<ModdingAPI.ModInformation>();

        public override void OnInitialize()
        {
            MelonEvents.OnPreApplicationStart.Subscribe(OnPreAppStart, int.MinValue);
        }

        private static void OnPreAppStart()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");

            harmony.Patch(typeof(ModdingAPI).GetMethod("GetInstalledMods", BindingFlags.Public | BindingFlags.Instance),
                typeof(Demeo_Module).GetMethod("GetInstalledMods", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            harmony.Patch(typeof(GameStateMachine).GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("GetConnectionString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
        }

        private static ModdingAPI.ModInformation ParseMelon(MelonBase melon)
            => new ModdingAPI.ModInformation()
            {
                name = melon.Info.Name,
                version = melon.Info.Version,
                author = melon.Info.Author,
                description = melon.Info.DownloadLink
            };

        private static void AppendMelonInfo(StringBuilder sb, MelonBase m)
        {
            if (MelonUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(m.Assembly) != null)
                sb.Append($", {m.Info.Name} v{m.Info.Version}");
        }

        private static bool GetInstalledMods(ref List<ModdingAPI.ModInformation> __result)
        {
            var result = new List<ModdingAPI.ModInformation>();

            foreach (var m in MelonMod.RegisteredMelons)
                result.Add(ParseMelon(m));
            foreach (var m in MelonPlugin.RegisteredMelons)
                result.Add(ParseMelon(m));

            result.Sort((ModdingAPI.ModInformation left, ModdingAPI.ModInformation right) => string.Compare(left.name, right.name));
            __result = result;
            return false;
        }

        private static bool GetConnectionString(ref string __result)
        {
            var sb = new StringBuilder();
            sb.Append(VersionInformation.MajorMinorVersion);

            foreach (var m in MelonMod.RegisteredMelons)
                AppendMelonInfo(sb, m);
            foreach (var m in MelonPlugin.RegisteredMelons)
                AppendMelonInfo(sb, m);

            __result = sb.ToString();
            return false;
        }
    }
}