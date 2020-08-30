using System;
using System.Linq;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static bool IsDestroying = false;
        internal static GameObject obj = null;
        internal static MelonLoaderComponent comp = null;
        private static Camera OnPostRenderCam = null;

        private static ISupportModule Initialize()
        {
            LogSupport.RemoveAllHandlers();
            if (MelonConsole.Enabled || Imports.IsDebugMode())
                LogSupport.InfoHandler += MelonLogger.Log;
            LogSupport.WarningHandler += MelonLogger.LogWarning;
            LogSupport.ErrorHandler += MelonLogger.LogError;
            if (Imports.IsDebugMode())
                LogSupport.TraceHandler += MelonLogger.Log;

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
            var unityVersionSplit = MelonLoaderBase.UnityVersion.Split('.');
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
            MelonHandler.OnUpdate();
            MelonCoroutines.Process();
        }
        void FixedUpdate()
        {
            MelonHandler.OnFixedUpdate();
            MelonCoroutines.ProcessWaitForFixedUpdate();
        }
        void LateUpdate() => MelonHandler.OnLateUpdate();
        void OnGUI() => MelonHandler.OnGUI();
        void OnDestroy() { if (!Main.IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); MelonHandler.OnApplicationQuit(); }
    }
}