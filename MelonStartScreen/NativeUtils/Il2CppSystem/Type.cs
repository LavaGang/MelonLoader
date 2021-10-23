using MelonLoader;
using System;
using UnhollowerMini;

namespace Il2CppSystem
{
    internal class Type : InternalObjectBase
    {
        private static readonly IntPtr m_internal_from_handle;

        static Type()
        {
            InternalClassPointerStore<Type>.NativeClassPtr = UnityInternals.GetClass("mscorlib.dll", "System", "Type");

            m_internal_from_handle = UnityInternals.GetMethod(InternalClassPointerStore<Type>.NativeClassPtr, "internal_from_handle", "System.Type", "System.IntPtr");
        }

        public Type(IntPtr ptr) : base(ptr) { }

        public unsafe static Type internal_from_handle(IntPtr handle)
        {
            void** args = stackalloc void*[1];
            args[0] = &handle;
            IntPtr returnedException = default;
            IntPtr intPtr = UnityInternals.runtime_invoke(Type.m_internal_from_handle, IntPtr.Zero, (void**)args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return (intPtr != IntPtr.Zero) ? new Type(intPtr) : null;
        }
    }
}
