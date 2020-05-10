using System;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MelonLoader.SupportModule
{
    internal static class Main
    {
        internal static MelonLoaderComponent comp = null;
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
                LogSupport.TraceHandler += MelonModLogger.LogWarning;
            }
            ClassInjector.DoHook += Imports.Hook;
            ClassInjector.RegisterTypeInIl2Cpp<MelonLoaderComponent>();
            MelonLoaderComponent.CreateComponent();
            SceneManager.add_sceneLoaded(DelegateSupport.ConvertDelegate<UnityAction<Scene, LoadSceneMode>>(new Action<Scene, LoadSceneMode>(OnSceneLoad)));
            
            return new Il2CppSupportModule();
        }
        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }
    }
    public class MelonLoaderComponent : MonoBehaviour
    {
        internal static void CreateComponent()
        {
            GameObject obj = new GameObject("MelonLoader");
            GameObject.DontDestroyOnLoad(obj);
            Main.comp = obj.AddComponent<MelonLoaderComponent>();
            Main.comp.transform.SetAsLastSibling();
        }
        public MelonLoaderComponent(IntPtr intPtr) : base(intPtr) { }
        void Start() => transform.SetAsLastSibling();

        void Update()
        {
            transform.SetAsLastSibling(); 
            MelonLoader.Main.OnUpdate();
            MelonCoroutinesIl2Cpp.Process();
        }
        void FixedUpdate()
        {
            MelonLoader.Main.OnFixedUpdate();
            MelonCoroutinesIl2Cpp.ProcessWaitForFixedUpdate();
        }

        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() => CreateComponent();
    }
}