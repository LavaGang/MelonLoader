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

        internal static nint GetSymbol(nint handle, string? symbol)
        {
            if ((handle == nint.Zero)
                || string.IsNullOrEmpty(symbol))
                return nint.Zero;

            // Herp: Using Prebuilt Interop classes for this caused weird crashing issues when attempting to marshal string to span
            // This works around the issue by manually importing the appropriate original export into a delegate and then calling original using that instead
#if WINDOWS
            return WindowsNative.GetProcAddress(handle, symbol);
#elif LINUX || OSX
            return LibcNative.Dlsym(handle, symbol);
#else
            return nint.Zero;
#endif
        }
        internal static nint GetSymbol(nint handle, nint symbol)
        {
            if ((handle == nint.Zero) 
                || (symbol == nint.Zero))
                return nint.Zero;

            return GetSymbol(handle, Marshal.PtrToStringAnsi(symbol));
        }

        private static nint SymbolDetour(nint handle, nint symbol)
        {
            string? symbolName = Marshal.PtrToStringAnsi(symbol);
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return nint.Zero;

            nint originalSymbolAddress = GetSymbol(handle, symbolName);
            if (originalSymbolAddress == nint.Zero)
                return nint.Zero;

            MelonDebug.Log($"Looking for Symbol {symbolName}");
            if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
                && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
                return originalSymbolAddress;

            if (!_runtimeInitialised)
            {
                MelonDebug.Log("Init");
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