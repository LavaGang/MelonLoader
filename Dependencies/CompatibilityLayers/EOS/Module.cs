using MelonLoader.Modules;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader.CompatibilityLayers
{
    internal class EOS_Module : MelonModule
    {
        private delegate IntPtr dLoadLibrary(IntPtr path);
        private dLoadLibrary _loadLibrary;

        public unsafe override void OnInitialize()
        {
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    IntPtr trampoline = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)LoadLibrary);
                    IntPtr detour = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)DetourWin);
                    MelonUtils.NativeHookAttach((IntPtr)(&trampoline), detour);
                    _loadLibrary = (dLoadLibrary)Marshal.GetDelegateForFunctionPointer(trampoline, typeof(dLoadLibrary));
                    break;

                case PlatformID.Unix:
                case PlatformID.MacOSX:

                    // TO-DO

                    // libdl.so.2
                    // dlopen

                    break;
            }
        }

        private unsafe IntPtr DetourWin(IntPtr path)
        {
            if (path == IntPtr.Zero)
                return IntPtr.Zero;
            
            var pathString = Marshal.PtrToStringUni(path);
            if (string.IsNullOrEmpty(pathString))
                return IntPtr.Zero;

            if (pathString.EndsWith("EOSOVH-Win64-Shipping.dll")
                || pathString.EndsWith("EOSOVH-Win32-Shipping.dll"))
            {
                IntPtr trampoline = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)LoadLibrary);
                IntPtr detour = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)DetourWin);
                MelonUtils.NativeHookDetach((IntPtr)(&trampoline), detour);
                return IntPtr.Zero;
            }

            return _loadLibrary(path);
        }

        unsafe ~EOS_Module()
        {
            if (_loadLibrary != null)
            {
                IntPtr trampoline = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)LoadLibrary);
                IntPtr detour = Marshal.GetFunctionPointerForDelegate((dLoadLibrary)DetourWin);
                MelonUtils.NativeHookDetach((IntPtr)(&trampoline), detour);
                _loadLibrary = null;
            }
        }

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(IntPtr lpLibFileName);
    }
}
