using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Mono;

internal class MonoLib
{
    private static readonly string[] folderNames =
    [
        "MonoBleedingEdge",
        "Mono",
        "MonoBleedingEdge.x64",
        "MonoBleedingEdge.x86"
    ];

    private static readonly string[] libNames =
    [
        "mono.dll",
        "mono-2.0-bdwgc.dll",
        "mono-2.0-sgen.dll",
        "mono-2.0-boehm.dll"
    ];

    private static readonly List<Delegate> passedDelegates = [];

    public required nint Handle { get; init; }
    public required bool IsOld { get; init; }

    public required nint JitInitVersionPtr { get; init; }
    public required nint RuntimeInvokePtr { get; init; }

    public required ThreadCurrentFn ThreadCurrent { get; init; }
    public required ThreadSetMainFn ThreadSetMain { get; init; }
    public required RuntimeInvokeFn RuntimeInvoke { get; init; }
    public required StringNewFn StringNew { get; init; }
    public required AssemblyGetObjectFn AssemblyGetObject { get; init; }
    public required MethodGetNameFn MethodGetName { get; init; }
    public required AddInternalCallFn AddInternalCall { get; init; }
    public required DomainAssemblyOpenFn DomainAssemblyOpen { get; init; }
    public required AssemblyGetImageFn AssemblyGetImage { get; init; }
    public required ClassFromNameFn ClassFromName { get; init; }
    public required ClassGetMethodFromNameFn ClassGetMethodFromName { get; init; }
    public required InstallAssemblyPreloadHookFn InstallAssemblyPreloadHook { get; init; }
    public required InstallAssemblySearchHookFn InstallAssemblySearchHook { get; init; }
    public required InstallAssemblyLoadHookFn InstallAssemblyLoadHook { get; init; }

    public DebugDomainCreateFn? DebugDomainCreate { get; init; }
    public DomainSetConfigFn? DomainSetConfig { get; init; }

    public static MonoLib? TryLoad(string gameDir)
    {
        var monoPath = FindMonoPath(gameDir);
        if (monoPath == null)
            return null;

        if (!NativeLibrary.TryLoad(monoPath, out var hRuntime))
            return null;

        var monoName = Path.GetFileNameWithoutExtension(monoPath);
        var isOld = monoName.Equals("mono", StringComparison.OrdinalIgnoreCase);

        if (!NativeLibrary.TryGetExport(hRuntime, "mono_jit_init_version", out var jitInitVersionPtr)
            || !NativeLibrary.TryGetExport(hRuntime, "mono_runtime_invoke", out var runtimeInvokePtr)
            || !NativeFunc.GetExport<ThreadCurrentFn>(hRuntime, "mono_thread_current", out var threadCurrent)
            || !NativeFunc.GetExport<ThreadSetMainFn>(hRuntime, "mono_thread_set_main", out var threadSetMain)
            || !NativeFunc.GetExport<StringNewFn>(hRuntime, "mono_string_new", out var stringNew)
            || !NativeFunc.GetExport<AssemblyGetObjectFn>(hRuntime, "mono_assembly_get_object", out var assemblyGetObject)
            || !NativeFunc.GetExport<MethodGetNameFn>(hRuntime, "mono_method_get_name", out var methodGetName)
            || !NativeFunc.GetExport<AddInternalCallFn>(hRuntime, "mono_add_internal_call", out var addInternalCall)
            || !NativeFunc.GetExport<DomainAssemblyOpenFn>(hRuntime, "mono_domain_assembly_open", out var domainAssemblyOpen)
            || !NativeFunc.GetExport<AssemblyGetImageFn>(hRuntime, "mono_assembly_get_image", out var assemblyGetImage)
            || !NativeFunc.GetExport<ClassFromNameFn>(hRuntime, "mono_class_from_name", out var classFromName)
            || !NativeFunc.GetExport<ClassGetMethodFromNameFn>(hRuntime, "mono_class_get_method_from_name", out var classGetMethodFromName)
            || !NativeFunc.GetExport<InstallAssemblyPreloadHookFn>(hRuntime, "mono_install_assembly_preload_hook", out var installAssemblyPreloadHook)
            || !NativeFunc.GetExport<InstallAssemblySearchHookFn>(hRuntime, "mono_install_assembly_search_hook", out var installAssemblySearchHook)
            || !NativeFunc.GetExport<InstallAssemblyLoadHookFn>(hRuntime, "mono_install_assembly_load_hook", out var installAssemblyLoadHook))
            return null;

        var runtimeInvoke = Marshal.GetDelegateForFunctionPointer<RuntimeInvokeFn>(runtimeInvokePtr);

        var debugDomainCreate = NativeFunc.GetExport<DebugDomainCreateFn>(hRuntime, "mono_debug_domain_create");
        var domainSetConfig = NativeFunc.GetExport<DomainSetConfigFn>(hRuntime, "mono_domain_set_config");

        return new()
        {
            Handle = hRuntime,
            IsOld = isOld,
            RuntimeInvoke = runtimeInvoke,
            JitInitVersionPtr = jitInitVersionPtr,
            RuntimeInvokePtr = runtimeInvokePtr,
            ThreadCurrent = threadCurrent,
            ThreadSetMain = threadSetMain,
            StringNew = stringNew,
            AssemblyGetObject = assemblyGetObject,
            MethodGetName = methodGetName,
            AddInternalCall = addInternalCall,
            DomainAssemblyOpen = domainAssemblyOpen,
            AssemblyGetImage = assemblyGetImage,
            ClassFromName = classFromName,
            ClassGetMethodFromName = classGetMethodFromName,
            InstallAssemblyPreloadHook = installAssemblyPreloadHook,
            InstallAssemblySearchHook = installAssemblySearchHook,
            InstallAssemblyLoadHook = installAssemblyLoadHook,
            DomainSetConfig = domainSetConfig,
            DebugDomainCreate = debugDomainCreate
        };
    }

    private static string? FindMonoPath(string gameDir)
    {
        foreach (var folder in folderNames)
        {
            foreach (var lib in libNames)
            {
                var path = Path.Combine(gameDir, folder, lib);
                if (File.Exists(path))
                    return path;

                path = Path.Combine(gameDir, folder, "EmbedRuntime", lib);
                if (File.Exists(path))
                    return path;

                path = Path.Combine(gameDir, folder, lib);
                if (File.Exists(path))
                    return path;

                path = Path.Combine(gameDir, folder, "EmbedRuntime", lib);
                if (File.Exists(path))
                    return path;

                path = Path.Combine(gameDir, folder, "x86_64", lib);
                if (File.Exists(path))
                    return path;
            }
        }

        return null;
    }

    public void SetCurrentThreadAsMain()
    {
        ThreadSetMain(ThreadCurrent());
    }

    public void AddManagedInternalCall<TDelegate>(string name, TDelegate func) where TDelegate : Delegate
    {
        passedDelegates.Add(func);
        AddInternalCall(name, Marshal.GetFunctionPointerForDelegate(func));
    }

    public string? GetMethodName(nint method)
    {
        if (method == 0)
            return null;

        return Marshal.PtrToStringAnsi(MethodGetName(method));
    }

    public void InstallAssemblyHooks(AssemblyPreloadHookFn? preloadHook, AssemblySearchHookFn? searchHook, AssemblyLoadHookFn? loadHook)
    {
        if (preloadHook != null)
        {
            passedDelegates.Add(preloadHook);
            InstallAssemblyPreloadHook(preloadHook, 0);
        }

        if (searchHook != null)
        {
            passedDelegates.Add(searchHook);
            InstallAssemblySearchHook(searchHook, 0);
        }

        if (loadHook != null)
        {
            passedDelegates.Add(loadHook);
            InstallAssemblyLoadHook(loadHook, 0);
        }
    }

    public delegate nint DomainGetFn();
    public delegate nint JitInitVersionFn(nint name, nint b);
    public delegate void DebugDomainCreateFn(nint domain);
    public delegate nint ThreadCurrentFn();
    public delegate void ThreadSetMainFn(nint thread);
    public delegate nint AssemblyGetImageFn(nint assembly);
    public unsafe delegate nint RuntimeInvokeFn(nint method, nint obj, void** args, ref nint ex);
    public delegate nint AssemblyGetObjectFn(nint domain, nint assembly);

    public delegate nint InstallAssemblyPreloadHookFn(AssemblyPreloadHookFn func, nint userData);
    public delegate nint InstallAssemblySearchHookFn(AssemblySearchHookFn func, nint userData);
    public delegate nint InstallAssemblyLoadHookFn(AssemblyLoadHookFn func, nint userData);
    public delegate nint MethodGetNameFn(nint method);
    public unsafe delegate void* StringNewFn(nint domain, nint value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void AddInternalCallFn(string name, nint func);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint ClassFromNameFn(nint image, string nameSpace, string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint ClassGetMethodFromNameFn(nint clas, string name, int paramCount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint DomainAssemblyOpenFn(nint domain, string path);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void DomainSetConfigFn(nint domain, string configPath, nint name);

    public delegate nint AssemblyPreloadHookFn(ref AssemblyName name, nint assemblyPaths, nint userData);
    public delegate nint AssemblySearchHookFn(ref AssemblyName name, nint userData);
    public delegate void AssemblyLoadHookFn(nint monoAssembly, nint userData);

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AssemblyName
    {
        public nint Name;

        // Non-marshalled strings
        public nint Culture;
        public nint HashValue;
        public nint PublicKey;

        public fixed byte PublicKeyToken[17];

        public uint HashAlg;
        public uint HashLength;

        public uint Flags;
        public ushort Major;
        public ushort Minor;
        public ushort Build;
        public ushort Revision;
        public uint Arch;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ReflectionAssembly
    {
        // MonoObject
        public nint VTable;
        public nint Sync;

        public nint Assembly;

        public nint Evidence;
    }
}
