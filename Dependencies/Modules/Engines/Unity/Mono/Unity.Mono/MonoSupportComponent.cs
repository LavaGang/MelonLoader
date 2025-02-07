using System;
using System.Reflection;
using UnityEngine;
using MelonLoader.Modules;

namespace MelonLoader.Engine.Unity.Mono
{
    internal class MonoSupportComponent : MonoBehaviour
    {
        private bool isQuitting;
        private bool hadError;

        private MethodInfo SetAsLastSiblingMethod;

        MonoSupportComponent()
        {
            try
            {
                SetAsLastSiblingMethod = typeof(Transform).GetMethod("SetAsLastSibling", BindingFlags.Public | BindingFlags.Instance);
                if (SetAsLastSiblingMethod == null)
                    throw new Exception("Unable to find UnityEngine.Transform::SetAsLastSibling");
            }
            catch (Exception ex) { LogError("Getting UnityEngine.Transform::SetAsLastSibling", ex); }
        }

        private void LogError(string cat, Exception ex)
        {
            hadError = true;
            MelonLogger.Warning($"Exception while {cat}: {ex}");
            MelonLogger.Warning("Melon Events might run before some MonoBehaviour Events");
        }

        internal void SiblingFix()
        {
            if (hadError)
                return;

            try
            {
                gameObject.transform.SetAsLastSibling();
                transform.SetAsLastSibling();
            }
            catch (Exception ex)
            {
                LogError("Invoking UnityEngine.Transform::SetAsLastSibling", ex);
            }
        }

        void Start()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            SiblingFix();

            MelonUnityEvents.OnApplicationLateStart.Invoke();
        }

        void Awake()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonCoroutines.ProcessQueue();
        }

        void Update()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            isQuitting = false;
            SiblingFix();

            ((MonoSupportModule)ModuleInterop.Support).sceneHandler.OnUpdate();

            MelonUnityEvents.OnUpdate.Invoke();
        }

        void OnDestroy()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            if (!isQuitting)
            {
                ((MonoSupportModule)ModuleInterop.Support).CreateGameObject();
                return;
            }

            OnApplicationDefiniteQuit();
        }

        void OnApplicationQuit()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
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
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnFixedUpdate.Invoke();
        }

        void LateUpdate()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnLateUpdate.Invoke();
        }

        void OnGUI()
        {
            if ((ModuleInterop.Support == null) || (((MonoSupportModule)ModuleInterop.Support).component != this))
                return;

            MelonUnityEvents.OnGUI.Invoke();
        }
    }
}
