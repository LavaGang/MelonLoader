using MelonLoader.Modules;
using MelonLoader.NativeUtils;
using System;
using System.Diagnostics;
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
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        internal delegate IntPtr LoadLibraryWDetour(IntPtr path);
        internal static NativeHook<LoadLibraryWDetour> _hook = new NativeHook<LoadLibraryWDetour>();
        public override void OnInitialize()
        {
            var lib = new NativeLibrary(LoadLibrary("kernel32.dll"));
            var func = lib.GetExport("LoadLibraryW");

            var detour = Marshal.GetFunctionPointerForDelegate((LoadLibraryWDetour)Detour);

            _hook = new NativeHook<LoadLibraryWDetour>(func, detour);
            _hook.Attach();
        }

        private IntPtr Detour(IntPtr path)
        {
            if (path == IntPtr.Zero)
                return _hook.Trampoline(path);
            
            var pathString = Marshal.PtrToStringUni(path);
            if (pathString.EndsWith("EOSOVH-Win64-Shipping.dll") || pathString.EndsWith("EOSOVH-Win32-Shipping.dll"))
            {
                _hook.Detach();
                return IntPtr.Zero;
            }

            return _hook.Trampoline(path);
        }

        ~EOS_Module()
        {
            _hook.Detach();
        }
    }
}
