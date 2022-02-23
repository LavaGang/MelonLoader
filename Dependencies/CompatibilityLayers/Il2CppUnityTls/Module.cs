using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader.MonoInternals;
using MelonLoader.NativeUtils;
using UnityVersion = AssetRipper.VersionUtilities.UnityVersion;

namespace MelonLoader.CompatibilityLayers
{
    internal class Il2CppUnityTls_Module : MelonCompatibilityLayer.Module
    {
        internal static MelonLogger.Instance Logger = new MelonLogger.Instance("Il2CppUnityTls");
        private static IntPtr UnityTlsInterface = IntPtr.Zero;

        public unsafe override void Setup()
        {
            if (!PatchMonoExport())
            {
                Logger.Warning("Web Connection based C# Methods may not work as intended.");
                return;
            }

            if (!PatchIl2CppExport())
            {
                Logger.Warning("Web Connection based C# Methods may not work as intended.");
                return;
            }

            IntPtr unityplayer = GetUnityPlayerModule(out int unityplayer_size);
            if (unityplayer == IntPtr.Zero)
                return;

            IntPtr[] ptrs = null;
            if (MelonUtils.IsGame32Bit())
                ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "A1 ?? ?? ?? ?? 85 C0 0F 85 68 01 00 00 A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? B8 ?? ?? ?? ?? C7 05");
            else
                ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "48 8B 0D ?? ?? ?? ?? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05");

            if (ptrs.Length <= 0)
            {
                if (MelonUtils.IsGame32Bit())
                    ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "A1 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 85 C0 0F 85 68 01 00 00 A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? B8 ?? ?? ?? ?? C7 05");
                else
                    ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "48 8B 0D ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ?? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05");
            }

            if (ptrs.Length <= 0)
            {
                //if (MelonUtils.IsGame32Bit())
                //    ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "");
                //else
                if (!MelonUtils.IsGame32Bit())
                    ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, "48 8B 05 ?? ?? ?? ?? 48 85 C0 0F 85 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 05");
            }

            if (ptrs.Length <= 0)
            {
                Logger.Error("Il2CppInstallUnityTlsInterface Function was not found!");
                Logger.Warning("Web Connection based C# Methods may not work as intended.");
                return;
            }

            foreach (IntPtr ptr in ptrs)
            {
                byte* i = (byte*)ptr.ToPointer();
                if (*i == 0 || (*i & 0xF) == 0xF)
                    continue;
                Logger.Msg("Calling Il2CppInstallUnityTlsInterface...");
                VoidDelegate installUnityTlsInterface = (VoidDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(VoidDelegate));
                installUnityTlsInterface();
                break;
            }
        }

        private unsafe static bool PatchMonoExport()
        {
            IntPtr monolib = MonoLibrary.GetLibPtr();
            if (monolib == IntPtr.Zero)
                return false;

            NativeLibrary monoLibrary = new NativeLibrary(monolib);
            IntPtr export = monoLibrary.GetExport("mono_unity_get_unitytls_interface");
            if (export == IntPtr.Zero)
            {
                Logger.Error("Failed to find mono_unity_get_unitytls_interface! This should never happen...");
                return false;
            }

            Logger.Msg("Patching mono_unity_get_unitytls_interface...");
            MethodInfo patch = typeof(Il2CppUnityTls_Module).GetMethod("GetUnityTlsInterface", BindingFlags.NonPublic | BindingFlags.Static);
            IntPtr patchptr = patch.MethodHandle.GetFunctionPointer();
            MelonUtils.NativeHookAttach((IntPtr)(&export), patchptr);

            return true;
        }

        private unsafe static bool PatchIl2CppExport()
        {
            NativeLibrary il2cppLibrary = NativeLibrary.Load(Path.Combine(MelonUtils.GameDirectory, "GameAssembly.dll"));
            IntPtr export = il2cppLibrary.GetExport("il2cpp_unity_install_unitytls_interface");
            if (export == IntPtr.Zero)
            {
                Logger.Error("Failed to find il2cpp_unity_install_unitytls_interface!");
                return false;
            }

            Logger.Msg("Patching il2cpp_unity_install_unitytls_interface...");
            MethodInfo patch = typeof(Il2CppUnityTls_Module).GetMethod("SetUnityTlsInterface", BindingFlags.NonPublic | BindingFlags.Static);
            IntPtr patchptr = patch.MethodHandle.GetFunctionPointer();
            MelonUtils.NativeHookAttach((IntPtr)(&export), patchptr);
            OriginalSetUnityTlsInterface = (dSetUnityTlsInterface)Marshal.GetDelegateForFunctionPointer(export, typeof(dSetUnityTlsInterface));

            return true;
        }

        private static IntPtr GetUnityPlayerModule(out int moduleSize)
        {
            string moduleName = "UnityPlayer.dll";
#if LINUX
            // TODO
#elif OSX
            // TODO
#elif ANDROID
            // TODO
#else
            UnityVersion currentUnityVersion = InternalUtils.UnityInformationHandler.EngineVersion;
            UnityVersion minimumVersion = new UnityVersion(2017, 2);
            if (currentUnityVersion < minimumVersion)
                moduleName = "player_win.exe";
#endif

            IntPtr moduleAddress = IntPtr.Zero;
            moduleSize = 0;

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    moduleAddress = module.BaseAddress;
                    moduleSize = module.ModuleMemorySize;
                    break;
                }
            }

            if (moduleAddress == IntPtr.Zero)
            {
                moduleSize = 0;
                Logger.Error($"Failed to find module \"{moduleName}\"");
                return IntPtr.Zero;
            }

            return moduleAddress;
        }

        private static IntPtr GetUnityTlsInterface()
            => UnityTlsInterface;
        private static void SetUnityTlsInterface(IntPtr ptr)
        {
            UnityTlsInterface = ptr;
            OriginalSetUnityTlsInterface(ptr);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void dSetUnityTlsInterface(IntPtr ptr);
        private static dSetUnityTlsInterface OriginalSetUnityTlsInterface;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidDelegate();
    }
}