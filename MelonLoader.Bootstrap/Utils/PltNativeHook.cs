using MelonLoader.Bootstrap.Logging;
using MelonLoader.NativeUtils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils
{
    internal class PltNativeHook<T> : NativeHook<T> 
        where T : Delegate
    {
        internal nint ModuleHandle { get; private set; }
        internal string? ModuleFilePath { get; private set; }
        internal string? FunctionName { get; private set; }

        private static readonly string? PlayerFileName = Process.GetCurrentProcess().Modules.OfType<ProcessModule>()
            .FirstOrDefault(x => x.FileName.Contains("UnityPlayer"))?.FileName;
        internal static PltNativeHook<T>? RedirectUnityPlayer(string? functionName, nint detour)
        {
            PltNativeHook<T>? newHook = null;

#if OSX
            string parentPlayerPath = Path.GetDirectoryName(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName))!;
            string playerLibPath = Path.Combine(parentPlayerPath, "Frameworks", "UnityPlayer.dylib");
            if (File.Exists(playerLibPath))
            {
                nint libHandle = LibcNative.Dlopen(playerLibPath, LibcNative.RtldLazy | LibcNative.RtldNoLoad);
                if (libHandle == IntPtr.Zero)
                {
                    MelonLogger.LogError($"Failed to dlopen {playerLibPath}, cannot apply plt hooks");
                    return null;
                }

                newHook = new(libHandle, functionName, detour);
            }
            else
#endif
            newHook = new(PlayerFileName, functionName, detour);

            return newHook;
        }

        internal PltNativeHook(string? moduleFilePath, string? functionName, nint detour)
        {
            if (string.IsNullOrEmpty(moduleFilePath)
                || string.IsNullOrWhiteSpace(moduleFilePath))
                throw new ArgumentNullException("moduleFilePath");

            if (string.IsNullOrEmpty(functionName)
                || string.IsNullOrWhiteSpace(functionName))
                throw new ArgumentNullException("functionName");

            if (detour == IntPtr.Zero)
                throw new ArgumentNullException("detour");

            ModuleFilePath = moduleFilePath;
            FunctionName = functionName;
            _detourHandle = detour;
        }

        internal PltNativeHook(nint moduleHandle, string? functionName, nint detour)
        {
            if (moduleHandle == IntPtr.Zero)
                throw new ArgumentNullException("moduleHandle");

            if (string.IsNullOrEmpty(functionName)
                || string.IsNullOrWhiteSpace(functionName))
                throw new ArgumentNullException("functionName");

            if (detour == IntPtr.Zero)
                throw new ArgumentNullException("detour");

            ModuleHandle = moduleHandle;
            FunctionName = functionName;
            _detourHandle = detour;
        }

        private bool OpenModule(ref nint pltHook)
        {
            bool pltHookOpened = false;
            if (ModuleHandle != IntPtr.Zero)
                pltHookOpened = PltHook.PlthookOpenByHandle(ref pltHook, ModuleHandle) == 0;
            else
            {
                if (string.IsNullOrEmpty(ModuleFilePath)
                    || string.IsNullOrWhiteSpace(ModuleFilePath))
                    return false;

                pltHookOpened = PltHook.PlthookOpen(ref pltHook, ModuleFilePath) == 0;
            }

            if (!pltHookOpened)
            {
                MelonLogger.LogError($"plthook_open error: {Marshal.PtrToStringAnsi(PltHook.PlthookError())}");
                return false;
            }
            return true;
        }

        internal override unsafe void HookAttach()
        {
            if (string.IsNullOrEmpty(FunctionName)
                || string.IsNullOrWhiteSpace(FunctionName))
                return;

            nint pltHook = IntPtr.Zero;
            if (!OpenModule(ref pltHook))
                return;

            nint oldFuncPtr = nint.Zero;
            if (PltHook.PlthookReplace(pltHook, FunctionName, _detourHandle, (IntPtr)(&oldFuncPtr)) != 0)
            {
                MelonDebug.Log($"plthook_replace error when hooking {FunctionName}: " +
                               $"{Marshal.PtrToStringAuto(PltHook.PlthookError())}");
                PltHook.PlthookClose(pltHook);
                return;
            }

            _trampolineHandle = oldFuncPtr;
            _trampoline = Marshal.GetDelegateForFunctionPointer<T>(_trampolineHandle);
            PltHook.PlthookClose(pltHook);
        }

        internal override void HookDetach()
        {
            if (string.IsNullOrEmpty(ModuleFilePath)
                || string.IsNullOrWhiteSpace(ModuleFilePath))
                return;
            if (string.IsNullOrEmpty(FunctionName)
                || string.IsNullOrWhiteSpace(FunctionName))
                return;
            
            nint pltHook = IntPtr.Zero;
            if (!OpenModule(ref pltHook))
                return;

            if (PltHook.PlthookReplace(pltHook, FunctionName, _trampolineHandle, IntPtr.Zero) != 0)
            {
                MelonDebug.Log($"plthook_replace error when unhooking {FunctionName}: " +
                               $"{Marshal.PtrToStringAuto(PltHook.PlthookError())}");
            }

            PltHook.PlthookClose(pltHook);
        }
    }
}
