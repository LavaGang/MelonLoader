﻿using System;
#if SM_Il2Cpp
using UnhollowerBaseLib;
#else
using System.Reflection;
#endif
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SM_Component : MonoBehaviour
    {
        private bool IsDestroying = false;

#if SM_Il2Cpp
        private delegate bool SetAsLastSiblingDelegate(IntPtr transformptr);
        private static SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;
        public SM_Component(IntPtr value) : base(value) { }
#else
        private static MethodInfo SetAsLastSiblingMethod = null;
#endif

        static SM_Component()
        {
            try
            {
#if SM_Il2Cpp
                SetAsLastSiblingDelegateField = IL2CPP.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");
#else
                SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance);
#endif
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Exception while Getting Transform.SetAsLastSibling: {ex}");
            }
        }

        internal static void Create()
        {
            Main.obj = new GameObject();
            DontDestroyOnLoad(Main.obj);
            Main.obj.hideFlags = HideFlags.DontSave;
#if SM_Il2Cpp
            Main.component = Main.obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<SM_Component>()).TryCast<SM_Component>();
#else
            Main.component = (SM_Component)Main.obj.AddComponent(typeof(SM_Component));
#endif
            Main.component.SiblingFix();
        }

        private void SiblingFix()
        {
#if SM_Il2Cpp
            SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(gameObject.transform));
            SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(transform));
#else
            SetAsLastSiblingMethod?.Invoke(gameObject.transform, new object[0]);
            SetAsLastSiblingMethod?.Invoke(transform, new object[0]);
#endif
        }

        internal void Destroy()
        {
            IsDestroying = true;
            GameObject.Destroy(gameObject);
        }

        public void Start()
        {
            SiblingFix();
            Main.Interface.OnApplicationLateStart();
        }

        public void Awake()
        {
            foreach (var queuedCoroutine in SupportModule_To.QueuedCoroutines)
#if SM_Il2Cpp
                StartCoroutine(new Il2CppSystem.Collections.IEnumerator(new MonoEnumeratorWrapper(queuedCoroutine).Pointer));
#else
                StartCoroutine(queuedCoroutine);
#endif
            SupportModule_To.QueuedCoroutines.Clear();
        }

        public void Update()
        {
            SiblingFix();
#if SM_Il2Cpp
            if (MelonUtils.IsBONEWORKS)
                BONEWORKS_SceneHandler.OnUpdate();
#endif
            Main.Interface.Update();
        }

        public void OnDestroy()
        {
            if (!IsDestroying)
                Create();
        }

        public void OnApplicationQuit()
        {
            Destroy();
            Main.Interface.Quit();
        }

        public void FixedUpdate() => Main.Interface.FixedUpdate();
        public void LateUpdate() => Main.Interface.LateUpdate();
        public void OnGUI() => Main.Interface.OnGUI();
    }
}