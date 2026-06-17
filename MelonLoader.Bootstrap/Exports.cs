using System.Diagnostics;
using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Logging;

namespace MelonLoader.Bootstrap;

internal static class Exports
{
    private static readonly string CurrentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
    
    public static nint LibraryHandle { get; private set; }
    public static string DataDir { get; private set; } = null!;
    
    public static string ProcessPath { get; private set; } = null!;
    public static string ProcessFileName { get; private set; } = null!;
    public static string ProcessFileNameWithoutExt { get; private set; } = null!;
    public static string ProcessDirectory { get; private set; } = null!;
    
#if LINUX || OSX
    private static bool _hookPlayerMainEntered;
    
    private const string LdPreloadEnvName =
#if OSX
        "DYLD_INSERT_LIBRARIES";
#else
        "LD_PRELOAD";
#endif
    
    private const string LdPathEnvName =
#if OSX
        "DYLD_LIBRARY_PATH";
#else
        "LD_LIBRARY_PATH";
#endif
#endif
    
    public const string LibExtension =
#if OSX
        "dylib";
#elif WINDOWS
        "dll";
#else
        "so";
#endif

    // It's not guaranteed that the first call is the one we want: it's possible someone was wrapping
    // the execution from another. We need to at least have good confidence that we're dealing with a
    // Unity game. The following heuristics checks the presence of a gameName_Data or Data folder which
    // is at least required for the game to boot. It's not perfect, but it makes it much more likely to not
    // mess up.
    private static bool Initialize(nint moduleHandle)
    {
        LibraryHandle = moduleHandle;
        
        ProcessPath = Environment.ProcessPath!;
        ProcessFileName = Path.GetFileName(ProcessPath);
        ProcessFileNameWithoutExt = Path.GetFileNameWithoutExtension(ProcessPath);
        ProcessDirectory = Path.GetDirectoryName(ProcessPath)!;

#if OSX
        DataDir = Path.Combine(Path.GetDirectoryName(ProcessDirectory)!, "Resources", "Data");
#else
        DataDir = Path.Combine(ProcessDirectory, "Data");
        if (!Directory.Exists(DataDir))
            DataDir = Path.Combine(ProcessDirectory, ProcessFileNameWithoutExt + "_Data");
#endif
        return Directory.Exists(DataDir);
    }

#if LINUX || OSX
    // We have confirmed Unity is about to get its main invoked, but we need to protect us against
    // double entry from other processes the game might launch. The most reliable way is to simply cut
    // original link meaning remove ourselves from LD_PRELOAD / LD_LIBRARY_PATH. It's much safer anyway
    // since hooking on libc's "pre main" is inherently dangerous so the less time we have this, the better
    private static void RemoveLibraryPreloadEnv()
    {
        string[] ldPreloads = Environment.GetEnvironmentVariable(LdPreloadEnvName)!.Split(":");
        string newLdPreload = string.Join(':', ldPreloads.Where(x => x != $"{CurrentAssemblyName}.{LibExtension}"));
        string[]? ldLibraryPaths = Environment.GetEnvironmentVariable(LdPathEnvName)?.Split(":");

        // It's possible to hook without a library path set so we only remove ourselves if the variable had a value
        if (ldLibraryPaths is not null && ldLibraryPaths.Length > 0)
        {
            string expectedLibFullPath =
#if OSX
                Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(ProcessDirectory)))!;
#else
                ProcessDirectory;
#endif

            string newLibraryPath = string.Join(':', ldLibraryPaths
                .Where(x => x.Length != 0 && expectedLibFullPath != Path.TrimEndingDirectorySeparator(Path.GetFullPath(x))));
            LibcNative.Setenv(LdPathEnvName, newLibraryPath, true);
            Environment.SetEnvironmentVariable(LdPathEnvName, newLibraryPath);
        }
        LibcNative.Setenv(LdPreloadEnvName, newLdPreload, true);
        Environment.SetEnvironmentVariable(LdPreloadEnvName, newLdPreload);
    }
#endif

#if WINDOWS
    // https://github.com/Xpl0itR/NativeDllMain
    [UnmanagedCallersOnly(EntryPoint = "DllProcessAttach")]
    [RequiresDynamicCode("Calls InitConfig")]
    public static bool WindowsEntryPoint(nint hModule)
    {
        if (!Initialize(hModule))
            return true;

        Proxy.ProxyResolver.Init(hModule);
        Core.Init();

        return true;
    }
#elif OSX
    [UnmanagedCallersOnly(EntryPoint = "Init", CallConvs = [typeof(CallConvCdecl)])]
    [RequiresDynamicCode("Calls InitConfig")]
    public static void Init()
    {
        // Double entry protection against the same process
        if (_hookPlayerMainEntered)
            return;

        string libraryPath = $"{CurrentAssemblyName}.dylib";
        nint handle = NativeLibrary.Load(libraryPath);
        if (!Initialize(handle))
            return;

        _hookPlayerMainEntered = true;

        RemoveLibraryPreloadEnv();
        Core.Init();
    }
#elif LINUX
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate int MainFn(int argc, char** argv, char** envp);

    private static MainFn? _originalMain;

    // The Linux entrypoint involves exposing our version of __libc_start_main as it's the only
    // symbol we have a guarantee will be called no matter the Unity version. Helpfully for us,
    // the function receives the main function as argument so we can capture it and pass our own
    // main to the real __libc_start_main. This will make libc call us as the main which allows us
    // to hook at the moment PlayerMain would be called, but still allow Unity to proceed with its
    // original PlayerMain. This works because part of LD_PRELOAD feature is exposing the same symbol
    // as another library will make it be used instead of the original.
    [UnmanagedCallersOnly(EntryPoint = "__libc_start_main", CallConvs = [typeof(CallConvCdecl)])]
    [RequiresDynamicCode("Uses a delegate that calls InitConfig")]
    public static unsafe int LibCStartMain(
        delegate* unmanaged[Cdecl]<int, char**, char**, int> main,
        int argc,
        char** argv,
        nint init,
        nint fini,
        nint rtLdFini,
        nint stackEnd)
    {
        // Double entry protection against the same process
        if (_hookPlayerMainEntered)
            return LibcNative.LibCStartMain(main, argc, argv, init, fini, rtLdFini, stackEnd);
        
        string libraryPath = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), $"{CurrentAssemblyName}.so");
        if (!File.Exists(libraryPath) || !NativeLibrary.TryLoad(libraryPath, out nint handle) || !Initialize(handle))
            return LibcNative.LibCStartMain(main, argc, argv, init, fini, rtLdFini, stackEnd);

        RemoveLibraryPreloadEnv();

        // Finally, we can redirect to our main
        _originalMain ??= Marshal.GetDelegateForFunctionPointer<MainFn>((nint)main);
        return LibcNative.LibCStartMain(&HookPlayerMain, argc, argv, init, fini, rtLdFini, stackEnd);
    }

    // This should only be called following a successful redirection by our __libc_start_main hook, and it
    // should only ever be called once since this getting called implies we removed ourselves from
    // LD_PRELOAD / LD_LIBRARY_PATH
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    [RequiresDynamicCode("Calls InitConfig")]
    public static unsafe int HookPlayerMain(int argc, char** argv, char** envp)
    {
        if (_originalMain is null)
            return 0;
        if (_hookPlayerMainEntered)
            return _originalMain(argc, argv, envp);
        _hookPlayerMainEntered = true;

        Core.Init();
        return _originalMain(argc, argv, envp);
    }
#endif

    // Herp: This is dirty Managed->Native->Managed pointer manipulation but trying to return a pointer causes crashes
    [UnmanagedCallersOnly(EntryPoint = "NativeHookAttach")]
    public static unsafe void NativeHookAttach(nint* target, nint detour)
    {
        *target = Dobby.HookAttach(*target, detour);
    }
    [UnmanagedCallersOnly(EntryPoint = "NativeHookDetach")]
    public static unsafe void NativeHookDetach(nint* target, nint detour)
    {
        Dobby.HookDetach(*target);
    }

    [UnmanagedCallersOnly(EntryPoint = "LogMsg")]
    public static unsafe void LogMsg(ColorARGB* msgColor, char* msg, int msgLength, ColorARGB* sectionColor, char* section, int sectionLength, char* strippedMsg, int strippedMsgLength)
    {
        if (msgColor == null || msg == null)
        {
            MelonLogger.LogSpacer();
            return;
        }

        var mMsg = new ReadOnlySpan<char>(msg, msgLength);
        var mStrippedMsg = new ReadOnlySpan<char>(strippedMsg, strippedMsgLength);

        if (sectionColor == null || section == null)
        {
            MelonLogger.Log(*msgColor, mMsg, mStrippedMsg);
            return;
        }

        MelonLogger.Log(*msgColor, mMsg, *sectionColor, new(section, sectionLength), mStrippedMsg);
    }

    [UnmanagedCallersOnly(EntryPoint = "LogError")]
    public static unsafe void LogError(char* msg, int msgLength, char* section, int sectionLength, bool warning)
    {
        var mMsg = new ReadOnlySpan<char>(msg, msgLength);
        if (section == null)
        {
            if (warning)
                MelonLogger.LogWarning(mMsg);
            else
                MelonLogger.LogError(mMsg);

            return;
        }

        if (warning)
            MelonLogger.LogWarning(mMsg, new(section, sectionLength));
        else
            MelonLogger.LogError(mMsg, new(section, sectionLength));
    }

    [UnmanagedCallersOnly(EntryPoint = "LogMelonInfo")]
    public static unsafe void LogMelonInfo(ColorARGB* nameColor, char* name, int nameLength, char* info, int infoLength)
    {
        MelonLogger.LogMelonInfo(*nameColor, new(name, nameLength), new(info, infoLength));
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoInstallHooks")]
    public static void MonoInstallHooks()
    {
        MonoHandler.InstallHooks();
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoGetDomainPtr")]
    public static nint MonoGetDomainPtr()
    {
        return MonoHandler.Domain;
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoGetRuntimeHandle")]
    public static nint MonoGetRuntimeHandle()
    {
        return MonoHandler.Mono.Handle;
    }

    [UnmanagedCallersOnly(EntryPoint = "IsConsoleOpen")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static bool IsConsoleOpen()
    {
        return ConsoleHandler.HasOwnWindow;
    }

    [UnmanagedCallersOnly(EntryPoint = "GetLoaderConfig")]
    public static unsafe void GetLoaderConfig(nint* pConfig)
    {
        Marshal.StructureToPtr(LoaderConfig.Current, *pConfig, false);
    }

    [UnmanagedCallersOnly(EntryPoint = "Il2CppGenerate")]
    public static unsafe void Il2CppGenerate(char* gameExePath, int gameExePathLength, char* outputFolder, int outputFolderLength, char* unstripDirectory, int unstripDirectoryLength)
    {
        RuntimeHandlers.Il2Cpp.Il2CppInteropGeneration.Run(new string(gameExePath, 0, gameExePathLength), new string(outputFolder, 0, outputFolderLength), new string(unstripDirectory, 0, unstripDirectoryLength));
    }
}