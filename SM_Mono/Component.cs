using System.Reflection;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class Component : MonoBehaviour
    {
        private static bool IsDestroying = false;
        private static MethodInfo SetAsLastSiblingMethod = null;
        static Component() { try { SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance); } catch { } }
        internal static void Create() { Main.obj = new GameObject(); DontDestroyOnLoad(Main.obj); Main.component = (Component)Main.obj.AddComponent(typeof(Component)); Main.component.SiblingFix(); }
        private void SiblingFix() { SetAsLastSiblingMethod?.Invoke(gameObject.transform, new object[0]); SetAsLastSiblingMethod?.Invoke(transform, new object[0]); }
        internal void Destroy() { IsDestroying = true; GameObject.Destroy(gameObject); }
        void Awake() { foreach (var queuedCoroutine in SupportModule_To.QueuedCoroutines) StartCoroutine(queuedCoroutine); SupportModule_To.QueuedCoroutines.Clear(); }
        void Start() => SiblingFix();
        void Update() { SiblingFix(); Main.Interface.Update(); }
        void FixedUpdate() => Main.Interface.FixedUpdate();
        void LateUpdate() => Main.Interface.LateUpdate();
        void OnGUI() => Main.Interface.OnGUI();
        void OnDestroy() { if (!IsDestroying) Create(); }
        void OnApplicationQuit() { Destroy(); Main.Interface.Quit(); }
    }
}