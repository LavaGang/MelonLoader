using MelonLoader;
using System;

namespace UnhollowerMini
{
    internal static class InternalType
    {
        public static Il2CppSystem.Type TypeFromPointer(IntPtr classPointer, string typeName = "<unknown type>")
        {
            if (classPointer == IntPtr.Zero) throw new ArgumentException($"{typeName} does not have a corresponding internal class pointer");
            var il2CppType = UnityInternals.class_get_type(classPointer);
            if (il2CppType == IntPtr.Zero) throw new ArgumentException($"{typeName} does not have a corresponding class type pointer");
            return Il2CppSystem.Type.internal_from_handle(il2CppType);
        }

        public static Il2CppSystem.Type Of<T>()
        {
            var classPointer = InternalClassPointerStore<T>.NativeClassPtr;
            return TypeFromPointer(classPointer, typeof(T).Name);
        }
    }
}
