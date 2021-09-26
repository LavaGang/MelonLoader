using System;
using UnhollowerMini;

namespace Il2CppSystem
{
    internal class Type : Il2CppObjectBase
    {
        private static readonly IntPtr m_internal_from_handle;

        static Type()
        {
            Il2CppClassPointerStore<Type>.NativeClassPtr = IL2CPP.GetIl2CppClass("mscorlib.dll", "System", "Type");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Type>.NativeClassPtr);

            m_internal_from_handle = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Type>.NativeClassPtr, "internal_from_handle", "System.Type", "System.IntPtr");
        }

        public Type(IntPtr ptr) : base(ptr) { }

        public unsafe static Type internal_from_handle(IntPtr handle)
        {
            void** args = stackalloc void*[1];
            args[0] = &handle;
            IntPtr returnedException = default;
            IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(Type.m_internal_from_handle, IntPtr.Zero, (void**)args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return (intPtr != IntPtr.Zero) ? new Type(intPtr) : null;
        }
    }
}
