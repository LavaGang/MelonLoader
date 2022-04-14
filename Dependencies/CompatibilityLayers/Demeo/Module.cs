using System.Collections.Generic;
using System.Reflection;
using Boardgame.Modding;
using MelonLoader;
using RGCommon;
using System.Text;
using MelonLoader.Modules;
using System;

namespace MelonLoader.CompatibilityLayers
{
    internal class Demeo_Module : MelonModule
    {
        internal static string ConnectionString;
        internal static List<ModdingAPI.ModInformation> ModInformation = new List<ModdingAPI.ModInformation>();

        public override void OnInitialize()
        {
            MelonEvents.OnPreApplicationStart.Subscribe(OnPreAppStart, int.MinValue);
        }

        private static void OnPreAppStart()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("DemeoIntegration");

            harmony.Patch(typeof(GameStateMachine).GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("GetConnectionString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            if (MelonUtils.IsGameIl2Cpp())
                Il2Cpp.Patch(harmony);
            else
                Mono.Patch(harmony);
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

        private static bool GetConnectionString(ref string __result) { __result = ConnectionString; return false; }

    }
}