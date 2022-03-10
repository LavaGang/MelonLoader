using System;
using System.Reflection;

namespace MelonLoader
{
    public static class UnhollowerSupport
    {
        public interface Interface
        {
            bool IsInheritedFromIl2CppObjectBase(Type type);
            public bool IsInjectedType(Type type);
            public IntPtr GetClassPointerForType(Type type);
            FieldInfo MethodBaseToIl2CppFieldInfo(MethodBase method);
            int? GetIl2CppMethodCallerCount(MethodBase method);
            void RegisterTypeInIl2CppDomain(Type type, bool logSuccess);
            IntPtr CopyMethodInfoStruct(IntPtr ptr);
        }
        internal static Interface SMInterface;

        private static void ValidateInterface()
        {
            if (!MelonUtils.IsGameIl2Cpp())
                throw new Exception("MelonLoader.UnhollowerSupport can't be used on Non-Il2Cpp Games");
            if (SMInterface == null)
                throw new NullReferenceException("SMInterface cannot be null.");
        }

        public static bool IsGeneratedAssemblyType(Type type)
            => IsInheritedFromIl2CppObjectBase(type) && !IsInjectedType(type);

        public static bool IsInheritedFromIl2CppObjectBase(Type type)
        {
            ValidateInterface();
            if (type == null)
                throw new NullReferenceException("The type cannot be null.");
            return SMInterface.IsInheritedFromIl2CppObjectBase(type);
        }

        public static bool IsInjectedType(Type type)
        {
            ValidateInterface();
            if (type == null)
                throw new NullReferenceException("The type cannot be null.");
            return SMInterface.IsInjectedType(type);
        }

        public static IntPtr GetClassPointerForType(Type type)
        {
            ValidateInterface();
            if (type == null)
                throw new NullReferenceException("The type cannot be null.");
            return SMInterface.GetClassPointerForType(type);
        }

        public static IntPtr MethodBaseToIl2CppMethodInfoPointer(MethodBase method)
        {
            ValidateInterface();
            if (method == null)
                throw new NullReferenceException("The method cannot be null.");
            FieldInfo field = MethodBaseToIl2CppFieldInfo(method);
            if (field == null)
                return IntPtr.Zero;
            return (IntPtr)field.GetValue(null);
        }

        public static FieldInfo MethodBaseToIl2CppFieldInfo(MethodBase method)
        {
            ValidateInterface();
            if (method == null)
                throw new NullReferenceException("The method cannot be null.");
            return SMInterface.MethodBaseToIl2CppFieldInfo(method);
        }

        public static T Il2CppObjectPtrToIl2CppObject<T>(IntPtr ptr)
        {
            ValidateInterface();
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException("The ptr cannot be IntPtr.Zero.");
            if (!IsGeneratedAssemblyType(typeof(T)))
                throw new NullReferenceException("The type must be a Generated Assembly Type.");
            return (T)typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IntPtr) }, new ParameterModifier[0]).Invoke(new object[] { ptr });
        }

        public static int? GetIl2CppMethodCallerCount(MethodBase method)
        {
            ValidateInterface();
            if (method == null)
                throw new NullReferenceException("The method cannot be null.");
            return SMInterface.GetIl2CppMethodCallerCount(method);
        }

        public static void RegisterTypeInIl2CppDomain(Type type) => RegisterTypeInIl2CppDomain(type, true);
        public static void RegisterTypeInIl2CppDomain(Type type, bool logSuccess)
        {
            ValidateInterface();
            if (type == null)
                throw new NullReferenceException("The type cannot be null.");
            SMInterface.RegisterTypeInIl2CppDomain(type, logSuccess);
        }

        public static IntPtr CopyMethodInfoStruct(IntPtr ptr)
        {
            ValidateInterface();
            return SMInterface.CopyMethodInfoStruct(ptr);
        }
    }
}