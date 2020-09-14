using System;
using System.Reflection;

namespace MelonLoader
{
    internal static class Unhollower
    {
        internal static Type Il2CppObjectBaseType = null;
        internal static Type Il2CppMethodInfoType = null;
        internal static MethodInfo Il2CppObjectBaseToPtrMethod = null;
        internal static MethodInfo Il2CppStringToManagedMethod = null;
        internal static MethodInfo ManagedStringToIl2CppMethod = null;
        internal static MethodInfo GetIl2CppMethodInfoPointerFieldForGeneratedMethod = null;
        static Unhollower()
        {
            if (!MelonUtils.IsGameIl2Cpp())
                return;
            Assembly UnhollowerBaseLib = Assembly.Load("UnhollowerBaseLib");
            if (UnhollowerBaseLib == null)
            {
                MelonLogger.ThrowInternalFailure("Failed to Load Assembly for UnhollowerBaseLib!");
                return;
            }
            Il2CppObjectBaseType = UnhollowerBaseLib.GetType("UnhollowerBaseLib.Il2CppObjectBase");
            Il2CppMethodInfoType = UnhollowerBaseLib.GetType("UnhollowerBaseLib.Runtime.Il2CppMethodInfo");
            Il2CppObjectBaseToPtrMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("Il2CppObjectBaseToPtr");
            Il2CppStringToManagedMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("Il2CppStringToManaged");
            ManagedStringToIl2CppMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP").GetMethod("ManagedStringToIl2Cpp");
            GetIl2CppMethodInfoPointerFieldForGeneratedMethod = UnhollowerBaseLib.GetType("UnhollowerBaseLib.UnhollowerUtils").GetMethod("GetIl2CppMethodInfoPointerFieldForGeneratedMethod");
        }
        internal static bool IsGeneratedAssemblyType(Type type) => ((Il2CppObjectBaseType != null) && (type != null) && type.IsSubclassOf(Il2CppObjectBaseType));
        internal static IntPtr MethodToIntPtr(MethodBase method)
        {
            if (method == null)
                throw new NullReferenceException("The method cannot be null.");
            if (!IsGeneratedAssemblyType(method.DeclaringType))
                return method.MethodHandle.GetFunctionPointer();
            FieldInfo methodPtr = (FieldInfo)GetIl2CppMethodInfoPointerFieldForGeneratedMethod.Invoke(null, new object[] { method });
            if (methodPtr == null)
                throw new NotSupportedException($"Cannot get IntPtr for {method.Name} as there is no corresponding IL2CPP method");
            return (IntPtr)methodPtr.GetValue(null);
        }
    }
}
