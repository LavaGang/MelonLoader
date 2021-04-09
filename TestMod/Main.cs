using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;

[assembly: MelonInfo(typeof(TestMod.Main), "Test Mod", "0.0.0")]
[assembly: MelonGame(null, null)]
namespace TestMod
{
    class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            //typeof(TestScript).GetProperty("ModSuccess").GetGetMethod().GetMethodBody();
            MelonLogger.Msg("test");
            MelonLogger.Msg(typeof(TestScript).GetProperty("ModSuccess").GetGetMethod().GetMethodBody() == null);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(TestScript), "ModSuccess", HarmonyLib.MethodType.Getter)]
    class TestPatch
    {
        static void Prefix()
        {
            MelonLogger.Msg("Success");
        }
    }
}
