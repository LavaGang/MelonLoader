using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MelonLoader
{
    internal class UnhollowerSupport
    {
        private static Assembly UnhollowerBaseLib = null;
        private static Type Il2CppObjectBaseType = null;
        internal static MethodInfo Il2CppObjectBaseToPtrMethod = null;

        internal static void Initialize()
        {
            UnhollowerBaseLib = Assembly.Load("UnhollowerBaseLib");
            Il2CppObjectBaseType = UnhollowerBaseLib.GetType("UnhollowerBaseLib.Il2CppObjectBase");
            Il2CppObjectBaseToPtrMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("Il2CppObjectBaseToPtr");
        }

        internal static bool IsGeneratedAssemblyType(Type type) => (Imports.IsIl2CppGame() && !Il2CppObjectBaseType.Equals(null) && !type.Equals(null) && type.IsSubclassOf(Il2CppObjectBaseType));

        internal static IntPtr MethodBaseToIntPtr(MethodBase method)
        {
            FieldInfo methodptr = method.DeclaringType.GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.StartsWith(("NativeMethodInfoPtr_" + method.Name)));
            if (methodptr != null)
                return (IntPtr)methodptr.GetValue(null);
            return IntPtr.Zero;
        }
    }
}
