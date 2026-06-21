using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Reflection;

namespace MelonLoader.Support
{
    internal class InteropInterface : InteropSupport.Interface
    {
        public IntPtr CopyMethodInfoStruct(IntPtr ptr)
            => ptr;
            //=> UnityVersionHandler.CopyMethodInfoStruct(ptr);

        public FieldInfo MethodBaseToIl2CppFieldInfo(MethodBase method)
            => Il2CppInternalsAccess.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);

        public bool IsInheritedFromIl2CppObjectBase(Type type)
            => (type != null) && typeof(Il2CppSystem.IObject).IsAssignableFrom(type);

        public bool IsInjectedType(Type type)
        {
            IntPtr ptr = GetClassPointerForType(type);
            return ptr != IntPtr.Zero && !TypeInjector.IsPreexistingType(type);
        }

        public IntPtr GetClassPointerForType(Type type)
        {
            return Il2CppType.GetClassPointer(type);
        }
    }
}
