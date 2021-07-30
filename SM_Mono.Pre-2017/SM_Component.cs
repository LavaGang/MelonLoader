using System;
using System.Reflection;
using MelonLoader.Utils;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SM_Component : MonoBehaviour
    {
        private static bool IsDestroying;
        private static MethodInfo SetAsLastSiblingMethod;
        static SM_Component() { try { SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance); } catch (Exception ex) { MelonLogger.Warning($"Exception while Getting Transform.SetAsLastSibling: {ex}"); } }
        internal static void Create() { Main.obj = new GameObject(); DontDestroyOnLoad(Main.obj); Main.component = (SM_Component)Main.obj.AddComponent(typeof(SM_Component)); Main.component.SiblingFix(); }
        private void SiblingFix() { SetAsLastSiblingMethod?.Invoke(gameObject.transform, new object[0]); SetAsLastSiblingMethod?.Invoke(transform, new object[0]); }
        internal void Destroy() { IsDestroying = true; Destroy(gameObject); }
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