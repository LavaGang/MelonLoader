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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
#if WINDOWS
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
#endif
        private delegate nint DetourFn(nint handle, string symbol);
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
            MelonDebug.Log("Symbol Redirect Attached!");
        }

        internal static void Detach()
        {
            nativeHook?.Detach();
            nativeHook = null;
        }

        internal static nint GetSymbol(nint handle, string symbolName)
        {
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return nint.Zero;

            if ((nativeHook != null) && nativeHook.IsHooked)
                return nativeHook.Trampoline(handle, symbolName);

            if (NativeLibrary.TryGetExport(handle, symbolName, out var export))
                return export;

            return nint.Zero;
        }

        private static nint SymbolDetour(nint handle, string symbolName)
        {
            nint originalSymbolAddress = GetSymbol(handle, symbolName);
            if (originalSymbolAddress == nint.Zero)
            {
                MelonDebug.Log($"Unable to find Symbol: {symbolName}");
                return nint.Zero;
            }

            if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
                && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
            {
                MelonDebug.Log($"No Redirect Found for Symbol: {symbolName}");
                return originalSymbolAddress;
            }

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
