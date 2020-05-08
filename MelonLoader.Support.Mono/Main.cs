using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.SupportModule
{
    internal static class Main
    {
        internal static MelonLoaderComponent comp = null;
        private static void Initialize()
        {
            MelonLoaderComponent.CreateComponent();
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (!scene.Equals(null)) SceneHandler.OnSceneLoad(scene.buildIndex); }
        private static float GetUnityDeltaTime() => Time.deltaTime;
        private static object GetComponent() => comp;
        private static Type GetMonoBehaviourType() => typeof(MonoBehaviour);
        private static Type GetCoroutineType() => typeof(Coroutine);
        private static Type GetWaitForSecondsType() => typeof(WaitForSeconds);
        private static Type GetCustomYieldInstructionType() => typeof(CustomYieldInstruction);
        private static Type GetWaitForFixedUpdateType() => typeof(WaitForFixedUpdate);
        private static Type GetWaitForEndOfFrameType() => typeof(WaitForEndOfFrame);
        private static object GetActiveScene() => SceneManager.GetActiveScene();
        private static int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
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
        void Start() => transform.SetAsLastSibling();
        void Update() { transform.SetAsLastSibling(); MelonLoader.Main.OnUpdate(); }
        void FixedUpdate() => MelonLoader.Main.OnFixedUpdate();
        void LateUpdate() => MelonLoader.Main.OnLateUpdate();
        void OnGUI() => MelonLoader.Main.OnGUI();
        void OnDestroy() => CreateComponent();
    }
}