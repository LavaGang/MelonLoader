using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.SupportModule
{
    internal static class Main
    {
        internal static MelonLoaderComponent comp = null;
        private static ISupportModule Initialize()
        {
            MelonLoaderComponent.CreateComponent();
            SceneManager.sceneLoaded += OnSceneLoad;
            
            return new MonoSupportModule();
        }
        
        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }
    }
    public class MelonLoaderComponent : MonoBehaviour
    {
        internal static void CreateComponent()
        {
            GameObject obj = new GameObject("MelonLoader");
            DontDestroyOnLoad(obj);
            Main.comp = obj.AddComponent<MelonLoaderComponent>();
            Main.comp.transform.SetAsLastSibling();
        }
        void Start() => transform.SetAsLastSibling();
        void Update() { transform.SetAsLastSibling(); MelonLoader.Main.OnUpdate(); }
        void FixedUpdate() => MelonLoader.Main.OnFixedUpdate();
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() => CreateComponent();
    }
}