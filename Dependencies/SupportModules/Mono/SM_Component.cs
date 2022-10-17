using System;
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
        private bool isQuitting;

#if SM_Il2Cpp
        private delegate bool SetAsLastSiblingDelegate(IntPtr transformptr);
        private static SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;
        public SM_Component(IntPtr value) : base(value) { }
#else
        private static MethodInfo SetAsLastSiblingMethod;
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
            if (Main.component != null)
                return;

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
            Destroy(gameObject);
        }

        void Start()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            SiblingFix();
            Main.Interface.OnApplicationLateStart();
        }

        void Awake()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            foreach (var queuedCoroutine in SupportModule_To.QueuedCoroutines)
#if SM_Il2Cpp
                StartCoroutine(new Il2CppSystem.Collections.IEnumerator(new MonoEnumeratorWrapper(queuedCoroutine).Pointer));
#else
                StartCoroutine(queuedCoroutine);
#endif
            SupportModule_To.QueuedCoroutines.Clear();
        }

        void Update()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            isQuitting = false;
            SiblingFix();

            SceneHandler.OnUpdate();
            Main.Interface.Update();
        }

        void OnDestroy()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            if (!isQuitting)
            {
                Create();
                return;
            }

            OnApplicationDefiniteQuit();
        }

        void OnApplicationQuit()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            isQuitting = true;
            Main.Interface.Quit();
        }

        void OnApplicationDefiniteQuit()
        {
            Main.Interface.DefiniteQuit();
        }

        void FixedUpdate()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            Main.Interface.FixedUpdate();
        }

        void LateUpdate()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            Main.Interface.LateUpdate();
        }

        void OnGUI()
        {
            if ((Main.component != null) && (Main.component != this))
                return;

            Main.Interface.OnGUI();
        }
    }
}