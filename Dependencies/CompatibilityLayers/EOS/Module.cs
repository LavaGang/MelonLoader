using MelonLoader.Modules;
using MelonLoader.NativeUtils;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(MelonLoader.BuildInfo.Description)]
[assembly: AssemblyDescription(MelonLoader.BuildInfo.Description)]
[assembly: AssemblyCompany(MelonLoader.BuildInfo.Company)]
[assembly: AssemblyProduct(MelonLoader.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + MelonLoader.BuildInfo.Author)]
[assembly: AssemblyTrademark(MelonLoader.BuildInfo.Company)]
[assembly: Guid("5100810A-9842-4073-9658-E5841FDF9D73")]
[assembly: AssemblyVersion(MelonLoader.BuildInfo.Version)]
[assembly: AssemblyFileVersion(MelonLoader.BuildInfo.Version)]
[assembly: MelonLoader.PatchShield]

namespace MelonLoader.CompatibilityLayers
{
    internal class EOS_Module : MelonModule
    {
        private delegate IntPtr LoadLibraryDetour(IntPtr path);
        private NativeHook<LoadLibraryDetour> _hookWin;

        //private delegate IntPtr dlopenDetour(IntPtr path, int flags);
        //private NativeHook<dlopenDetour> _hookUnix;

        public override void OnInitialize()
        {
            IntPtr ldlib = NativeLibrary.AgnosticGetLoadLibraryPtr();
            if (ldlib == IntPtr.Zero)
                return;

            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    _hookWin = new NativeHook<LoadLibraryDetour>(ldlib, 
                        Marshal.GetFunctionPointerForDelegate(DetourWin));
                    _hookWin.Attach();
                    break;

                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    // TO-DO

                    //_hookUnix = new NativeHook<dlopenDetour>(ldlib,
                    //    Marshal.GetFunctionPointerForDelegate(DetourUnix));
                    //_hookUnix.Attach();

                    break;
            }
        }

        private IntPtr DetourWin(IntPtr path)
        {
            if (path == IntPtr.Zero)
                return _hookWin.Trampoline(path);
            
            var pathString = Marshal.PtrToStringUni(path);
            if (pathString.EndsWith("EOSOVH-Win64-Shipping.dll") || pathString.EndsWith("EOSOVH-Win32-Shipping.dll"))
            {
                _hookWin.Detach();
                return IntPtr.Zero;
            }

            return _hookWin.Trampoline(path);
        }

        /*
        private IntPtr DetourUnix(IntPtr path, int flags)
        {
            if (path == IntPtr.Zero)
                return _hookUnix.Trampoline(path, flags);

            var pathString = Marshal.PtrToStringUni(path);
            if (pathString.StartsWith("EOSOVH-"))
            {
                _hookUnix.Detach();
                return IntPtr.Zero;
            }

            return _hookUnix.Trampoline(path, flags);
        }
        */

        ~EOS_Module()
        {
            _hookWin?.Detach();
            _hookWin = null;

            //_hookUnix?.Detach();
            //_hookUnix = null;
        }
    }
}
