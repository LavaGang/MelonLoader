using MelonLoader.Modules;
using MelonLoader.NativeUtils;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader.CompatibilityLayers
{
    internal class EOS_Module : MelonModule
    {
        private delegate IntPtr LoadLibraryDetour(IntPtr path);
        private NativeHook<LoadLibraryDetour> _hookWin;

        public override void OnInitialize()
        {
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    NativeLibrary lib = NativeLibrary.Load("kernel32");
                    if (lib != null)
                    {
                        IntPtr loadLibraryWPtr = lib.GetExport("LoadLibraryW");
                        if (loadLibraryWPtr != IntPtr.Zero)
                        {
                            IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate((LoadLibraryDetour)DetourWin);
                            _hookWin = new NativeHook<LoadLibraryDetour>(loadLibraryWPtr, detourPtr);
                            _hookWin.Attach();
                        }
                    }
                    break;

                case PlatformID.Unix:
                case PlatformID.MacOSX:

                    // TO-DO

                    // libdl.so.2
                    // dlopen

                    break;
            }
        }

        private IntPtr DetourWin(IntPtr path)
        {
            if (path == IntPtr.Zero)
                return _hookWin.Trampoline(path);
            
            var pathString = Marshal.PtrToStringUni(path);
            if (string.IsNullOrEmpty(pathString))
                return _hookWin.Trampoline(path);
				
            if (pathString.EndsWith("EOSOVH-Win64-Shipping.dll")
                || pathString.EndsWith("EOSOVH-Win32-Shipping.dll"))
            {
                _hookWin.Detach();
                return IntPtr.Zero;
            }

            return _hookWin.Trampoline(path);
        }

        ~EOS_Module()
        {
            _hookWin?.Detach();
            _hookWin = null;
        }
    }
}
