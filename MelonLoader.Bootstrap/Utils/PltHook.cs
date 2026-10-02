using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    private static string? PlayerFilePath;
    private static nint PlayerModuleHandle;
    private const string UnityPlayerLibName = "UnityPlayer";

    internal static void InstallHooks(List<(
        string functionName,
        nint hookFunctionPtr)> hooks)
    {
        if (!FindUnityPlayerLibrary()
            || !LoadUnityPlayerLibrary())
            return;
        
        nint pltHook = IntPtr.Zero;
        if (PlthookOpenByHandle(ref pltHook, PlayerModuleHandle) != 0)
        {
            Core.Logger.Error($"plthook_open_by_handle error: {Marshal.PtrToStringAuto(PlthookError())}");
            PlayerFilePath = null;
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

    private static bool LoadUnityPlayerLibrary()
    {
        if (WasError)
            return false;
        if (PlayerModuleHandle != IntPtr.Zero)
            return true;
        
        MelonDebug.Log($"Attempting to use UnityPlayer: '{PlayerFilePath}'");
        
#if WINDOWS
        PlayerModuleHandle = WindowsNative.LoadLibrary(PlayerFilePath!);
#else
        PlayerModuleHandle = LibcNative.Dlopen(PlayerFilePath!, LibcNative.RtldNow | LibcNative.RtldGlobal);
#endif
        if (PlayerModuleHandle == IntPtr.Zero)
        {
            Core.Logger.Error($"Failed to load {PlayerFilePath}, cannot apply plt hooks");
            PlayerModuleHandle = IntPtr.Zero;
            PlayerFilePath = null;
            WasError = true;
            return false;
        }
        
        Core.Logger.Msg($"Using UnityPlayer: '{PlayerFilePath}'");
        return true;
    }

    private static bool FindUnityPlayerLibrary()
    {
        if (WasError)
            return false;
        
        if (!string.IsNullOrEmpty(PlayerFilePath))
            return true;
        
        PlayerFilePath = Process.GetCurrentProcess().Modules.OfType<ProcessModule>()
            .FirstOrDefault(x => x.FileName.Contains(UnityPlayerLibName))?.FileName;
        if (!string.IsNullOrEmpty(PlayerFilePath))
            return true;

        string processModulePath = Process.GetCurrentProcess().MainModule!.FileName;
        string parentPlayerPath = Path.GetDirectoryName(processModulePath)!;
        
#if OSX
        string libName = $"{UnityPlayerLibName}.dylib";
        parentPlayerPath = Path.Combine(Path.GetDirectoryName(parentPlayerPath)!, "Frameworks");
        PlayerFilePath = Path.Combine(parentPlayerPath, libName);
#elif LINUX
        string libName = $"{UnityPlayerLibName}.so";
        PlayerFilePath = Path.Combine(parentPlayerPath, libName);
#elif WINDOWS
        string libName = $"{UnityPlayerLibName}.dll";
        PlayerFilePath = Path.Combine(parentPlayerPath, libName);
#endif

#if !WINDOWS
        if (!File.Exists(PlayerFilePath))
        {
            libName = $"lib{UnityPlayerLibName}.so";
            PlayerFilePath = Path.Combine(parentPlayerPath, libName);
        }
        if (!File.Exists(PlayerFilePath))
        {
            libName = $"Lib{UnityPlayerLibName}.so";
            PlayerFilePath = Path.Combine(parentPlayerPath, libName);
        }
#endif
        
        if (!File.Exists(PlayerFilePath))
        {
            Core.Logger.Error($"Unable to find {UnityPlayerLibName} library to apply plthook");
            PlayerFilePath = null;
            WasError = true;
            return false;
        }
        
        return true;
    }
}
