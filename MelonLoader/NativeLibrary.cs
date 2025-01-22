using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if LINUX
using System.IO;
#endif

namespace MelonLoader;

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
        var ptr = AgnosticLoadLibrary(filepath);
        return ptr == IntPtr.Zero ? throw new Exception($"Unable to Load Native Library {filepath}!") : ptr;
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

        var returnval = AgnosticGetProcAddress(nativeLib, name);
        return returnval == IntPtr.Zero ? throw new Exception($"Unable to Find Native Library Export {name}!") : returnval;
    }

    public static IntPtr AgnosticLoadLibrary(string name)
    {
#if WINDOWS
        return LoadLibrary(name);
#elif LINUX
        if (!Path.HasExtension(name))
            name += ".so";
        
        return dlopen(name, RTLD_NOW);
#endif
    }

    public static IntPtr AgnosticGetProcAddress(IntPtr hModule, string lpProcName)
    {
#if WINDOWS
        return GetProcAddress(hModule, lpProcName);
#elif LINUX
        return dlsym(hModule, lpProcName);
#endif
    }

#if WINDOWS
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpLibFileName);
    [DllImport("kernel32")]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    [DllImport("kernel32")]
    internal static extern IntPtr FreeLibrary(IntPtr hModule);
#elif LINUX
    [DllImport("libdl.so.2")]
    protected static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so.2")]
    protected static extern IntPtr dlsym(IntPtr handle, string symbol);

    const int RTLD_NOW = 2; // for dlopen's flags 
#endif

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

        var specifiedType = typeof(T);
        if (specifiedType.IsAbstract && specifiedType.IsSealed)
            throw new Exception($"Specified Type {specifiedType.FullName} must be Non-Static!");

        Instance = (T)Activator.CreateInstance(specifiedType);

        foreach (var fieldInfo in specifiedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (fieldInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                continue;

            var fieldType = fieldInfo.FieldType;
            if (fieldType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false).Length == 0)
                continue;

            fieldInfo.SetValue(Instance, GetExport(fieldType, fieldInfo.Name));
        }

        foreach (var propertyInfo in specifiedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var fieldType = propertyInfo.PropertyType;
            if (fieldType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false).Length == 0)
                continue;

            propertyInfo.SetValue(Instance, GetExport(fieldType, propertyInfo.Name), null);
        }
    }
}
