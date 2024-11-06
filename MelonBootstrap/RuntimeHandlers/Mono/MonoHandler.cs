using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Mono;

internal static class MonoHandler
{
    private static Dobby.Patch<MonoLib.JitInitVersionFn>? initPatch;
    private static Dobby.Patch<MonoLib.RuntimeInvokeFn>? invokePatch;

    private static MonoLib mono = null!;

    private static nint domain;

    private static nint assemblyManagerResolve;
    private static nint assemblyManagerLoadInfo;
    private static nint coreStart;

    public static bool TryInitialize()
    {
        var monoLib = MonoLib.TryLoad(
#if WINDOWS
            Core.GameDir
#else
            Core.DataDir
#endif
            );
        if (monoLib == null)
            return false;

        mono = monoLib;

        MelonDebug.Log("Patching mono init");
        initPatch = Dobby.CreatePatch<MonoLib.JitInitVersionFn>(mono.JitInitVersionPtr, InitDetour);

        return true;
    }

    private static nint InitDetour(nint name, nint b)
    {
        if (initPatch == null)
            return 0;

        initPatch.Destroy();

        ConsoleHandler.ResetHandles();
        MelonDebug.Log("In init detour");

        domain = initPatch.Original(name, b);

        if (Core.Debug && mono.DebugDomainCreate != null)
        {
            MelonDebug.Log("Creating Mono Debug Domain");
            mono.DebugDomainCreate(domain);
        }
        
        MelonDebug.Log("Setting Mono Main Thread");
        mono.SetCurrentThreadAsMain();
        
        if (mono is { IsOld: false, DomainSetConfig: not null })
        {
            MelonDebug.Log("Setting Mono Config");
        
            mono.DomainSetConfig(domain, Core.GameDir, name);
        }

        InitializeManaged();

        return domain;
    }

    private static unsafe void InitializeManaged()
    {
        MelonDebug.Log("Initializing managed assemblies");

        var mlPath = Path.Combine(Core.BaseDir, "MelonLoader", "net35", "MelonLoader.dll");
        if (!File.Exists(mlPath))
        {
            Core.Logger.Error($"Mono MelonLoader assembly not found at: '{mlPath}'");
            return;
        }

        var corlibPath = Path.Combine(Core.DataDir, "Managed", "mscorlib.dll");
        if (File.Exists(corlibPath))
        {
            var corlibVersion = FileVersionInfo.GetVersionInfo(corlibPath);
            if (corlibVersion.FileMajorPart <= 2)
            {
                Core.Logger.Msg("Loading .NET Standard 2.0 overrides");

                var overridesDir = Path.Combine(Core.BaseDir, "MelonLoader", "Dependencies", "NetStandardOverrides");
                if (Directory.Exists(overridesDir))
                {
                    foreach (var dll in Directory.EnumerateFiles(overridesDir, "*.dll"))
                    {
                        MelonDebug.Log("Loading assembly: " + dll);
                        if (mono.DomainAssemblyOpen(domain, dll) == 0)
                            MelonDebug.Log("Assembly failed to load!");
                    }
                }
            }
        }

        MelonDebug.Log("Loading ML assembly");
        var assembly = mono.DomainAssemblyOpen(domain, mlPath);
        if (assembly == 0)
        {
            Core.Logger.Error($"Failed to load the Mono MelonLoader assembly");
            return;
        }
        
        MelonDebug.Log("Adding internal calls");
        mono.AddManagedInternalCall("MelonLoader.Resolver.AssemblyManager::InstallHooks", InstallHooks);
        mono.AddManagedInternalCall<PtrRet>("MelonLoader.Utils.MonoLibrary::GetRootDomainPtr", GetRootDomainPtrImpl);
        mono.AddManagedInternalCall<PtrRet>("MelonLoader.Utils.MonoLibrary::GetLibPtr", GetLibPtrImpl);
        mono.AddManagedInternalCall<CastManagedAssemblyPtr>("MelonLoader.Utils.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtrImpl);
        mono.AddManagedInternalCall<NativeHookFn>("MelonLoader.BootstrapInterop::NativeHookAttach", NativeHookAttachImpl);
        mono.AddManagedInternalCall<NativeHookFn>("MelonLoader.BootstrapInterop::NativeHookDetach", NativeHookDetachImpl);
        mono.AddManagedInternalCall<LogMsgFn>("MelonLoader.MelonLogger::HostLogMsg", MelonLogger.LogFromManaged);
        mono.AddManagedInternalCall<LogErrorFn>("MelonLoader.MelonLogger::HostLogError", MelonLogger.LogErrorFromManaged);
        mono.AddManagedInternalCall<LogMelonInfoFn>("MelonLoader.MelonLogger::HostLogMelonInfo", MelonLogger.LogMelonInfoFromManaged);
        
        var image = mono.AssemblyGetImage(assembly);
        var coreClass = mono.ClassFromName(image, "MelonLoader", "Core");
        
        var initMethod = mono.ClassGetMethodFromName(coreClass, "Initialize", 0);
        coreStart = mono.ClassGetMethodFromName(coreClass, "Start", 0);
        
        var assemblyManagerClass = mono.ClassFromName(image, "MelonLoader.Resolver", "AssemblyManager");
        
        assemblyManagerResolve = mono.ClassGetMethodFromName(assemblyManagerClass, "Resolve", 6);
        assemblyManagerLoadInfo = mono.ClassGetMethodFromName(assemblyManagerClass, "LoadInfo", 1);
        
        nint ex = 0;
        MelonDebug.Log("Invoking managed core init");
        mono.RuntimeInvoke(initMethod, 0, null, ref ex);
        if (ex != 0)
        {
            Core.Logger.Error($"The init method threw an exception:");
            Core.Logger.Error(mono.ToString(ex)!);
            return;
        }

        MelonDebug.Log("Patching invoke");
        invokePatch = Dobby.CreatePatch<MonoLib.RuntimeInvokeFn>(mono.RuntimeInvokePtr, InvokeDetour);
    }

    private static unsafe nint InvokeDetour(nint method, nint obj, void** args, ref nint ex)
    {
        if (invokePatch == null)
            return 0;

        var result = invokePatch.Original(method, obj, args, ref ex);

        var name = mono.GetMethodName(method);
        if (name == null ||
            ((!mono.IsOld || (!name.Contains("Awake") && !name.Contains("DoSendMouseEvents"))) 
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
        mono.RuntimeInvoke(coreStart, 0, null, ref ex);
        if (ex != 0)
        {
            Core.Logger.Error($"The start method threw an exception:");
            Core.Logger.Error(mono.ToString(ex)!);
        }
    }

    private static unsafe void NativeHookAttachImpl(nint* target, nint detour)
    {
        *target = Dobby.HookAttach(*target, detour);
    }

    private static unsafe void NativeHookDetachImpl(nint* target, nint detour)
    {
        Dobby.HookDetach(*target);
    }

    private static nint GetRootDomainPtrImpl()
    {
        return domain;
    }

    private static nint CastManagedAssemblyPtrImpl(nint ptr)
    {
        return ptr;
    }

    private static nint GetLibPtrImpl()
    {
        return mono.Handle;
    }

    private static void InstallHooks()
    {
        MelonDebug.Log("Installing hooks");

        mono.InstallAssemblyHooks(OnAssemblyPreload, OnAssemblySearch, OnAssemblyLoad);
    }

    private static unsafe void OnAssemblyLoad(nint monoAssembly, nint userData)
    {
        if (monoAssembly == 0)
            return;

        var obj = mono.AssemblyGetObject(domain, monoAssembly);

        nint ex = 0;
        mono.RuntimeInvoke(assemblyManagerLoadInfo, 0, (void**)&obj, ref ex);
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
            mono.StringNew(domain, name.Name),
            &name.Major,
            &name.Minor,
            &name.Build,
            &name.Revision,
            &preload
        };

        nint ex = 0;
        var reflectionAsm = (MonoLib.ReflectionAssembly*)mono.RuntimeInvoke(assemblyManagerResolve, 0, args, ref ex);
        return reflectionAsm == null ? 0 : reflectionAsm->Assembly;
    }

    private delegate nint PtrRet();
    private delegate nint CastManagedAssemblyPtr(nint ptr);
}
