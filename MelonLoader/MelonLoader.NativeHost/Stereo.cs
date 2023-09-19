using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.NativeHost;

public static class Stereo
{
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    internal static unsafe void LoadAssemblyAndGetFuncPtr(IntPtr pathNative, IntPtr typeNameNative, IntPtr methodNameNative, void** resultHandle)
    {
        var assemblyPath = Marshal.PtrToStringUni(pathNative);
        var typeName = Marshal.PtrToStringUni(typeNameNative);
        var methodName = Marshal.PtrToStringUni(methodNameNative);
            
        ArgumentNullException.ThrowIfNull(assemblyPath);
        ArgumentNullException.ThrowIfNull(typeName);
        ArgumentNullException.ThrowIfNull(methodName);

        if ((IntPtr)resultHandle == IntPtr.Zero)
            throw new ArgumentNullException(nameof(resultHandle));

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

        Func<AssemblyName, Assembly> resolver = name => AssemblyLoadContext.Default.LoadFromAssemblyName(name);

        var type = Type.GetType(typeName, resolver, null, true);
            
        if(type == null)
            throw new TypeLoadException("Failed to load type: " + typeName);
            
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
            throw new MissingMethodException(typeName, methodName);

        *resultHandle = (void*)method.MethodHandle.GetFunctionPointer();
    }
}