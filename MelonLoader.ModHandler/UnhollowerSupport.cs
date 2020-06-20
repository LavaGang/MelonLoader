using System;
using System.Reflection;

namespace MelonLoader
{
    internal class UnhollowerSupport
    {
        private static Assembly UnhollowerBaseLib = null;
        private static Type Il2CppObjectBaseType = null;
        internal static Type Il2CppMethodInfoType = null;
        internal static MethodInfo Il2CppObjectBaseToPtrMethod = null;
        internal static MethodInfo Il2CppStringToManagedMethod = null;
        internal static MethodInfo ManagedStringToIl2CppMethod = null;
        internal static MethodInfo GetIl2CppMethodInfoPointerFieldForGeneratedMethod = null;

        internal static void Initialize()
        {
            UnhollowerBaseLib = Assembly.Load("UnhollowerBaseLib");
            Il2CppObjectBaseType = UnhollowerBaseLib.GetType("UnhollowerBaseLib.Il2CppObjectBase");
            Il2CppMethodInfoType = UnhollowerBaseLib.GetType("UnhollowerBaseLib.Runtime.Il2CppMethodInfo");
            Il2CppObjectBaseToPtrMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("Il2CppObjectBaseToPtr");
            Il2CppStringToManagedMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("Il2CppStringToManaged");
            ManagedStringToIl2CppMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("ManagedStringToIl2Cpp");
            GetIl2CppMethodInfoPointerFieldForGeneratedMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.UnhollowerUtils").GetMethod("GetIl2CppMethodInfoPointerFieldForGeneratedMethod");
        }

        internal static bool IsGeneratedAssemblyType(Type type) => (Imports.IsIl2CppGame() && !Il2CppObjectBaseType.Equals(null) && !type.Equals(null) && type.IsSubclassOf(Il2CppObjectBaseType));

        internal static IntPtr MethodBaseToIntPtr(MethodBase method)
        {
            FieldInfo methodPtr = (FieldInfo) GetIl2CppMethodInfoPointerFieldForGeneratedMethod.Invoke(null, new object[] { method });
            if (methodPtr == null) {
                throw new NotSupportedException($"Cannot patch method {method.Name} as there is no corresponding IL2CPP method");
            }
            return (IntPtr) methodPtr.GetValue(null);
        }
    }
}
