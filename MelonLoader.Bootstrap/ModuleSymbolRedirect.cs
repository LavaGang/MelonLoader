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
        private static readonly DetourFn DetourDelegate = RedirectSymbol;

        internal static void Attach()
        {
            MelonDebug.Log("Attaching Symbol Redirect...");

            IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate(DetourDelegate);

#if LINUX || OSX
            PltHook.InstallHooks
            ([
                ("dlsym", detourPtr)
            ]);
#endif
#if WINDOWS
            PltHook.InstallHooks
            ([
                ("GetProcAddress", detourPtr)
            ]);
#endif

            MelonDebug.Log("Symbol Redirect Attached!");
        }

        internal static nint GetSymbol(nint handle, string symbolName)
        {
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return nint.Zero;

#if WINDOWS
            return WindowsNative.GetProcAddress(handle, symbolName);
#elif LINUX || OSX
            return LibcNative.Dlsym(handle, symbol);
#endif
        }

        private static nint RedirectSymbol(nint handle, string symbolName)
        {
            nint originalSymbolAddress = GetSymbol(handle, symbolName);
            if (originalSymbolAddress == nint.Zero)
            {
                //MelonDebug.Log($"Unable to find Symbol: {symbolName}");
                return nint.Zero;
            }

            if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
                && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
            {
                //MelonDebug.Log($"No Redirect Found for Symbol: {symbolName}");
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
