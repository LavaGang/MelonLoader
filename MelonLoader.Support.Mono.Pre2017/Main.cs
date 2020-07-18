using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static bool IsDestroying = false;
        internal static GameObject obj = null;
        internal static MelonLoaderComponent comp = null;

        private static ISupportModule Initialize()
        {
            MelonLoaderComponent.Create();
            SceneManager.sceneLoaded += OnSceneLoad;
            return new Module();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }

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
        void Update() { transform.SetAsLastSibling(); MelonLoader.Main.OnUpdate(); }
        void FixedUpdate() => MelonLoader.Main.OnFixedUpdate();
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() { if (!Main.IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); MelonLoader.Main.OnApplicationQuit(); }
    }
}