using System.Runtime.InteropServices;
using System.Text;
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal static class DotnetHandler
{
    private const string _rootKey = "DOTNET_ROOT";
    private static DotnetLib? _module;
    private static Action? _startFunc;
    
    public static void Initialize()
    {
        var managedDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "net10.0");
        var runtimeConfigPath = Path.Combine(managedDir, "MelonLoader.runtimeconfig.json");
        var nativeHostPath = Path.Combine(managedDir, "MelonLoader.NativeHost.dll");

        if (!File.Exists(runtimeConfigPath))
        {
            Core.Logger.Error($"Runtime config not found at: '{runtimeConfigPath}'");
            return;
        }

        if (!File.Exists(nativeHostPath))
        {
            Core.Logger.Error($"NativeHost not found at: '{runtimeConfigPath}'");
            return;
        }
        
        // Try to use a portable runtime from config
        string portableDir = LoaderConfig.Current.Loader.HostFXRPathOverride;
        if (!string.IsNullOrEmpty(portableDir)
            && !string.IsNullOrWhiteSpace(portableDir))
        {
            if (File.Exists(portableDir))
                portableDir = Path.GetDirectoryName( // dotnet
                    Path.GetDirectoryName( // host
                        Path.GetDirectoryName( // fxr
                            Path.GetDirectoryName( // 10.x.x
                                portableDir))))!;

            MelonDebug.Log($"Attempting to load hostfxr using .NET runtime from: {portableDir}");
            if (ScanDirectory(portableDir)
                && InitializeDomain(runtimeConfigPath, nativeHostPath))
                return;
        }

        // Try to use a portable runtime from game directory
        portableDir = Path.Combine(Exports.ProcessDirectory, "dotnet");
        MelonDebug.Log($"Attempting to load hostfxr using .NET runtime from: {portableDir}");
        if (ScanDirectory(portableDir)
            && InitializeDomain(runtimeConfigPath, nativeHostPath))
            return;

        string basePortableDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "dotnet");
        if (portableDir != basePortableDir)
        {
            portableDir = basePortableDir;
            MelonDebug.Log($"Attempting to load hostfxr using .NET runtime from: {portableDir}");
            if (ScanDirectory(portableDir)
                && InitializeDomain(runtimeConfigPath, nativeHostPath))
                return;
        }
        
        // Try to use a portable runtime from repository
        portableDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Dependencies", "dotnet");
        MelonDebug.Log($"Attempting to load hostfxr using .NET runtime from: {portableDir}");
        if (ScanDirectory(portableDir)
            && InitializeDomain(runtimeConfigPath, nativeHostPath))
            return;

        // Try the normal system detection/installation
        MelonDebug.Log("Attempting to load hostfxr from system");
        if (DotnetLib.GetHostFxrSystemPath(out var path)
            && !string.IsNullOrEmpty(path)
            && ((_module = DotnetLib.TryLoad(path)) != null))
        {
            Core.Logger.Msg($"Using .NET runtime: '{path}'");
            if (InitializeDomain(runtimeConfigPath, nativeHostPath))
                return;
        }

#if WINDOWS
        // Try to install runtime then attempt system detection/installation again
        MelonDebug.Log("Attempting to reinstall .NET runtime and load hostfxr from system");
        if (DotnetInstaller.AttemptInstall()
            && DotnetLib.GetHostFxrSystemPath(out path)
            && !string.IsNullOrEmpty(path)
            && ((_module = DotnetLib.TryLoad(path)) != null))
        {
            Core.Logger.Msg($"Using .NET runtime: '{path}'");
            if (InitializeDomain(runtimeConfigPath, nativeHostPath))
                return;
        }
#endif
        
#if X64 && (WINDOWS || OSX || LINUX)
        // Try to download portable runtime from repository then attempt to use it again
        MelonDebug.Log($"Attempting to download .NET runtime from repository and load hostfxr from: {portableDir}");
        if (DotnetPortable.AttemptInstall()
            && ScanDirectory(portableDir)
            && InitializeDomain(runtimeConfigPath, nativeHostPath))
            return;
#endif
        
        // Failure
        Core.Logger.Error("Failed to load Hostfxr");
    }

    private static bool InitializeDomain(string runtimeConfigPath, string nativeHostPath)
    {
        MelonDebug.Log("Initializing domain");
        if (!_module!.InitializeForRuntimeConfig(runtimeConfigPath, out var context))
        {
            Core.Logger.Error($"Failed to initialize a .NET domain");
            return false;
        }

        MelonDebug.Log("Loading NativeHost assembly");
        var initialize = _module!.LoadAssemblyAndGetFunctionUco<InitializeFn>(context, nativeHostPath, "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost", "NativeEntry");
        if (initialize == null)
        {
            Core.Logger.Error($"Failed to load assembly from: '{nativeHostPath}'");
            return false;
        }

        var startFuncPtr = Exports.LibraryHandle;

        MelonDebug.Log("Invoking NativeHost entry");
        initialize(ref startFuncPtr);

        if (startFuncPtr == 0 || startFuncPtr == Exports.LibraryHandle)
        {
            Core.Logger.Error($"Managed did not return the initial function pointer");
            return false;
        }

        _startFunc = Marshal.GetDelegateForFunctionPointer<Action>(startFuncPtr);
        return true;
    }
    
    public static void Start()
    {
        _startFunc?.Invoke();
    }
    
    public static bool ScanDirectory(string dotnetPath)
    {
        string pathKey = "PATH";
        string? hostfxrPath = null;
        try
        {
            if (!Directory.Exists(dotnetPath))
                return false;
            if (!FindHostFxrInDirectory(dotnetPath, out hostfxrPath))
                return false;
        }
        catch (Exception ex)
        {
            if (LoaderConfig.Current.Loader.DebugMode)
                MelonLogger.LogError(ex.ToString());
            return false;
        }

        string? originalRoot = null;
        string? originalPath = null;
        try
        {
            // Get Original Root and Path
            originalRoot = Environment.GetEnvironmentVariable(_rootKey);
            originalPath = Environment.GetEnvironmentVariable(pathKey);
        }
        catch (Exception ex)
        {
            if (LoaderConfig.Current.Loader.DebugMode)
                MelonLogger.LogError(ex.ToString());
            return false;
        }

        try
        {
            // Temporarily Apply New Root
            Environment.SetEnvironmentVariable(_rootKey, dotnetPath);

            // Temporarily Apply New Path
            string path = originalPath ?? string.Empty;
            var pathEntries = path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (!pathEntries.Any(p => string.Equals(p, dotnetPath, StringComparison.OrdinalIgnoreCase)))
                Environment.SetEnvironmentVariable(pathKey, dotnetPath + Path.PathSeparator + path);
            
            // Attempt to load HostFXR
            if (((_module = DotnetLib.TryLoad(hostfxrPath!)) != null))
            {
                Core.Logger.Msg($"Using .NET runtime: '{dotnetPath}'");
                return true;
            }
        }
        catch (Exception ex)
        {
            if (LoaderConfig.Current.Loader.DebugMode)
                MelonLogger.LogError(ex.ToString());
        }
        
        // Restore Original Root and Path
        try
        {
            Environment.SetEnvironmentVariable(_rootKey, originalRoot);
            Environment.SetEnvironmentVariable(pathKey, originalPath);
        }
        catch (Exception ex)
        {
            if (LoaderConfig.Current.Loader.DebugMode)
                MelonLogger.LogError(ex.ToString());
        }

        return false;
    }

    private static bool FindHostFxrInDirectory(string? path, out string? hostfxrPath)
    {
        hostfxrPath = null;
        if (string.IsNullOrEmpty(path))
            return false;
        
        string filename = $"hostfxr.{Exports.LibExtension}";
#if !WINDOWS
        filename = $"lib{filename}";
#endif

        string[] foundFiles = Directory.GetFiles(path, filename, SearchOption.AllDirectories);
        hostfxrPath = foundFiles.FirstOrDefault();
        return (hostfxrPath != null);
    }
    
    // Requires the bootstrap handle to be passed first
    private delegate void InitializeFn(ref nint startFunc);
}
