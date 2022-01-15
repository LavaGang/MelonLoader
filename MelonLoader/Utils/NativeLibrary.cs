using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    public class NativeLibrary
    {
        public readonly IntPtr Ptr;
        public NativeLibrary(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Ptr = ptr;
        }

        public static NativeLibrary Load(string filepath)
            => LoadLib(filepath).ToNewNativeLibrary();
        public static NativeLibrary<T> Load<T>(string filepath)
            => LoadLib(filepath).ToNewNativeLibrary<T>();
        public static T ReflectiveLoad<T>(string filepath)
            => Load<T>(filepath).Instance;
        public static IntPtr LoadLib(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));
            IntPtr ptr = LoadLibrary(filepath);
            if (ptr == IntPtr.Zero)
                throw new Exception($"Unable to Load Native Library {filepath}!");
            return ptr;
        }

        public IntPtr GetExport(string name)
            => GetExport(Ptr, name);
        public Delegate GetExport(Type type, string name)
            => GetExport(name).GetDelegate(type);
        public T GetExport<T>(string name) where T : Delegate
            => GetExport(name).GetDelegate<T>();
        public void GetExport<T>(string name, out T output) where T : Delegate
            => output = GetExport<T>(name);
        public static IntPtr GetExport(IntPtr nativeLib, string name)
        {
            if (nativeLib == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nativeLib));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            IntPtr returnval = GetProcAddress(nativeLib, name);
            if (returnval == IntPtr.Zero)
                throw new Exception($"Unable to Find Native Library Export {name}!");

            return returnval;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);
        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    }

    public class NativeLibrary<T> : NativeLibrary
    {
        public readonly T Instance;

        public NativeLibrary(IntPtr ptr) : base(ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));

            Type specifiedType = typeof(T);
            if (specifiedType.IsAbstract && specifiedType.IsSealed)
                throw new Exception($"Specified Type {specifiedType.FullName} must be Non-Static!");

            Instance = (T)Activator.CreateInstance(specifiedType);

            FieldInfo[] fields = specifiedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields.Length <= 0)
                return;

            foreach (FieldInfo fieldInfo in fields)
            {
                Type fieldType = fieldInfo.FieldType;
                if (fieldType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false).Length <= 0)
                    continue;

                fieldInfo.SetValue(Instance, GetExport(fieldType, fieldInfo.Name));
            }
        }
    }
}
