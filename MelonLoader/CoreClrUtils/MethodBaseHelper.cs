#if NET6_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable

namespace MelonLoader.CoreClrUtils
{
    internal static class MethodBaseHelper
    {
        private static Type? RuntimeMethodHandleInternal;
        private static ConstructorInfo? RuntimeMethodHandleInternal_Constructor;
        private static Type? RuntimeType;
        private static MethodInfo? RuntimeType_GetMethodBase;

        public static MethodBase? GetMethodBaseFromHandle(IntPtr handle)
        {
            RuntimeMethodHandleInternal ??= typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodHandleInternal", throwOnError: true)!;
            RuntimeMethodHandleInternal_Constructor ??= RuntimeMethodHandleInternal.GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DoNotWrapExceptions,
                binder: null,
                new[] { typeof(IntPtr) },
                modifiers: null
            ) ?? throw new InvalidOperationException("RuntimeMethodHandleInternal constructor is missing!");

            RuntimeType ??= typeof(Type).Assembly.GetType("System.RuntimeType", throwOnError: true)!;
            RuntimeType_GetMethodBase ??= RuntimeType.GetMethod
            (
                "GetMethodBase",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DoNotWrapExceptions,
                binder: null,
                new[] { RuntimeType, RuntimeMethodHandleInternal },
                modifiers: null
            ) ?? throw new InvalidOperationException("RuntimeType.GetMethodBase is missing!");

            // Wrap the handle
            object runtimeHandle = RuntimeMethodHandleInternal_Constructor.Invoke(new[] { (object)handle });
            return (MethodBase?)RuntimeType_GetMethodBase.Invoke(null, new[] { null, runtimeHandle });
        }
    }
}
#endif