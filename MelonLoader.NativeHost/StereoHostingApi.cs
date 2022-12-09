using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.NativeHost
{
    /// <summary>
    /// It's mono but better, geddit
    /// </summary>
    internal static class StereoHostingApi
    {
        private const int ID_OUT_OF_BOUNDS = -1000;
        private const int TYPE_NAME_NOT_FOUND = -1001;
        private const int METHOD_NAME_NOT_FOUND = -1002;

        private delegate Assembly DelegateInternalLoad(ReadOnlySpan<byte> arrAssembly, ReadOnlySpan<byte> arrSymbols);

        private static readonly MethodInfo AlcInternalLoad = typeof(AssemblyLoadContext).GetMethod("InternalLoad", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new("Failed to get ALC.InternalLoad");
        private static DelegateInternalLoad DefaultContextInternalLoad = AlcInternalLoad.CreateDelegate<DelegateInternalLoad>(AssemblyLoadContext.Default);

        private static List<Assembly> _loadedAssemblies = new();
        private static List<Type> _loadedTypes = new();
        private static List<object> _allocatedObjects = new();

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

        [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvStdcall)})]
        internal static unsafe int LoadAssemblyFromByteArray(IntPtr baseOfArray, int arrayLength)
        {
            var managedArray = new byte[arrayLength];
            Marshal.Copy(baseOfArray, managedArray, 0, arrayLength);

            var asm = DefaultContextInternalLoad(managedArray, ReadOnlySpan<byte>.Empty);

            var ret = _loadedAssemblies.Count;
            _loadedAssemblies.Add(asm);
            return ret;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        internal static unsafe int GetTypeByName(int assemblyId, IntPtr namePtr)
        {
            if (assemblyId < 0 || assemblyId >= _loadedAssemblies.Count)
                return ID_OUT_OF_BOUNDS;

            var asm = _loadedAssemblies[assemblyId];
            var name = Marshal.PtrToStringUni(namePtr);
            
            ArgumentNullException.ThrowIfNull(name);
            
            var type = asm.GetType(name);
            if (type == null)
            {
                Console.WriteLine($"[Stereo] Couldn't find type with name '{name}'");
                return TYPE_NAME_NOT_FOUND;
            }

            var ret = _loadedTypes.Count;
            _loadedTypes.Add(type);
            return ret;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        internal static unsafe int ConstructType(int typeId, int numParams, IntPtr* pParamTypes, IntPtr* pParamValues)
        {
            if (typeId < 0 || typeId >= _loadedTypes.Count)
                return ID_OUT_OF_BOUNDS;

            var type = _loadedTypes[typeId];

            int ret;
            if(numParams == 0)
            {
                ret = _allocatedObjects.Count;
                _allocatedObjects.Add(Activator.CreateInstance(type)!);
                return ret;
            }

            var paramTypes = new string[numParams];
            for (var i = 0; i < numParams; i++)
                paramTypes[i] = Marshal.PtrToStringUni(pParamTypes[i]) ?? throw new($"Null parameter type at index {i}");
            
            var paramValues = new object?[numParams];
            for (var i = 0; i < numParams; i++)
                paramValues[i] = GetParam(paramTypes[i], pParamValues[i]);

            ret = _allocatedObjects.Count;
            _allocatedObjects.Add(Activator.CreateInstance(type, paramValues)!);
            return ret;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        internal static unsafe int InvokeMethod(int typeId, IntPtr pMethodName, int instanceId, int numParams, IntPtr* pParamTypes, IntPtr* pParamValues)
        {
            if (typeId < 0 || typeId >= _loadedTypes.Count)
                return ID_OUT_OF_BOUNDS;

            var type = _loadedTypes[typeId];

            object? instance;
            if (instanceId < 0)
                instance = null;
            else if (instanceId >= _allocatedObjects.Count)
                return ID_OUT_OF_BOUNDS;
            else
                instance = _allocatedObjects[instanceId];

            object?[] paramValues;
            string[] paramTypeNames;
            if (numParams == 0)
            {
                paramTypeNames = Array.Empty<string>();
                paramValues = Array.Empty<object>();
            }
            else
            {
                paramTypeNames = new string[numParams];
                for (var i = 0; i < numParams; i++)
                    paramTypeNames[i] = Marshal.PtrToStringUni(pParamTypes[i]) ?? throw new($"Null parameter type at index {i}");

                paramValues = new object?[numParams];
                for (var i = 0; i < numParams; i++)
                    paramValues[i] = GetParam(paramTypeNames[i], pParamValues[i]);
            }

            var paramTypes = paramTypeNames.Select(p => $"System.{GetSystemTypeName(p)}").Select(Type.GetType).ToArray();
            var methodName = Marshal.PtrToStringUni(pMethodName)!;

            var method = type.GetMethod(methodName, (BindingFlags)(-1), paramTypes!);

            if (method == null)
            {
                Console.WriteLine($"[Stereo] Couldn't find method with name '{methodName}', param types {string.Join<Type?>(", ", paramTypes)}");
                return METHOD_NAME_NOT_FOUND;
            }

            method.Invoke(instance, paramValues);

            return 0; //Success
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        internal static unsafe nuint GetPointerToUcoMethod(int typeId, IntPtr pMethodName, int numParams, IntPtr* pParamTypes)
        {
            if (typeId < 0 || typeId >= _loadedTypes.Count)
                return 0;

            var type = _loadedTypes[typeId];

            string[] paramTypeNames;
            if (numParams == 0)
            {
                paramTypeNames = Array.Empty<string>();
            }
            else
            {
                paramTypeNames = new string[numParams];
                for (var i = 0; i < numParams; i++)
                    paramTypeNames[i] = Marshal.PtrToStringUni(pParamTypes[i]) ?? throw new($"Null parameter type at index {i}");
            }

            var paramTypes = paramTypeNames.Select(p => $"System.{GetSystemTypeName(p)}").Select(Type.GetType).ToArray();
            var methodName = Marshal.PtrToStringUni(pMethodName)!;

            var method = type.GetMethod(methodName, (BindingFlags)(-1), paramTypes!);

            if(method == null)
            {
                Console.WriteLine($"[Stereo] Couldn't find method with name '{methodName}', param types {string.Join<Type?>(", ", paramTypes)}");
                return 0;
            }

            if(method.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() == null)
            {
                Console.WriteLine($"[Stereo] {method} is not annotated as UnmanagedCallersOnly, so cannot have a pointer returned from GetPointerToUcoMethod");
                return 0;
            }

            return (nuint) method.MethodHandle.GetFunctionPointer().ToInt64();
        }

        private static string GetSystemTypeName(string primitiveName)
            => primitiveName switch
            {
                "byte" => "Byte",
                "sbyte" => "SByte",
                "ushort" => "UInt16",
                "short" => "Int16",
                "uint" => "UInt32",
                "int" => "Int32",
                "ulong" => "UInt64",
                "long" => "Int64",
                "float" => "Single",
                "double" => "Double",
                "string" => "String",
                _ => primitiveName,
            };

        private static unsafe object? GetParam(string paramType, IntPtr pParam)
        =>
            paramType switch
            {
                "byte" => *(byte*)pParam,
                "sbyte" => *(sbyte*)pParam,
                "ushort" => *(ushort*)pParam,
                "short" => *(short*)pParam,
                "uint" => *(uint*)pParam,
                "int" => *(int*)pParam,
                "ulong" => *(ulong*)pParam,
                "long" => *(long*)pParam,
                "float" => *(float*)pParam,
                "double" => *(double*)pParam,
                "string" => Marshal.PtrToStringUni(pParam),
                _ => throw new($"Unknown param type {paramType}")
            };
       
    }
}
