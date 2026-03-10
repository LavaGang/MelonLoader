using MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap
{
    internal static partial class ModuleSymbolRedirect
    {
        private static bool _runtimeInitialised;

#if LINUX || OSX
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
#if WINDOWS
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
#endif
        private delegate nint DetourFn(nint handle, nint symbol);
        private static readonly DetourFn DetourDelegate = SymbolDetour;

        private static PltNativeHook<DetourFn>? nativeHook;

        internal static void Attach()
        {
            MelonDebug.Log("Attaching Symbol Redirect...");

            IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate(DetourDelegate);

#if LINUX || OSX
            nativeHook = PltNativeHook<DetourFn>.RedirectUnityPlayer("dlsym", detourPtr);
#endif

#if WINDOWS
            nativeHook = PltNativeHook<DetourFn>.RedirectUnityPlayer("GetProcAddress", detourPtr);
#endif

            nativeHook?.Attach();
        }

        internal static void Detach()
        {
            nativeHook?.Detach();
            nativeHook = null;
        }

        internal static nint GetSymbol(nint handle, nint symbolNamePtr)
        {
            if (symbolNamePtr == nint.Zero)
                return nint.Zero;

            if ((nativeHook != null) && nativeHook.IsHooked)
                return nativeHook.Trampoline(handle, symbolNamePtr);

            string? symbolName = Marshal.PtrToStringAnsi(symbolNamePtr);
            if (!string.IsNullOrEmpty(symbolName)
                && !string.IsNullOrWhiteSpace(symbolName)
                && NativeFunc.TryGetExport(handle, symbolName, out var export))
                return export;

            return nint.Zero;
        }

        internal static nint GetSymbol(nint handle, string symbolName)
        {
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return nint.Zero;

            if ((nativeHook != null) && nativeHook.IsHooked)
                return nativeHook.Trampoline(handle, Marshal.StringToHGlobalAnsi(symbolName));

            if (NativeLibrary.TryGetExport(handle, symbolName, out var export))
                return export;

            return nint.Zero;
        }

        private static nint SymbolDetour(nint handle, nint symbol)
        {
            nint originalSymbolAddress = GetSymbol(handle, symbol);

            string? symbolName = Marshal.PtrToStringAnsi(symbol);
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return originalSymbolAddress;

            if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
                && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
                return originalSymbolAddress;

            if (!_runtimeInitialised)
            {
                MelonDebug.Log("Initializing Runtime...");
                redirect.InitMethod(handle);
                if (!LoaderConfig.Current.Loader.CapturePlayerLogs)
                    ConsoleHandler.ResetHandles();
            }
            _runtimeInitialised = true;

            MelonDebug.Log($"Redirecting {symbolName}");
            return redirect.detourPtr;
        }
    }
}
