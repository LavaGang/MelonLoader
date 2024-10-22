using MelonBootstrap;
using MelonBootstrap.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonBootstrap.RuntimeHandlers.Mono;

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
        var monoLib = MonoLib.TryLoad(Core.GameDir);
        if (monoLib == null)
            return false;

        mono = monoLib;

        initPatch = Dobby.CreatePatch<MonoLib.JitInitVersionFn>(mono.JitInitVersionPtr, InitDetour);
        if (initPatch == null)
        {
            // TODO: Error
        }

        return true;
    }

    private static nint InitDetour(nint name, nint b)
    {
        if (initPatch == null)
            return 0;

        initPatch.Destroy();

        ConsoleUtils.ResetHandles();

        domain = initPatch.Original(name, b);

        if (Core.Debug && mono.DebugDomainCreate != null)
        {
            Console.WriteLine("Creating Mono Debug Domain");
            mono.DebugDomainCreate(domain);
        }

        Console.WriteLine("Setting Mono Main Thread");
        mono.SetCurrentThreadAsMain();

        if (!mono.IsOld && mono.DomainSetConfig != null)
        {
            Console.WriteLine("Setting Mono Config");

            mono.DomainSetConfig(domain, Core.GameDir, name);
        }

        InitializeManaged();

        return domain;
    }

    private static unsafe void InitializeManaged()
    {
        var mlPath = Path.Combine(Core.GameDir, "MelonLoader", "net35", "MelonLoader.dll");
        if (!File.Exists(mlPath))
            // TODO: Error
            return;

        var corlibPath = Path.Combine(Core.DataDir, "Managed", "mscorlib.dll");
        if (!File.Exists(corlibPath))
        {
            Console.WriteLine("No mscorlib?");
        }

        var corlibVersion = FileVersionInfo.GetVersionInfo(corlibPath);
        if (corlibVersion.FileMajorPart <= 2)
        {
            Console.WriteLine("Loading .NET Standard 2.0 overrides");

            var overridesDir = Path.Combine(Core.GameDir, "MelonLoader", "Dependencies", "NetStandardOverrides");
            if (Directory.Exists(overridesDir))
            {
                foreach (var dll in Directory.EnumerateFiles(overridesDir, "*.dll"))
                {
                    mono.DomainAssemblyOpen(domain, dll);
                }
            }
        }

        var assembly = mono.DomainAssemblyOpen(domain, mlPath);
        if (assembly == 0)
            // TODO: Error
            return;

        mono.AddManagedInternalCall("MelonLoader.Resolver.AssemblyManager::InstallHooks", InstallHooks);
        mono.AddManagedInternalCall<PtrRet>("MelonLoader.Utils.MonoLibrary::GetRootDomainPtr", GetRootDomainPtrImpl);
        mono.AddManagedInternalCall<PtrRet>("MelonLoader.Utils.MonoLibrary::GetLibPtr", GetLibPtrImpl);
        mono.AddManagedInternalCall<CastManagedAssemblyPtr>("MelonLoader.Utils.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtrImpl);
        mono.AddManagedInternalCall<NativeHook>("MelonLoader.BootstrapInterop::NativeHookAttach", NativeHookAttachImpl);
        mono.AddManagedInternalCall<NativeHook>("MelonLoader.BootstrapInterop::NativeHookDetach", NativeHookDetachImpl);

        var image = mono.AssemblyGetImage(assembly);
        var coreClass = mono.ClassFromName(image, "MelonLoader", "Core");

        var initMethod = mono.ClassGetMethodFromName(coreClass, "Initialize", 0);
        coreStart = mono.ClassGetMethodFromName(coreClass, "Start", 0);

        var assemblyManagerClass = mono.ClassFromName(image, "MelonLoader.Resolver", "AssemblyManager");

        assemblyManagerResolve = mono.ClassGetMethodFromName(assemblyManagerClass, "Resolve", 6);
        assemblyManagerLoadInfo = mono.ClassGetMethodFromName(assemblyManagerClass, "LoadInfo", 1);

        nint ex = 0;
        mono.RuntimeInvoke(initMethod, 0, null, ref ex);

        invokePatch = Dobby.CreatePatch<MonoLib.RuntimeInvokeFn>(mono.RuntimeInvokePtr, InvokeDetour);
    }

    private static unsafe nint InvokeDetour(nint method, nint obj, void** args, ref nint ex)
    {
        if (invokePatch == null)
            return 0;

        var result = invokePatch.Original(method, obj, args, ref ex);

        var name = mono.GetMethodName(method);
        if (name != null && 
            ((mono.IsOld && (name.Contains("Awake") || name.Contains("DoSendMouseEvents")))
            || name.Contains("Internal_ActiveSceneChanged")
            || name.Contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize")))
        {
            invokePatch.Destroy();

            Start();
        }

        return result;
    }

    private static unsafe void Start()
    {
        nint ex = 0;
        mono.RuntimeInvoke(coreStart, 0, null, ref ex);
    }

    private static void NativeHookAttachImpl(ref nint target, nint detour)
    {
        target = Dobby.HookAttach(target, detour);
    }

    private static void NativeHookDetachImpl(ref nint target, nint detour)
    {
        Dobby.HookDetach(target);
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
        Console.WriteLine("Installing hooks");

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

    private static unsafe nint OnAssemblyPreload(ref MonoLib.AssemblyName name, nint assemblyPaths, nint userData)
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
        if (reflectionAsm == null)
            return 0;

        return reflectionAsm->Assembly;
    }

    private delegate nint PtrRet();
    private delegate nint CastManagedAssemblyPtr(nint ptr);
    private delegate void NativeHook(ref nint target, nint detour);
}
