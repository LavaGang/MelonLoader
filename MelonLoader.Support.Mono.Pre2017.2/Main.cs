using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Harmony;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static bool IsDestroying = false;
        internal static GameObject obj = null;
        internal static MelonLoaderComponent comp = null;
        internal static int CurrentScene = -9;
        private static ISupportModule Initialize()
        {
            if (!Imports.IsDebugMode())
            {
                try
                {
                    Assembly mscorlib = Assembly.Load("mscorlib");
                    if (mscorlib != null)
                    {
                        Type System_Console = mscorlib.GetType("System.Console");
                        if (System_Console != null)
                        {
                            MethodInfo[] methods = System_Console.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name.StartsWith("Write")).ToArray();
                            if (methods.Length > 0)
                            {
                                HarmonyInstance harmonyInstance = HarmonyInstance.Create("MelonLoader.Support.Mono.Pre2017.2");
                                for (int i = 0; i < methods.Length; i++)
                                    harmonyInstance.Patch(methods[i], new HarmonyMethod(typeof(Main).GetMethod("NullPrefixPatch", BindingFlags.NonPublic | BindingFlags.Static)));
                            }
                            else
                                throw new Exception("Failed to find Write Methods!");
                        }
                        else
                            throw new Exception("Failed to get Type System.Console!");
                    }
                    else
                        throw new Exception("Failed to get Assembly mscorlib!");
                }
                catch (Exception ex)
                {
                    MelonModLogger.LogWarning("Exception while setting up Console Cleaner, Game Logs may show in Console:  | " + ex);
                }
            }

            MelonLoaderComponent.Create();
            return new Module();
        }

        private static bool NullPrefixPatch() => false;
    }
    public class MelonLoaderComponent : MonoBehaviour
    {
        internal static void Create()
        {
            Main.obj = new GameObject("MelonLoader");
            DontDestroyOnLoad(Main.obj);
            Main.comp = Main.obj.AddComponent<MelonLoaderComponent>();
            Main.obj.transform.SetAsLastSibling();
            Main.comp.transform.SetAsLastSibling();
        }
        internal static void Destroy() { Main.IsDestroying = true; if (Main.obj != null) GameObject.Destroy(Main.obj); }
        void Start() => transform.SetAsLastSibling();
        void Update()
        {
            transform.SetAsLastSibling();
            if (Main.CurrentScene != Application.loadedLevel)
            {
                SceneHandler.OnSceneLoad(Application.loadedLevel);
                Main.CurrentScene = Application.loadedLevel;
            }
            MelonLoader.Main.OnUpdate();
        }
        void FixedUpdate() => MelonLoader.Main.OnFixedUpdate();
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() { if (!Main.IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); MelonLoader.Main.OnApplicationQuit(); }
    }
}