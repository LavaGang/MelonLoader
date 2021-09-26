using System;
using System.Runtime.CompilerServices;

namespace UnhollowerMini
{
    internal static class Il2CppClassPointerStore<T>
    {
        public static IntPtr NativeClassPtr;
        public static Type CreatedTypeRedirect;

        static Il2CppClassPointerStore()
        {
            var targetType = typeof(T);
            RuntimeHelpers.RunClassConstructor(targetType.TypeHandle);

            if (targetType.IsPrimitive || targetType == typeof(string))
                RuntimeHelpers.RunClassConstructor(typeof(Il2CppClassPointerStore<>).Assembly.GetType("Il2Cpp" + targetType.FullName).TypeHandle);
        }
    }
}
