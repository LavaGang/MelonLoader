﻿using MelonLoader;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Resources
    {
        private static readonly IntPtr m_GetBuiltinResource;

        static Resources()
        {

            InternalClassPointerStore<Resources>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Resources");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Resources>.NativeClassPtr);

            /*
            IntPtr mptr = IntPtr.Zero;
            IntPtr iter = IntPtr.Zero;
            while ((mptr = UnityInternals.class_get_methods(InternalClassPointerStore<Resources>.NativeClassPtr, ref iter)) != IntPtr.Zero)
            {
                uint paramCount = UnityInternals.method_get_param_count(mptr);
                string[] paramTypes = new string[paramCount];
                for (uint i = 0; i < paramCount; ++i)
                    paramTypes[i] = Marshal.PtrToStringAnsi(UnityInternals.type_get_name(UnityInternals.method_get_param(mptr, i)));
                MelonLoader.MelonLogger.Msg($" {Marshal.PtrToStringAnsi(UnityInternals.type_get_name(UnityInternals.method_get_return_type(mptr)))} {Marshal.PtrToStringAnsi(UnityInternals.method_get_name(mptr))}({string.Join(", ", paramTypes)})");
            }
            */
            // T GetBuiltinResource(System.String)

            m_GetBuiltinResource = UnityInternals.GetMethod(InternalClassPointerStore<Resources>.NativeClassPtr, "GetBuiltinResource", "UnityEngine.Object", "System.Type", "System.String");

        }

        public unsafe static IntPtr GetBuiltinResource(Il2CppSystem.Type type, string path)
        {
            void** ptr = stackalloc void*[2];
            ptr[0] = (void*)UnityInternals.ObjectBaseToPtr(type);
            ptr[1] = (void*)UnityInternals.ManagedStringToInternal(path);
            IntPtr returnedException = default;
            MelonDebug.Msg("Calling runtime_invoke for GetBuiltinResource");
            IntPtr objectPointer = UnityInternals.runtime_invoke(m_GetBuiltinResource, IntPtr.Zero, ptr, ref returnedException);
            MelonDebug.Msg("returnedException: " + returnedException + ", objectPointer: " + objectPointer);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return objectPointer;
        }

        public static T GetBuiltinResource<T>(string path) where T : InternalObjectBase
        {
            MelonDebug.Msg("GetBuiltinResource<T>");
            IntPtr ptr = GetBuiltinResource(InternalType.Of<T>(), path);
            return ptr != IntPtr.Zero ? (T)typeof(T).GetConstructor(new[] { typeof(IntPtr) }).Invoke(new object[] { ptr }) : null;
        }
    }
}
