﻿using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Mono;

internal class MonoLib
{
    private static readonly List<Delegate> passedDelegates = [];

    public bool IsOld { get; set; }
    public required nint Handle { get; init; }

    public required JitInitVersionFn JitInitVersion { get; init; }
    public required JitParseOptionsFn JitParseOptions { get; init; }
    public required ImageOpenFromDataWithNameFn ImageOpenFromDataWithName { get; init; }

    public required ThreadCurrentFn ThreadCurrent { get; init; }
    public required DebugInitFn DebugInit { get; init; }
    public required ConfigParseFn ConfigParse { get; init; }
    public required ThreadSetMainFn ThreadSetMain { get; init; }
    public required RuntimeInvokeFn RuntimeInvoke { get; init; }
    public required StringNewFn StringNew { get; init; }
    public required AssemblyGetObjectFn AssemblyGetObject { get; init; }
    public required SetAssembliesPathFn SetAssembliesPath { get; init; }
    public required AssemblyGetrootdirFn AssemblyGetrootdir { get; init; }
    public required MethodGetNameFn MethodGetName { get; init; }
    public required AddInternalCallFn AddInternalCall { get; init; }
    public required DomainAssemblyOpenFn DomainAssemblyOpen { get; init; }
    public required AssemblyGetImageFn AssemblyGetImage { get; init; }
    public required ClassFromNameFn ClassFromName { get; init; }
    public required ClassGetMethodFromNameFn ClassGetMethodFromName { get; init; }
    public required InstallAssemblySearchHookFn InstallAssemblySearchHook { get; init; }
    public required FreeFn Free { get; init; }
    public required StringToUtf8Fn StringToUtf8 { get; init; }
    public required ObjectGetClassFn ObjectGetClass { get; init; }

    public DomainSetConfigFn? DomainSetConfig { get; init; }
    public DebugEnabledFn? DebugEnabled { get; init; }
    public ObjectToStringFn? ObjectToString { get; init; }

    public static MonoLib? TryLoad(nint hRuntime)
    {
        MelonDebug.Log("Loading Mono exports");

        if (!NativeFunc.GetExport<JitInitVersionFn>(hRuntime, "mono_jit_init_version", out var jitInitVersion)
            || !NativeFunc.GetExport<RuntimeInvokeFn>(hRuntime, "mono_runtime_invoke", out var runtimeInvoke)
            || !NativeFunc.GetExport<JitParseOptionsFn>(hRuntime, "mono_jit_parse_options", out var jitParseOptions)
            || !NativeFunc.GetExport<DebugInitFn>(hRuntime, "mono_debug_init", out var debugInit)
            || !NativeFunc.GetExport<ImageOpenFromDataWithNameFn>(hRuntime, "mono_image_open_from_data_with_name", out var imageOpenFromDataWithName)
            || !NativeFunc.GetExport<ConfigParseFn>(hRuntime, "mono_config_parse", out var configParse)
            || !NativeFunc.GetExport<ThreadCurrentFn>(hRuntime, "mono_thread_current", out var threadCurrent)
            || !NativeFunc.GetExport<ThreadSetMainFn>(hRuntime, "mono_thread_set_main", out var threadSetMain)
            || !NativeFunc.GetExport<StringNewFn>(hRuntime, "mono_string_new", out var stringNew)
            || !NativeFunc.GetExport<AssemblyGetObjectFn>(hRuntime, "mono_assembly_get_object", out var assemblyGetObject)
            || !NativeFunc.GetExport<MethodGetNameFn>(hRuntime, "mono_method_get_name", out var methodGetName)
            || !NativeFunc.GetExport<SetAssembliesPathFn>(hRuntime, "mono_set_assemblies_path", out var setAssembliesPath)
            || !NativeFunc.GetExport<AssemblyGetrootdirFn>(hRuntime, "mono_assembly_getrootdir", out var assemblyGetRootDir)
            || !NativeFunc.GetExport<AddInternalCallFn>(hRuntime, "mono_add_internal_call", out var addInternalCall)
            || !NativeFunc.GetExport<DomainAssemblyOpenFn>(hRuntime, "mono_domain_assembly_open", out var domainAssemblyOpen)
            || !NativeFunc.GetExport<AssemblyGetImageFn>(hRuntime, "mono_assembly_get_image", out var assemblyGetImage)
            || !NativeFunc.GetExport<ClassFromNameFn>(hRuntime, "mono_class_from_name", out var classFromName)
            || !NativeFunc.GetExport<ClassGetMethodFromNameFn>(hRuntime, "mono_class_get_method_from_name", out var classGetMethodFromName)
            || !NativeFunc.GetExport<InstallAssemblySearchHookFn>(hRuntime, "mono_install_assembly_search_hook", out var installAssemblySearchHook)
            || (!NativeFunc.GetExport<FreeFn>(hRuntime, "mono_free", out var free) && !NativeFunc.GetExport<FreeFn>(hRuntime, "g_free", out free))
            || !NativeFunc.GetExport<StringToUtf8Fn>(hRuntime, "mono_string_to_utf8", out var stringToUtf8)
            || !NativeFunc.GetExport<ObjectGetClassFn>(hRuntime, "mono_object_get_class", out var objectGetClass))
            return null;

        var debugEnabled = NativeFunc.GetExport<DebugEnabledFn>(hRuntime, "mono_debug_enabled");
        var domainSetConfig = NativeFunc.GetExport<DomainSetConfigFn>(hRuntime, "mono_domain_set_config");
        var objectToString = NativeFunc.GetExport<ObjectToStringFn>(hRuntime, "mono_object_to_string");

        return new()
        {
            Handle = hRuntime,
            RuntimeInvoke = runtimeInvoke,
            JitInitVersion = jitInitVersion,
            JitParseOptions = jitParseOptions,
            ThreadCurrent = threadCurrent,
            DebugEnabled = debugEnabled,
            DebugInit = debugInit,
            ThreadSetMain = threadSetMain,
            StringNew = stringNew,
            AssemblyGetObject = assemblyGetObject,
            MethodGetName = methodGetName,
            SetAssembliesPath = setAssembliesPath,
            AssemblyGetrootdir = assemblyGetRootDir,
            AddInternalCall = addInternalCall,
            DomainAssemblyOpen = domainAssemblyOpen,
            AssemblyGetImage = assemblyGetImage,
            ClassFromName = classFromName,
            ClassGetMethodFromName = classGetMethodFromName,
            ImageOpenFromDataWithName = imageOpenFromDataWithName,
            InstallAssemblySearchHook = installAssemblySearchHook,
            DomainSetConfig = domainSetConfig,
            ConfigParse = configParse,
            StringToUtf8 = stringToUtf8,
            Free = free,
            ObjectToString = objectToString,
            ObjectGetClass = objectGetClass
        };
    }

    public void SetCurrentThreadAsMain()
    {
        ThreadSetMain(ThreadCurrent());
    }

    public string? GetMethodName(nint method)
    {
        if (method == 0)
            return null;

        return Marshal.PtrToStringAnsi(MethodGetName(method));
    }

    public void InstallAssemblyHooks(AssemblySearchHookFn? searchHook)
    {
        if (searchHook != null)
        {
            passedDelegates.Add(searchHook);
            InstallAssemblySearchHook(searchHook, 0);
        }
    }

    public void LogMonoException(nint exceptionObject)
    {
        if (exceptionObject == 0)
            return;
        string? returnstr = MonoObjectToString(exceptionObject);
        if (returnstr == null)
            return;
        Core.Logger.Error(returnstr);
    }

    public string? MonoStringToString(nint monoString)
    {
        if (monoString == 0)
            return null;

        var cStr = StringToUtf8(monoString);
        if (cStr == 0)
            return null;

        var str = Marshal.PtrToStringUTF8(cStr);
        Free(cStr);
        return str;
    }

    unsafe public string? MonoObjectToString(nint obj)
    {
        nint monoStr = 0;
        nint ex = 0;

        if (ObjectToString != null)
            monoStr = ObjectToString(obj, ref ex);
        else
        {
            nint objClass = ObjectGetClass(obj);
            if (objClass == 0)
                return null;

            nint method = ClassGetMethodFromName(objClass, "ToString", 0);
            if (method == 0)
                return null;

            var initArgs = stackalloc nint*[0];
            monoStr = RuntimeInvoke(method, obj, (void**)initArgs, ref ex);
        }

        if (ex != 0)
        {
            Free(monoStr);
            return null;
        }

        string? ret = MonoStringToString(monoStr);
        Free(monoStr);
        return ret;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint DomainGetFn();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint JitInitVersionFn(nint name, nint b);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void JitParseOptionsFn(nint argc, string[] argv);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugInitFn(MonoDebugFormat format);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool DebugEnabledFn();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint ThreadCurrentFn();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ThreadSetMainFn(nint thread);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint AssemblyGetImageFn(nint assembly);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate nint RuntimeInvokeFn(nint method, nint obj, void** args, ref nint ex);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint AssemblyGetObjectFn(nint domain, nint assembly);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint SetAssembliesPathFn(string domain);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate string AssemblyGetrootdirFn();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint InstallAssemblySearchHookFn(AssemblySearchHookFn func, nint userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint MethodGetNameFn(nint method);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void* StringNewFn(nint domain, nint value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint ObjectToStringFn(nint obj, ref nint ex);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint StringToUtf8Fn(nint str);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint ObjectGetClassFn(nint obj);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public delegate string StringToUtf16Fn(nint str);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void AddInternalCallFn(string name, nint func);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint ClassFromNameFn(nint image, string nameSpace, string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint ClassGetMethodFromNameFn(nint clas, string name, int paramCount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate nint DomainAssemblyOpenFn(nint domain, string path);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void DomainSetConfigFn(nint domain, string configPath, string configFile);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void ConfigParseFn(string? configPath);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FreeFn(nint ptr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public unsafe delegate nint ImageOpenFromDataWithNameFn(byte* data, uint dataLen, bool needCopy, ref MonoImageOpenStatus status, bool refonly, string name);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint AssemblySearchHookFn(ref AssemblyName name, nint userData);

    public enum MonoDebugFormat
    {
        MONO_DEBUG_FORMAT_NONE,
        MONO_DEBUG_FORMAT_MONO,
        MONO_DEBUG_FORMAT_DEBUGGER
    }

    public enum MonoImageOpenStatus
    {
        MONO_IMAGE_OK,
        MONO_IMAGE_ERROR_ERRNO,
        MONO_IMAGE_MISSING_ASSEMBLYREF,
        MONO_IMAGE_IMAGE_INVALID
    }

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
