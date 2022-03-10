using System;
using System.Reflection;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace MelonLoader.Support
{
    internal class UnhollowerInterface : UnhollowerSupport.Interface
    {
        public IntPtr CopyMethodInfoStruct(IntPtr ptr)
            => UnityVersionHandler.CopyMethodInfoStruct(ptr);

        public int? GetIl2CppMethodCallerCount(MethodBase method)
        {
            CallerCountAttribute att = method.GetCustomAttribute<CallerCountAttribute>();
            if (att == null)
                return null;
            return att.Count;
        }

        public FieldInfo MethodBaseToIl2CppFieldInfo(MethodBase method)
            => UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);

        public void RegisterTypeInIl2CppDomain(Type type, bool logSuccess)
            => ClassInjector.RegisterTypeInIl2Cpp(type, logSuccess);

        public bool IsInheritedFromIl2CppObjectBase(Type type)
            => (type != null) && type.IsSubclassOf(typeof(Il2CppObjectBase));

        public bool IsInjectedType(Type type)
        {
            IntPtr ptr = GetClassPointerForType(type);
            return ptr != IntPtr.Zero && RuntimeSpecificsStore.IsInjected(ptr);
        }

        public IntPtr GetClassPointerForType(Type type)
        {
            if (type == typeof(void)) return Il2CppClassPointerStore<Il2CppSystem.Void>.NativeClassPtr;
            return (IntPtr)typeof(Il2CppClassPointerStore<>).MakeGenericType(type)
                  .GetField(nameof(Il2CppClassPointerStore<int>.NativeClassPtr)).GetValue(null);
        }
    }
}
