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
            IntPtr ptr = AgnosticLoadLibrary(filepath);
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

            IntPtr returnval = AgnosticGetProcAddress(nativeLib, name);
            if (returnval == IntPtr.Zero)
                throw new Exception($"Unable to Find Native Library Export {name}!");

            return returnval;
        }

        public static IntPtr AgnosticLoadLibrary(string name)
        {
            var platform = Environment.OSVersion.Platform;

            switch (platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return LoadLibrary(name);
                
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return dlopen(name, RTLD_NOW);
            }
            
            throw new PlatformNotSupportedException($"Unsupported platform: {platform}");
        }

        public static IntPtr AgnosticGetProcAddress(IntPtr hModule, string lpProcName)
        {
            var platform = Environment.OSVersion.Platform;

            switch (platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return GetProcAddress(hModule, lpProcName);
                
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return dlsym(hModule, lpProcName);
            }
            
            throw new PlatformNotSupportedException($"Unsupported platform: {platform}");
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);
        [DllImport("kernel32")]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32")]
        internal static extern IntPtr FreeLibrary(IntPtr hModule);
        
        [DllImport("libdl.so.2")]
        protected static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so.2")]
        protected static extern IntPtr dlsym(IntPtr handle, string symbol);

        const int RTLD_NOW = 2; // for dlopen's flags 
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal delegate string StringDelegate();
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
