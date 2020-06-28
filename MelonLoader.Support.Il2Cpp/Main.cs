using System;
using System.Linq;
using System.Reflection;
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
            if (Console.Enabled || Imports.IsDebugMode())
            {
                LogSupport.InfoHandler -= System.Console.WriteLine;
                LogSupport.InfoHandler += MelonModLogger.Log;
            }
            LogSupport.WarningHandler -= System.Console.WriteLine;
            LogSupport.WarningHandler += MelonModLogger.LogWarning;
            LogSupport.ErrorHandler -= System.Console.WriteLine;
            LogSupport.ErrorHandler += MelonModLogger.LogError;
            if (Imports.IsDebugMode())
            {
                LogSupport.TraceHandler -= System.Console.WriteLine;
                LogSupport.TraceHandler += MelonModLogger.Log;
            }

            try
            {
                Assembly il2cppSystem = Assembly.Load("Il2CppSystem");
                if (il2cppSystem != null)
                {
                    Type unitytls = il2cppSystem.GetType("Il2CppMono.Unity.UnityTls");
                    if (unitytls != null)
                    {
                        unsafe
                        {
                            var tlsHookTarget = typeof(Uri).Assembly.GetType("Mono.Unity.UnityTls").GetMethod("GetUnityTlsInterface", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer();
                            var unityMethodField = UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(unitytls.GetMethod("GetUnityTlsInterface", BindingFlags.Public | BindingFlags.Static));
                            var unityMethodPtr = (IntPtr)unityMethodField.GetValue(null);
                            var unityMethod = *(IntPtr*)unityMethodPtr;
                            Imports.Hook((IntPtr)(&tlsHookTarget), unityMethod);
                        }
                    }
                    else
                        throw new Exception("Failed to get Type Il2CppMono.Unity.UnityTls!");
                }
                else
                    throw new Exception("Failed to get Assembly Il2CppSystem!");
            } catch(Exception ex) {
                MelonModLogger.LogWarning("Exception while setting up TLS, mods will not be able to use HTTPS: " + ex);
            }

            ClassInjector.DoHook += Imports.Hook;
            GetUnityVersionNumbers(out var major, out var minor, out var patch);
            UnityVersionHandler.Initialize(major, minor, patch);
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
            var unityVersionSplit = Application.unityVersion.Split('.');
            major = int.Parse(unityVersionSplit[0]);
            minor = int.Parse(unityVersionSplit[1]);
            var patchString = unityVersionSplit[2];
            var firstBadChar = patchString.FirstOrDefault(it => it < '0' || it > '9');
            patch = int.Parse(firstBadChar == 0 ? patchString : patchString.Substring(0, patchString.IndexOf(firstBadChar)));
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }
        private static void OnPostRender(Camera cam) { if (OnPostRenderCam == null) OnPostRenderCam = cam; if (OnPostRenderCam == cam) MelonCoroutines.ProcessWaitForEndOfFrame(); }
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