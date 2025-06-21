using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Mono;

internal static class MonoHandler
{
    private static nint assemblyManagerSearchAssembly;
    private static bool debugInitCalled;
    private static bool jitInitDone;

    internal static nint Domain { get; private set; }
    internal static MonoLib Mono { get; private set; } = null!;

    private const char MonoPathSeparator =
#if WINDOWS
        ';';
#elif LINUX || OSX || ANDROID
        ':';
#endif

    private const string MonoDebugArgsStart = "--debugger-agent=transport=dt_socket,server=y,address=";
    private const string MonoDebugNoSuspendArg = ",suspend=n";
    private const string MonoDebugNoSuspendArgOldMono = ",suspend=n,defer=y";

    private static readonly MonoLib.JitInitVersionFn MonoInitDetourFn = InitDetour;
    private static readonly MonoLib.JitParseOptionsFn JitParseOptionsDetourFn = JitParseOptionsDetour;
    private static readonly MonoLib.DebugInitFn DebugInitDetourFn = DebugInitDetour;
    private static readonly unsafe MonoLib.ImageOpenFromDataWithNameFn ImageOpenFromDataWithNameFn = ImageOpenFromDataWithNameDetour;
    internal static readonly Dictionary<string, (Action<nint> InitMethod, IntPtr detourPtr)> SymbolRedirects = new()
    {
        { "mono_jit_init_version", (Initialize, Marshal.GetFunctionPointerForDelegate(MonoInitDetourFn))},
        { "mono_jit_parse_options", (Initialize, Marshal.GetFunctionPointerForDelegate(JitParseOptionsDetourFn))},
        { "mono_debug_init", (Initialize, Marshal.GetFunctionPointerForDelegate(DebugInitDetourFn))},
        { "mono_image_open_from_data_with_name", (Initialize, Marshal.GetFunctionPointerForDelegate(ImageOpenFromDataWithNameFn))}
    };

    public static void Initialize(nint handle)
    {
        var mono = MonoLib.TryLoad(handle);
        if (mono is null)
        {
            Core.Logger.Error("Could not load mono");
            return;
        }
        Mono = mono;
    }

    internal static unsafe IntPtr ImageOpenFromDataWithNameDetour(byte* data, uint dataLen, bool needCopy, ref MonoLib.MonoImageOpenStatus status, bool refonly, string name)
    {
        if (string.IsNullOrEmpty(name))
            return Mono.ImageOpenFromDataWithName(data, dataLen, needCopy, ref status, refonly, name);

        if (string.IsNullOrWhiteSpace(LoaderConfig.Current.UnityEngine.MonoSearchPathOverride))
            return Mono.ImageOpenFromDataWithName(data, dataLen, needCopy, ref status, refonly, name);

        string fileName = Path.GetFileName(name);
        var foundOverridenFile = LoaderConfig.Current.UnityEngine.MonoSearchPathOverride
            .Split(MonoPathSeparator)
            .Select(x => Path.GetFullPath(Path.Combine(x, fileName)))
            .FirstOrDefault(File.Exists);

        if (foundOverridenFile == null)
            return Mono.ImageOpenFromDataWithName(data, dataLen, needCopy, ref status, refonly, name);
        
        MelonDebug.Log($"Overriding the image load of {name} to {foundOverridenFile}");
        byte[] newDataArray = File.ReadAllBytes(foundOverridenFile);
        uint newDataLen = (uint)newDataArray.Length;
        fixed (byte* newDataPtr = &newDataArray[0])
        {
            var newReturn = Mono.ImageOpenFromDataWithName(newDataPtr, newDataLen, needCopy, ref status, refonly, foundOverridenFile);
            return newReturn;
        }
    }

    internal static void DebugInitDetour(MonoLib.MonoDebugFormat format)
    {
        debugInitCalled = true;
        Mono.DebugInit(format);
    }

    internal static nint InitDetour(nint domainName, nint runtimeVersion)
    {
        if (jitInitDone)
            return Mono.JitInitVersion(domainName, runtimeVersion);

        ConsoleHandler.ResetHandles();
        MelonDebug.Log("In init detour");
        string domainNameStr = Marshal.PtrToStringAnsi(domainName)!;
        string runtimeVersionStr = Marshal.PtrToStringAnsi(runtimeVersion)!;
        MelonDebug.Log($"Domain: {domainNameStr}, Runtime version: {runtimeVersionStr}");
        if (runtimeVersionStr.Length > 2 && int.TryParse(runtimeVersionStr.AsSpan(1, 1), out var runtimeVersionInt))
            Mono.IsOld = runtimeVersionInt <= 3;

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
        Domain = Mono.JitInitVersion(domainName, runtimeVersion);

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
        jitInitDone = true;

        return Domain;
    }

    internal static void JitParseOptionsDetour(IntPtr argc, string[] argv)
    {
        if (jitInitDone || !LoaderConfig.Current.Loader.DebugMode)
        {
            Mono.JitParseOptions(argc, argv);
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

        Mono.JitParseOptions(argc, newArgv);
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

        var image = Mono.AssemblyGetImage(assembly);
        var interopClass = Mono.ClassFromName(image, "MelonLoader.InternalUtils", "BootstrapInterop");

        var initMethod = Mono.ClassGetMethodFromName(interopClass, "Initialize", 1);
        Mono.ClassGetMethodFromName(interopClass, "Start", 0);

        var assemblyManagerClass = Mono.ClassFromName(image, "MelonLoader.Resolver", "AssemblyManager");

        assemblyManagerSearchAssembly = Mono.ClassGetMethodFromName(assemblyManagerClass, "SearchAssembly", 5);

        nint ex = 0;
        MelonDebug.Log("Invoking managed core init");

        var bootstrapHandle = Core.LibraryHandle;
        var initArgs = stackalloc nint*[]
        {
            &bootstrapHandle
        };
        Mono.RuntimeInvoke(initMethod, 0, (void**)initArgs, ref ex);
    }

    internal static void InstallHooks()
    {
        MelonDebug.Log("Installing hooks");

        Mono.InstallAssemblyHooks(OnAssemblySearch);
    }

    private static nint OnAssemblySearch(ref MonoLib.AssemblyName name, nint userData)
    {
        return SearchAssembly(name);
    }

    private static unsafe nint SearchAssembly(MonoLib.AssemblyName name)
    {
        var args = stackalloc void*[]
        {
            Mono.StringNew(Domain, name.Name),
            &name.Major,
            &name.Minor,
            &name.Build,
            &name.Revision
        };

        nint ex = 0;
        var reflectionAsm = (MonoLib.ReflectionAssembly*)Mono.RuntimeInvoke(assemblyManagerSearchAssembly, 0, args, ref ex);
        return reflectionAsm == null ? 0 : reflectionAsm->Assembly;
    }
}
