using MelonLoader.Bootstrap.Utils;
using System.Text;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Mono;

internal static class MonoHandler
{
    private static Dobby.Patch<MonoLib.JitInitVersionFn>? initPatch;
    private static Dobby.Patch<MonoLib.JitParseOptionsFn>? jitParseOptionsPatch;
    private static Dobby.Patch<MonoLib.DebugInitFn>? debugInitPatch;
    private static Dobby.Patch<MonoLib.RuntimeInvokeFn>? invokePatch;
    private static Dobby.Patch<MonoLib.ImageOpenFromDataWithNameFn>? imageOpenFromDataWithNamePatch;

    private static nint assemblyManagerResolve;
    private static nint assemblyManagerLoadInfo;
    private static nint coreStart;
    private static bool debugInitCalled;

    internal static nint Domain { get; private set; }
    internal static MonoLib Mono { get; private set; } = null!;

    private const char MonoPathSeparator =
#if WINDOWS
        ';';
#elif LINUX
        ':';
#endif
    
    private const string MonoDebugArgsStart = "--debugger-agent=transport=dt_socket,server=y,address=";
    private const string MonoDebugNoSuspendArg = ",suspend=n";
    private const string MonoDebugNoSuspendArgOldMono = ",suspend=n,defer=y";
    
    public static unsafe bool TryInitialize()
    {
        var monoLib = MonoLib.TryLoad(Core.GameDir);
        if (monoLib == null)
            monoLib = MonoLib.TryLoad(Core.DataDir);
        if (monoLib == null)
            return false;

        Mono = monoLib;

        MelonDebug.Log("Patching mono init");
        initPatch = Dobby.CreatePatch<MonoLib.JitInitVersionFn>(Mono.JitInitVersionPtr, InitDetour);
        MelonDebug.Log("Patching mono jit parse options");
        jitParseOptionsPatch = Dobby.CreatePatch<MonoLib.JitParseOptionsFn>(Mono.JitParseOptionsPtr, JitParseOptionsDetour);
        MelonDebug.Log("Patching mono debug init");
        debugInitPatch = Dobby.CreatePatch<MonoLib.DebugInitFn>(Mono.DebugInitPtr, DebugInitDetour);
        MelonDebug.Log("Patching mono image open from data with name patch");
        imageOpenFromDataWithNamePatch = Dobby.CreatePatch<MonoLib.ImageOpenFromDataWithNameFn>(Mono.ImageOpenFromDataWithNamePtr, ImageOpenFromDataWithNameDetour);

        return true;
    }

    private static unsafe IntPtr ImageOpenFromDataWithNameDetour(byte* data, uint dataLen, bool needCopy, ref MonoLib.MonoImageOpenStatus status, bool refonly, string name)
    {
        if (imageOpenFromDataWithNamePatch == null)
            return IntPtr.Zero;

        if (string.IsNullOrEmpty(name))
            return imageOpenFromDataWithNamePatch.Original(data, dataLen, needCopy, ref status, refonly, name);

        if (string.IsNullOrWhiteSpace(LoaderConfig.Current.UnityEngine.MonoSearchPathOverride))
            return imageOpenFromDataWithNamePatch.Original(data, dataLen, needCopy, ref status, refonly, name);

        string fileName = Path.GetFileName(name);
        var foundOverridenFile = LoaderConfig.Current.UnityEngine.MonoSearchPathOverride
            .Split(MonoPathSeparator)
            .Select(x => Path.GetFullPath(Path.Combine(x, fileName)))
            .FirstOrDefault(File.Exists);
        
        if (foundOverridenFile == null)
            return imageOpenFromDataWithNamePatch.Original(data, dataLen, needCopy, ref status, refonly, name);
        
        MelonDebug.Log($"Overriding the image load of {name} to {foundOverridenFile}");
        byte[] newDataArray = File.ReadAllBytes(foundOverridenFile);
        uint newDataLen = (uint)newDataArray.Length;
        fixed (byte* newDataPtr = &newDataArray[0])
        {
            var newReturn = imageOpenFromDataWithNamePatch.Original(newDataPtr, newDataLen, needCopy, ref status, refonly, name);
            return newReturn;
        }
    }

    private static void DebugInitDetour(MonoLib.MonoDebugFormat format)
    {
        if (debugInitPatch == null)
            return;

        debugInitPatch.Destroy();

        debugInitCalled = true;

        debugInitPatch.Original(format);
    }

    private static nint InitDetour(nint name, nint b)
    {
        if (initPatch == null)
            return 0;

        initPatch.Destroy();

        ConsoleHandler.ResetHandles();
        MelonDebug.Log("In init detour");

        StringBuilder newAssembliesPathSb = new();
        if (!string.IsNullOrWhiteSpace(LoaderConfig.Current.UnityEngine.MonoSearchPathOverride))
        {
            var absolutePaths = LoaderConfig.Current.UnityEngine.MonoSearchPathOverride
                .Split(MonoPathSeparator)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Path.GetFullPath(x, LoaderConfig.Current.Loader.BaseDirectory));
            newAssembliesPathSb.Append(string.Join(MonoPathSeparator, absolutePaths));
            newAssembliesPathSb.Append(MonoPathSeparator);
        }

        if (Mono.IsOld)
        {
            string baseOverridesDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Dependencies", "NetStandardPatches");
            newAssembliesPathSb.Append(baseOverridesDir);
            newAssembliesPathSb.Append(MonoPathSeparator);
        }
        newAssembliesPathSb.Append(Mono.AssemblyGetrootdir());

        string newAssembliesPath = newAssembliesPathSb.ToString();
        MelonDebug.Log($"Setting Mono assemblies path to: {newAssembliesPath}");
        Mono.SetAssembliesPath(newAssembliesPath);

        JitParseOptionsDetour(0, []);
        bool debuggerAlreadyEnabled = debugInitCalled || (Mono.DebugEnabled != null && Mono.DebugEnabled());

        if (LoaderConfig.Current.Loader.DebugMode && !debuggerAlreadyEnabled)
        {
            MelonDebug.Log("Initialising mono debugger");
            Mono.DebugInit(MonoLib.MonoDebugFormat.MONO_DEBUG_FORMAT_MONO);
        }

        MelonDebug.Log("Original init jit version");
        Domain = initPatch.Original(name, b);

        MelonDebug.Log("Setting Mono Main Thread");
        Mono.SetCurrentThreadAsMain();

        if (Mono is { IsOld: false, DomainSetConfig: not null })
        {
            string configFile = $"{Environment.ProcessPath}.config";
            MelonDebug.Log($"Setting Mono Config paths: base_dir: {Core.GameDir}, config_file_name: {configFile}");

            Mono.DomainSetConfig(Domain, Core.GameDir, configFile);
        }
        
        MelonDebug.Log("Parsing default Mono config");
        Mono.ConfigParse(null);

        InitializeManaged();
        jitParseOptionsPatch?.Destroy();

        return Domain;
    }

    private static void JitParseOptionsDetour(IntPtr argc, string[] argv)
    {
        if (jitParseOptionsPatch == null)
            return;

        if (!LoaderConfig.Current.Loader.DebugMode)
        {
            jitParseOptionsPatch.Original(argc, argv);
            return;
        }
        
        string newArgs;
        string? dnSpyEnv = Environment.GetEnvironmentVariable("DNSPY_UNITY_DBG2");
        if (dnSpyEnv == null)
        {
            StringBuilder newArgsSb = new(MonoDebugArgsStart);
            newArgsSb.Append(LoaderConfig.Current.MonoDebugServer.DebugIpAddress);
            newArgsSb.Append(':');
            newArgsSb.Append(LoaderConfig.Current.MonoDebugServer.DebugPort);
            if (!LoaderConfig.Current.MonoDebugServer.DebugSuspend)
                newArgsSb.Append(Mono.IsOld ? MonoDebugNoSuspendArgOldMono : MonoDebugNoSuspendArg);
            newArgs = newArgsSb.ToString();
        }
        else
        {
            newArgs = dnSpyEnv;
        }
        
        string[] newArgv = new string[argc + 1];
        Array.Copy(argv, 0, newArgv, 0, argc);
        argc++;
        newArgv[argc - 1] = newArgs;
        
        MelonDebug.Log($"Adding jit option: {string.Join(' ', newArgs)}");

        jitParseOptionsPatch.Original(argc, newArgv);
    }

    private static unsafe void InitializeManaged()
    {
        MelonDebug.Log("Initializing managed assemblies");

        var mlPath = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "net35", "MelonLoader.dll");
        if (!File.Exists(mlPath))
        {
            Core.Logger.Error($"Mono MelonLoader assembly not found at: '{mlPath}'");
            return;
        }

        MelonDebug.Log("Loading ML assembly");
        var assembly = Mono.DomainAssemblyOpen(Domain, mlPath);
        if (assembly == 0)
        {
            Core.Logger.Error($"Failed to load the Mono MelonLoader assembly");
            return;
        }

        MelonDebug.Log("Adding internal calls");
        Mono.AddManagedInternalCall<CastManagedAssemblyPtrFn>("MelonLoader.Utils.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtrImpl);

        var image = Mono.AssemblyGetImage(assembly);
        var interopClass = Mono.ClassFromName(image, "MelonLoader.InternalUtils", "BootstrapInterop");

        var initMethod = Mono.ClassGetMethodFromName(interopClass, "Initialize", 1);
        coreStart = Mono.ClassGetMethodFromName(interopClass, "Start", 0);

        var assemblyManagerClass = Mono.ClassFromName(image, "MelonLoader.Resolver", "AssemblyManager");

        assemblyManagerResolve = Mono.ClassGetMethodFromName(assemblyManagerClass, "Resolve", 6);
        assemblyManagerLoadInfo = Mono.ClassGetMethodFromName(assemblyManagerClass, "LoadInfo", 1);

        nint ex = 0;
        MelonDebug.Log("Invoking managed core init");

        var bootstrapHandle = Core.LibraryHandle;
        var initArgs = stackalloc nint*[]
        {
            &bootstrapHandle
        };
        Mono.RuntimeInvoke(initMethod, 0, (void**)initArgs, ref ex);
        if (ex != 0)
            return;

        MelonDebug.Log("Patching invoke");
        invokePatch = Dobby.CreatePatch<MonoLib.RuntimeInvokeFn>(Mono.RuntimeInvokePtr, InvokeDetour);
    }

    private static unsafe nint InvokeDetour(nint method, nint obj, void** args, ref nint ex)
    {
        if (invokePatch == null)
            return 0;

        var result = invokePatch.Original(method, obj, args, ref ex);

        var name = Mono.GetMethodName(method);
        if (name == null ||
            ((!Mono.IsOld || (!name.Contains("Awake") && !name.Contains("DoSendMouseEvents")))
             && !name.Contains("Internal_ActiveSceneChanged")
             && !name.Contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize")))
            return result;

        MelonDebug.Log("Invoke hijacked");
        invokePatch.Destroy();

        Start();

        return result;
    }

    private static unsafe void Start()
    {
        nint ex = 0;
        Mono.RuntimeInvoke(coreStart, 0, null, ref ex);
    }

    private static nint CastManagedAssemblyPtrImpl(nint ptr)
    {
        return ptr;
    }

    internal static void InstallHooks()
    {
        MelonDebug.Log("Installing hooks");

        Mono.InstallAssemblyHooks(OnAssemblyPreload, OnAssemblySearch, OnAssemblyLoad);
    }

    private static unsafe void OnAssemblyLoad(nint monoAssembly, nint userData)
    {
        if (monoAssembly == 0)
            return;

        var obj = Mono.AssemblyGetObject(Domain, monoAssembly);

        nint ex = 0;
        Mono.RuntimeInvoke(assemblyManagerLoadInfo, 0, (void**)&obj, ref ex);
    }

    private static nint OnAssemblySearch(ref MonoLib.AssemblyName name, nint userData)
    {
        return ResolveAssembly(name, false);
    }

    private static nint OnAssemblyPreload(ref MonoLib.AssemblyName name, nint assemblyPaths, nint userData)
    {
        return ResolveAssembly(name, true);
    }

    private static unsafe nint ResolveAssembly(MonoLib.AssemblyName name, bool preload)
    {
        var args = stackalloc void*[]
        {
            Mono.StringNew(Domain, name.Name),
            &name.Major,
            &name.Minor,
            &name.Build,
            &name.Revision,
            &preload
        };

        nint ex = 0;
        var reflectionAsm = (MonoLib.ReflectionAssembly*)Mono.RuntimeInvoke(assemblyManagerResolve, 0, args, ref ex);
        return reflectionAsm == null ? 0 : reflectionAsm->Assembly;
    }

    private delegate nint CastManagedAssemblyPtrFn(nint ptr);
}
