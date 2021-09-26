using System;

namespace UnhollowerMini
{
    internal static class Il2CppType
    {
        public static Il2CppSystem.Type TypeFromPointer(IntPtr classPointer, string typeName = "<unknown type>")
        {
            if (classPointer == IntPtr.Zero) throw new ArgumentException($"{typeName} does not have a corresponding IL2CPP class pointer");
            var il2CppType = IL2CPP.il2cpp_class_get_type(classPointer);
            if (il2CppType == IntPtr.Zero) throw new ArgumentException($"{typeName} does not have a corresponding IL2CPP type pointer");
            return Il2CppSystem.Type.internal_from_handle(il2CppType);
        }

        public static Il2CppSystem.Type Of<T>()
        {
            var classPointer = Il2CppClassPointerStore<T>.NativeClassPtr;
            return TypeFromPointer(classPointer, typeof(T).Name);
        }
    }
}
