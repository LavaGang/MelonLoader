using System;
using System.Reflection;
using UnityEngine;
using Il2CppInterop.Runtime;
using MelonLoader.Modules;
using Il2CppInterop.Runtime.Attributes;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    internal class Il2CppSupportComponent : MonoBehaviour
    {
        private bool isQuitting;
        private bool hadError;
        private bool useGeneratedAssembly = true;

        private delegate bool SetAsLastSiblingDelegate(IntPtr transformptr);
        private SetAsLastSiblingDelegate SetAsLastSiblingDelegateField;
        private MethodInfo SetAsLastSiblingMethod;

        public Il2CppSupportComponent(IntPtr value) : base(value) { }

        Il2CppSupportComponent()
        {
            try
            {
                SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance);
                if (SetAsLastSiblingMethod != null)
                    return;

                useGeneratedAssembly = false;
                SetAsLastSiblingDelegateField = IL2CPP.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");
                if (SetAsLastSiblingDelegateField == null)
                    throw new Exception("Unable to find Internal Call for UnityEngine.Transform::SetAsLastSibling");
            }
            catch (Exception ex) { LogError("Getting UnityEngine.Transform::SetAsLastSibling", ex); }
        }

        [HideFromIl2Cpp]
        private void LogError(string cat, Exception ex)
        {
            hadError = true;
            useGeneratedAssembly = false;
            MelonLogger.Warning($"Exception while {cat}: {ex}");
            MelonLogger.Warning("Melon Events might run before some MonoBehaviour Events");
        }

        internal void SiblingFix()
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
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            SiblingFix();

            MelonUnityEvents.OnApplicationLateStart.Invoke();
        }

        void Awake()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonCoroutines.ProcessQueue();
        }

        void Update()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            isQuitting = false;
            SiblingFix();

            ((Il2CppSupportModule)ModuleInterop.Support).sceneHandler.OnUpdate();

            MelonUnityEvents.OnUpdate.Invoke();
        }

        void OnDestroy()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            if (!isQuitting)
            {
                ((Il2CppSupportModule)ModuleInterop.Support).CreateGameObject();
                return;
            }

            OnApplicationDefiniteQuit();
        }

        void OnApplicationQuit()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            isQuitting = true;
            MelonUnityEvents.OnApplicationQuit.Invoke();
        }

        void OnApplicationDefiniteQuit()
        {
            MelonUnityEvents.OnApplicationDefiniteQuit.Invoke();
        }

        void FixedUpdate()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnFixedUpdate.Invoke();
        }

        void LateUpdate()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnLateUpdate.Invoke();
        }

        void OnGUI()
        {
            if ((ModuleInterop.Support == null) || (((Il2CppSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnGUI.Invoke();
        }
    }
}
