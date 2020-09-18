using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static Component component = null;
        private static Camera OnPostRenderCam = null;
        private static bool ShouldCheckForUiManager = true;
        private static Assembly Assembly_CSharp = null;
        private static Type VRCUiManager = null;
        private static System.Reflection.PropertyInfo VRCUiManager_Instance = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            LogSupport.RemoveAllHandlers();
            if (MelonDebug.IsEnabled())
                LogSupport.InfoHandler += MelonLogger.Msg;
            LogSupport.WarningHandler += MelonLogger.Warning;
            LogSupport.ErrorHandler += MelonLogger.Error;
            if (MelonDebug.IsEnabled())
                LogSupport.TraceHandler += MelonLogger.Msg;
            ClassInjector.DoHook = MelonUtils.NativeHookAttach;
            InitializeUnityVersion();
            ConsoleCleaner();
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
            ClassInjector.RegisterTypeInIl2Cpp<Component>();
            Component.Create();
            return new SupportModule_To();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (scene != null) Interface.OnSceneWasLoaded(scene.buildIndex); }
        private static void OnPostRender(Camera cam) { if (OnPostRenderCam == null) OnPostRenderCam = cam; if (OnPostRenderCam == cam) Coroutines.ProcessWaitForEndOfFrame(); }

        private static void ConsoleCleaner()
        {
            Il2CppSystem.IO.TextWriter writer = new Il2CppSystem.IO.TextWriter();
            Il2CppSystem.Console.stdout = writer;
            Il2CppSystem.Console.stderr = writer;
        }

        private static void InitializeUnityVersion()
        {
            string unityVersion = MelonUtils.GetUnityVersion();
            if (string.IsNullOrEmpty(unityVersion))
                return;
            string[] unityVersionSplit = unityVersion.Split('.');
            int major = int.Parse(unityVersionSplit[0]);
            int minor = int.Parse(unityVersionSplit[1]);
            string patchString = unityVersionSplit[2];
            char firstBadChar = patchString.FirstOrDefault(it => it < '0' || it > '9');
            int patch = int.Parse(firstBadChar == 0 ? patchString : patchString.Substring(0, patchString.IndexOf(firstBadChar)));
            UnityVersionHandler.Initialize(major, minor, patch);
        }

        internal static void VRChat_CheckUiManager()
        {
            if (!ShouldCheckForUiManager)
                return;
            if (Assembly_CSharp == null)
                Assembly_CSharp = Assembly.Load("Assembly_CSharp");
            if (Assembly_CSharp == null)
                return;
            if (VRCUiManager == null)
                VRCUiManager = Assembly_CSharp.GetType("VRCUiManager");
            if (VRCUiManager == null)
                return;
            if (VRCUiManager_Instance == null)
                VRCUiManager_Instance = VRCUiManager.GetProperty("field_Protected_Static_VRCUiManager_0");
            if (VRCUiManager_Instance == null)
                return;
            object returnval = VRCUiManager_Instance.GetValue(null, new object[0]);
            if (returnval == null)
                return;
            ShouldCheckForUiManager = false;
            Interface.VRChat_OnUiManagerInit();
        }
    }
}