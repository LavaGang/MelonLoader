using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap;

internal static partial class PltHook
{
    [LibraryImport("*", EntryPoint = "plthook_open", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int PlthookOpen(ref nint pltHookOut, string? filename);

    [LibraryImport("*", EntryPoint = "plthook_open_by_handle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int PlthookOpenByHandle(ref nint pltHookOut, nint handle);

    [LibraryImport("*", EntryPoint = "plthook_open_by_address")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int PlthookOpenByAddress(ref nint pltHookOut, nint address);

    [LibraryImport("*", EntryPoint = "plthook_replace", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe partial int PlthookReplace(nint pltHook, string funcName, nint funcAddr, nint oldFunc);

    [LibraryImport("*", EntryPoint = "plthook_close")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void PlthookClose(nint pltHook);

    [LibraryImport("*", EntryPoint = "plthook_enum", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe partial int PlthookEnum(nint pltHook, ref uint pos, byte** name, byte*** addr);

    [LibraryImport("*", EntryPoint = "plthook_error")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial nint PlthookError();

    private static bool WasError;
    private static string? PlayerFileName;
    private const string UnityPlayerLibName = "UnityPlayer";

    internal static void InstallHooks(List<(string functionName, nint hookFunctionPtr)> hooks)
    {
        if (!EnsureUnityPlayerLibrary())
            return;
        
        nint pltHook = IntPtr.Zero;
        if (PlthookOpen(ref pltHook, PlayerFileName) != 0)
        {
            MelonLogger.LogError($"plthook_open error: {Marshal.PtrToStringAuto(PlthookError())}");
            PlayerFileName = null;
            WasError = true;
            return;
        }

        foreach (var hook in hooks)
        {
            if (PlthookReplace(pltHook, hook.functionName, hook.hookFunctionPtr, IntPtr.Zero) != 0)
            {
                MelonDebug.Log($"plthook_replace error when hooking {hook.functionName}: " +
                               $"{Marshal.PtrToStringAuto(PlthookError())}");
                continue;
            }

            MelonDebug.Log($"Plt hooked {hook.functionName} successfully");
        }

        PlthookClose(pltHook);
    }

    private static bool EnsureUnityPlayerLibrary()
    {
        if (WasError)
            return false;
        
        if (!string.IsNullOrEmpty(PlayerFileName))
            return true;
        
        PlayerFileName = Process.GetCurrentProcess().Modules.OfType<ProcessModule>()
            .FirstOrDefault(x => x.FileName.Contains(UnityPlayerLibName))?.FileName;
        if (!string.IsNullOrEmpty(PlayerFileName))
        {
            MelonDebug.Log($"Attempting to use UnityPlayer: '{PlayerFileName}'");
            return true;
        }

        string processModulePath = Process.GetCurrentProcess().MainModule!.FileName;
        string parentPlayerPath = Path.GetDirectoryName(processModulePath)!;
        
#if OSX
        string libName = $"{UnityPlayerLibName}.dylib";
        parentPlayerPath = Path.Combine(Path.GetDirectoryName(parentPlayerPath)!, "Frameworks");
        PlayerFileName = Path.Combine(parentPlayerPath, libName);
#elif LINUX
        string libName = $"{UnityPlayerLibName}.so";
        PlayerFileName = Path.Combine(parentPlayerPath, libName);
#elif WINDOWS
        string libName = $"{UnityPlayerLibName}.dll";
        PlayerFileName = Path.Combine(parentPlayerPath, libName);
#endif

#if !WINDOWS
        if (!File.Exists(PlayerFileName))
        {
            libName = $"lib{UnityPlayerLibName}.so";
            PlayerFileName = Path.Combine(parentPlayerPath, libName);
        }
        if (!File.Exists(PlayerFileName))
        {
            libName = $"Lib{UnityPlayerLibName}.so";
            PlayerFileName = Path.Combine(parentPlayerPath, libName);
        }
#endif
        
        if (!File.Exists(PlayerFileName))
        {
            MelonLogger.LogError($"Unable to find {UnityPlayerLibName} library to apply plthook");
            PlayerFileName = null;
            WasError = true;
            return false;
        }
        
        MelonDebug.Log($"Attempting to use UnityPlayer: '{PlayerFileName}'");
        return true;
    }
}