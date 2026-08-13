using System;
using UnityEngine;

#if SM_Il2Cpp
using Il2CppInterop.Runtime;
using MelonLoader.InternalUtils;
#else
using System.Linq;
using System.Reflection;
#endif

namespace MelonLoader.Support
{
    internal static class ComponentSiblingFix
    {
        private static bool _failure;

#if SM_Il2Cpp
        private delegate void SetAsLastSiblingDelegate_Unity6(Il2CppSystem.IntPtr obj);
        private static SetAsLastSiblingDelegate_Unity6 _method_unity6;
#else
        private static MethodInfo _methodInfo;
#endif
        private delegate void SetAsLastSiblingDelegate(Transform obj);

        private static SetAsLastSiblingDelegate _method;

        internal static void SetAsLastSibling(Component obj)
        {
            if (_failure = !FindMethod())
                return;
            _failure = !TrySetAsLastSibling(obj);
        }

        private static void LogError(string cat, Exception ex)
        {
            MelonLogger.Warning($"Exception while {cat}: {ex}");
            MelonLogger.Warning("Melon Events might run before some MonoBehaviour Events");
        }

        private static bool FindMethod()
        {
            if (_failure)
                return false;

            try
            {
#if SM_Il2Cpp
                if (UnityInformationHandler.EngineVersion.Major >= 6000)
                {
                    _method_unity6 = RuntimeInvoke.ResolveICall<SetAsLastSiblingDelegate_Unity6>("UnityEngine.Transform::SetAsLastSibling_Injected");
                }
                else
                {
                    _method = RuntimeInvoke.ResolveICall<SetAsLastSiblingDelegate>("UnityEngine.Transform::SetAsLastSibling");
                }
                if (_method == null && _method_unity6 == null)
                    throw new Exception("Unable to find Internal Call for UnityEngine.Transform::SetAsLastSibling");
#else
                _methodInfo = typeof(Transform).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => (
                    (x.Name == nameof(SetAsLastSibling))
                    && (x.GetParameters().Count() == 0)));
                if (_methodInfo == null)
                    throw new Exception("Unable to find Method for UnityEngine.Transform::SetAsLastSibling");

                _method = (SetAsLastSiblingDelegate)Delegate.CreateDelegate(typeof(SetAsLastSiblingDelegate), _methodInfo);
#endif
            }
            catch (Exception ex)
            {
                LogError("Getting UnityEngine.Transform::SetAsLastSibling", ex);
                return false;
            }

            return true;
        }

        private static void InvokeMethod(Transform transform)
        {
#if SM_Il2Cpp
            if (UnityInformationHandler.EngineVersion.Major >= 6000)
            {
                _method_unity6(transform.m_CachedPtr);
            }
            else
            {
                _method(transform);
            }
#else
            _method(transform);
#endif
        }

        private static bool TrySetAsLastSibling(Component obj)
        {
            if (_failure || (_method == null))
                return false;

            try
            {
                InvokeMethod(obj.transform);
                InvokeMethod(obj.gameObject.transform);
            }
            catch (Exception ex)
            {
                LogError("Invoking UnityEngine.Transform::SetAsLastSibling", ex);
                return false;
            }

            return true;
        }
    }
}
