using Il2CppInterop.Common;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    public static class Il2CppExtensions
    {
        #region Coroutine

        public static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new Il2CppEnumeratorWrapper(enumerator).Pointer));

        public static void StopCoroutine<T>(this T behaviour, Coroutine routine)
            where T : MonoBehaviour
            => behaviour.StopCoroutine(routine);

        #endregion

        #region Interop

        public static int? GetIl2CppMethodCallerCount(this MethodBase method)
        {
            CallerCountAttribute att = method.GetCustomAttribute<CallerCountAttribute>();
            if (att == null)
                return null;
            return att.Count;
        }

        public static FieldInfo MethodBaseToIl2CppFieldInfo(this MethodBase method)
            => Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);

        public static void RegisterTypeInIl2CppDomain(this Type type, bool logSuccess)
            => ClassInjector.RegisterTypeInIl2Cpp(type, new() { LogSuccess = logSuccess });
        public static void RegisterTypeInIl2CppDomainWithInterfaces(this Type type, Type[] interfaces, bool logSuccess)
            => ClassInjector.RegisterTypeInIl2Cpp(type, new() { LogSuccess = logSuccess, Interfaces = interfaces });

        public static bool IsGeneratedAssemblyType(this Type type)
            => type.IsInheritedFromIl2CppObjectBase() && !type.IsInjectedType();

        public static bool IsInheritedFromIl2CppObjectBase(this Type type)
            => (type != null) && type.IsSubclassOf(typeof(Il2CppObjectBase));

        public static bool IsInjectedType(this Type type)
        {
            IntPtr ptr = GetClassPointerForType(type);
            return ptr != IntPtr.Zero && RuntimeSpecificsStore.IsInjected(ptr);
        }

        public static IntPtr GetClassPointerForType(this Type type)
        {
            if (type == typeof(void)) return Il2CppClassPointerStore<Il2CppSystem.Void>.NativeClassPtr;
            return (IntPtr)typeof(Il2CppClassPointerStore<>).MakeGenericType(type)
                  .GetField(nameof(Il2CppClassPointerStore<int>.NativeClassPtr)).GetValue(null);
        }

        public static T Il2CppObjectPtrToIl2CppObject<T>(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException("The ptr cannot be IntPtr.Zero.");
            if (!IsGeneratedAssemblyType(typeof(T)))
                throw new NullReferenceException("The type must be a Generated Assembly Type.");
            return (T)typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, 
                null, 
                [typeof(IntPtr)], 
                Array.Empty<ParameterModifier>())
                .Invoke([ptr]);
        }

        #endregion
    }
}
