using UnityEngine;

namespace MelonLoader.Support
{
    internal class Component : MonoBehaviour
    {
        private static int CurrentSceneIndex = -1;
        private static bool IsDestroying = false;
        internal static void Create() { Main.obj = new GameObject(); DontDestroyOnLoad(Main.obj); Main.component = (Component)Main.obj.AddComponent(typeof(Component)); Main.component.SiblingFix(); }
        private void SiblingFix() { gameObject.transform.SetAsLastSibling(); transform.SetAsLastSibling(); }
        internal void Destroy() { IsDestroying = true; GameObject.Destroy(gameObject); }
        void Awake() { foreach (var queuedCoroutine in SupportModule_To.QueuedCoroutines) StartCoroutine(queuedCoroutine); SupportModule_To.QueuedCoroutines.Clear(); }
        void Start() => SiblingFix();
        void Update() { SiblingFix(); int loadedLevel = Application.loadedLevel; if (loadedLevel != CurrentSceneIndex) { Main.Interface.OnSceneWasLoaded(loadedLevel); CurrentSceneIndex = loadedLevel; } Main.Interface.Update(); }
        void FixedUpdate() => Main.Interface.FixedUpdate();
        void LateUpdate() => Main.Interface.LateUpdate();
        void OnGUI() => Main.Interface.OnGUI();
        void OnDestroy() { if (!IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); Main.Interface.Quit(); }
    }
}