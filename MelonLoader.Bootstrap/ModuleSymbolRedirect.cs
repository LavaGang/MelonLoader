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

        internal static nint GetSymbol(nint handle, string symbolName)
            => GetSymbol(handle, Marshal.StringToHGlobalAnsi(symbolName));
        internal static nint GetSymbol(nint handle, nint symbolName)
        {
            if ((handle == nint.Zero)
                || (symbolName == nint.Zero))
                return nint.Zero;

#if WINDOWS
            return GetProcAddress(handle, symbolName);
#elif LINUX || OSX
            return dlsym(handle, symbolName);
#else
            return nint.Zero;
#endif
        }

        private static nint SymbolDetour(nint handle, nint symbol)
        {
            nint originalSymbolAddress = GetSymbol(handle, symbol);

            string? symbolName = Marshal.PtrToStringAnsi(symbol);
            if (string.IsNullOrEmpty(symbolName)
                || string.IsNullOrWhiteSpace(symbolName))
                return originalSymbolAddress;

            //MelonDebug.Log($"Looking for Symbol {symbolName}");
            if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
                && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
                return originalSymbolAddress;

            if (!_runtimeInitialised)
            {
                _runtimeInitialised = true;
                MelonDebug.Log("Initializing Runtime");
                redirect.InitMethod(handle);
                if (!LoaderConfig.Current.Loader.CapturePlayerLogs)
                    ConsoleHandler.ResetHandles();
            }

            MelonDebug.Log($"Redirecting {symbolName}");
            return redirect.detourPtr;
        }

#if WINDOWS
        [DllImport("kernel32")]
        private static extern nint GetProcAddress(nint handle, nint symbol);
#elif LINUX
        [DllImport("libdl.so.2")]
        private static extern IntPtr dlsym(nint handle, nint symbol);
#elif OSX
        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlsym(nint handle, nint symbol);
#endif
    }
}
