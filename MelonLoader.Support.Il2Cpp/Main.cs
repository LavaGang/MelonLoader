using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Harmony;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static bool IsDestroying = false;
        internal static GameObject obj = null;
        internal static MelonLoaderComponent comp = null;
        private static Camera OnPostRenderCam = null;
        private static MethodInfo Il2CppSystem_Console_WriteLine = null;
        private static HarmonyInstance harmonyInstance = null;

        private static ISupportModule Initialize()
        {
            LogSupport.RemoveAllHandlers();
            if (Console.Enabled || Imports.IsDebugMode())
                LogSupport.InfoHandler += MelonLogger.Log;
            LogSupport.WarningHandler += MelonLogger.LogWarning;
            LogSupport.ErrorHandler += MelonLogger.LogError;
            if (Imports.IsDebugMode())
                LogSupport.TraceHandler += MelonLogger.Log;

            if (MelonLoader.Main.IsVRChat)
            {
                try
                {
                    Assembly Transmtn = Assembly.Load("Transmtn");
                    if (Transmtn != null)
                    {
                        Type Transmtn_HttpConnection = Transmtn.GetType("Transmtn.HttpConnection");
                        if (Transmtn_HttpConnection != null)
                        {
                            Il2CppSystem_Console_WriteLine = typeof(Il2CppSystem.Console).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => (x.Name.Equals("WriteLine") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string))));
                            if (harmonyInstance == null)
                                harmonyInstance = HarmonyInstance.Create("MelonLoader.Support.Il2Cpp");
                            harmonyInstance.Patch(Transmtn_HttpConnection.GetMethod("get", BindingFlags.Public | BindingFlags.Instance), new HarmonyMethod(typeof(Main).GetMethod("Transmtn_HttpConnection_get_Prefix", BindingFlags.NonPublic | BindingFlags.Static)), new HarmonyMethod(typeof(Main).GetMethod("Transmtn_HttpConnection_get_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));
                        }
                        else
                            throw new Exception("Failed to get Type Transmtn.HttpConnection!");
                    }
                    else
                        throw new Exception("Failed to get Assembly Transmtn!");
                }
                catch (Exception ex)
                {
                    MelonLogger.LogWarning("Exception while setting up Auth Token Hider, Auth Tokens may show in Console: " + ex);
                }
            }

            ClassInjector.DoHook += Imports.Hook;
            GetUnityVersionNumbers(out var major, out var minor, out var patch);
            UnityVersionHandler.Initialize(major, minor, patch);

            SetAsLastSiblingDelegateField = IL2CPP.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");

            ClassInjector.RegisterTypeInIl2Cpp<MelonLoaderComponent>();
            MelonLoaderComponent.Create();

            SceneManager.sceneLoaded = (
                (SceneManager.sceneLoaded == null)
                ? new Action<Scene, LoadSceneMode>(OnSceneLoad)
                : Il2CppSystem.Delegate.Combine(SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)new Action<Scene, LoadSceneMode>(OnSceneLoad)).Cast<UnityAction<Scene, LoadSceneMode>>()
                );
            Camera.onPostRender = (
                (Camera.onPostRender == null)
                ? new Action<Camera>(OnPostRender)
                : Il2CppSystem.Delegate.Combine(Camera.onPostRender, (Camera.CameraCallback)new Action<Camera>(OnPostRender)).Cast<Camera.CameraCallback>()
                );

            return new Module();
        }

        private static void GetUnityVersionNumbers(out int major, out int minor, out int patch)
        {
            var unityVersionSplit = MelonLoader.Main.UnityVersion.Split('.');
            major = int.Parse(unityVersionSplit[0]);
            minor = int.Parse(unityVersionSplit[1]);
            var patchString = unityVersionSplit[2];
            var firstBadChar = patchString.FirstOrDefault(it => it < '0' || it > '9');
            patch = int.Parse(firstBadChar == 0 ? patchString : patchString.Substring(0, patchString.IndexOf(firstBadChar)));
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }
        private static void OnPostRender(Camera cam) { if (OnPostRenderCam == null) OnPostRenderCam = cam; if (OnPostRenderCam == cam) MelonCoroutines.ProcessWaitForEndOfFrame(); }

        internal delegate bool SetAsLastSiblingDelegate(IntPtr u0040this);
        internal static SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;

        private static bool Il2CppSystem_Console_WriteLine_Patch() => false;
        private static void Transmtn_HttpConnection_get_Prefix() => harmonyInstance.Patch(Il2CppSystem_Console_WriteLine, new Harmony.HarmonyMethod(typeof(Main).GetMethod("Il2CppSystem_Console_WriteLine_Patch", BindingFlags.NonPublic | BindingFlags.Static)));
        private static void Transmtn_HttpConnection_get_Postfix() => harmonyInstance.Unpatch(Il2CppSystem_Console_WriteLine, typeof(Main).GetMethod("Il2CppSystem_Console_WriteLine_Patch", BindingFlags.NonPublic | BindingFlags.Static));
    }

    public class MelonLoaderComponent : MonoBehaviour
    {
        internal static void Create()
        {
            Main.obj = new GameObject();
            DontDestroyOnLoad(Main.obj);
            Main.comp = new MelonLoaderComponent(Main.obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<MelonLoaderComponent>()).Pointer);
            Main.SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(Main.obj.transform));
            Main.SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(Main.comp.transform));
        }
        internal static void Destroy() { Main.IsDestroying = true; if (Main.obj != null) GameObject.Destroy(Main.obj); }
        public MelonLoaderComponent(IntPtr intPtr) : base(intPtr) { }
        void Start() => transform.SetAsLastSibling();
        void Update()
        {
            transform.SetAsLastSibling();
            MelonLoader.Main.OnUpdate();
            MelonCoroutines.Process();
        }
        void FixedUpdate()
        {
            MelonLoader.Main.OnFixedUpdate();
            MelonCoroutines.ProcessWaitForFixedUpdate();
        }
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() { if (!Main.IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); MelonLoader.Main.OnApplicationQuit(); }
    }
}