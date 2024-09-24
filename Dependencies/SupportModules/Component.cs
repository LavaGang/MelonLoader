using System;
using System.Reflection;
using UnityEngine;

#if SM_Il2Cpp
using Il2CppInterop.Runtime;
#endif

namespace MelonLoader.Support
{
    internal class SM_Component : MonoBehaviour
    {
        private bool isQuitting;
        private static bool hadError;
        private static bool useGeneratedAssembly = true;

        private static MethodInfo SetAsLastSiblingMethod;

#if SM_Il2Cpp
        private delegate bool SetAsLastSiblingDelegate(IntPtr transformptr);
        private static SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;
        public SM_Component(IntPtr value) : base(value) { }
#endif

        static SM_Component()
        {
            try
            {
#if SM_Il2Cpp
                SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance);
                if (SetAsLastSiblingMethod != null)
                    return;

                useGeneratedAssembly = false;
                SetAsLastSiblingDelegateField = IL2CPP.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");
                if (SetAsLastSiblingDelegateField == null)
                    throw new Exception("Unable to find Internal Call for UnityEngine.Transform::SetAsLastSibling");
#else
                SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance);
                if (SetAsLastSiblingMethod == null)
                    throw new Exception("Unable to find UnityEngine.Transform::SetAsLastSibling");
#endif
            }
            catch (Exception ex) { LogError("Getting UnityEngine.Transform::SetAsLastSibling", ex); }
        }

        internal static void Create()
        {
            if (Main.component != null)
                return;

            Main.obj = new GameObject();
            DontDestroyOnLoad(Main.obj);
            Main.obj.hideFlags = HideFlags.DontSave;

#if SM_Il2Cpp
            Main.component = Main.obj.AddComponent(Il2CppType.Of<SM_Component>()).TryCast<SM_Component>();
#else
            Main.component = (SM_Component)Main.obj.AddComponent(typeof(SM_Component));
#endif

            Main.component.SiblingFix();
        }

        private static void LogError(string cat, Exception ex)
        {
            hadError = true;
            useGeneratedAssembly = false;
            MelonLogger.Warning($"Exception while {cat}: {ex}");
            MelonLogger.Warning("Melon Events might run before some MonoBehaviour Events");
        }

        private void SiblingFix()
        {
            if (hadError)
                return;

            try
            {
                if (useGeneratedAssembly)
                {
                    gameObject.transform.SetAsLastSibling();
                    transform.SetAsLastSibling();
                    return;
                }

#if SM_Il2Cpp
                SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(gameObject.transform));
                SetAsLastSiblingDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(transform));
#endif
            }
            catch (Exception ex)
            {
#if SM_Il2Cpp
                if (useGeneratedAssembly)
                {
                    useGeneratedAssembly = false;
                    SiblingFix();
                    return;
                }
#endif

                LogError("Invoking UnityEngine.Transform::SetAsLastSibling", ex);
            }
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