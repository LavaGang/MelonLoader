using System;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(TestMod3.Main), "Test Mod 3", "0.0.0")]
[assembly: MelonGame(null, null)]
namespace TestMod3
{
    class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("starting");
            //foreach (var el in UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScanImpl_Test())
            //{
            //}
            //typeof(TestScript).GetProperty("ModSuccess").GetGetMethod().GetMethodBody();
            MelonLogger.Msg("test");
            //MelonLogger.Msg(typeof(global::TestScript).GetProperty("ModSuccess").GetGetMethod().GetMethodBody() == null);

            //UnhollowerBaseLib.LogSupport.TraceHandler += (msg) => MelonLogger.Msg(msg);

            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<TestType>();
        }
    }

    class TestType : UnityEngine.MonoBehaviour
    {
        public TestType(IntPtr value) : base(value) { }

        void FixedUpdate()
        {
            MelonLogger.Msg("Fixed update");
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(global::TestScript), "Start")]
    class TestPatch
    {
        static void Postfix(TestScript __instance)
        {
            var textMesh = __instance.GetComponent<TextMesh>();
            MelonLogger.Msg(textMesh);

            if (textMesh == null)
            {
                MelonLogger.Msg("text mesh is null");
                return;
            }

            textMesh.text = "MELON LOADER WORKS!";
            textMesh.color = Color.green;

            __instance.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            __instance.gameObject.AddComponent<TestType>();
        }
    }
}
