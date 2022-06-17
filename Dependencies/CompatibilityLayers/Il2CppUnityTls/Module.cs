using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using MelonLoader.Modules;
using MelonLoader.MonoInternals;
using MelonLoader.NativeUtils;
using UnityVersion = AssetRipper.VersionUtilities.UnityVersion;

namespace MelonLoader.CompatibilityLayers
{
    internal class Il2CppUnityTls_Module : MelonModule
    {
        private static Il2CppUnityTls_Module Instance;
        internal static MelonLogger.Instance Logger = new MelonLogger.Instance("Il2CppUnityTls");
        private static IntPtr UnityTlsInterface = IntPtr.Zero;

        private static string[] Signatures_x86 =
        {
            "A1 ?? ?? ?? ?? 85 C0 0F 85 68 01 00 00 A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? B8 ?? ?? ?? ?? C7 05",
            "A1 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 85 C0 0F 85 68 01 00 00 A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? A1 ?? ?? ?? ?? A3 ?? ?? ?? ?? B8 ?? ?? ?? ?? C7 05"
        };

        private static string[] Signatures_x64 =
        {
            "48 8B 0D ?? ?? ?? ?? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05",
            "48 8B 0D ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ?? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05",
            "48 8B 05 ?? ?? ?? ?? 48 85 C0 0F 85 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 89 05"
        };

        public unsafe override void OnInitialize()
        {
            Instance = this;

            Environment.SetEnvironmentVariable("MONO_TLS_PROVIDER", "default");

            if (!PatchExports())
                return;
            RunInstallFunction();
        }

        private unsafe static bool PatchExports()
        {
            IntPtr monolib = MonoLibrary.GetLibPtr();
            if (monolib == IntPtr.Zero)
            {
                Logger.Warning("Unable to find Mono Library Pointer!");
                return false;
            }

            NativeLibrary monoLibrary = new NativeLibrary(monolib);
            IntPtr mono_export = monoLibrary.GetExport("mono_unity_get_unitytls_interface");
            if (mono_export == IntPtr.Zero)
            {
                Logger.Warning("Unable to find Mono's mono_unity_get_unitytls_interface Export!");
                return false;
            }

            NativeLibrary il2cppLibrary = NativeLibrary.Load(Path.Combine(MelonUtils.GameDirectory, "GameAssembly.dll"));
            IntPtr il2cpp_export = il2cppLibrary.GetExport("il2cpp_unity_install_unitytls_interface");
            if (il2cpp_export == IntPtr.Zero)
            {
                Logger.Warning("Unable to find Il2Cpp's il2cpp_unity_install_unitytls_interface Export!");
                return false;
            }

            Logger.Msg("Patching mono_unity_get_unitytls_interface...");
            MelonUtils.NativeHookAttach((IntPtr)(&mono_export), typeof(Il2CppUnityTls_Module).GetMethod("GetUnityTlsInterface", BindingFlags.NonPublic | BindingFlags.Static).MethodHandle.GetFunctionPointer());

            Logger.Msg("Patching il2cpp_unity_install_unitytls_interface...");
            MelonUtils.NativeHookAttach((IntPtr)(&il2cpp_export), typeof(Il2CppUnityTls_Module).GetMethod("SetUnityTlsInterface", BindingFlags.NonPublic | BindingFlags.Static).MethodHandle.GetFunctionPointer());
            OriginalSetUnityTlsInterface = (dSetUnityTlsInterface)Marshal.GetDelegateForFunctionPointer(il2cpp_export, typeof(dSetUnityTlsInterface));

            return true;
        }

        private unsafe static void RunInstallFunction()
        {
            IntPtr unityplayer = GetUnityPlayerModule(out int unityplayer_size);
            if (unityplayer == IntPtr.Zero)
                return;

            IntPtr[] ptrs = null;
            if (MelonUtils.IsGame32Bit())
                ptrs = GetPointers(unityplayer, unityplayer_size, Signatures_x86);
            else
                ptrs = GetPointers(unityplayer, unityplayer_size, Signatures_x64);

            if ((ptrs == null) || (ptrs.Length <= 0))
            {
                Logger.Warning("Unable to find Il2CppInstallUnityTlsInterface!");
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

        private static IntPtr[] GetPointers(IntPtr unityplayer, int unityplayer_size, string[] possible_signatures)
        {
            foreach (string signature in possible_signatures)
            {
                IntPtr[] ptrs = CppUtils.SigscanAll(unityplayer, unityplayer_size, signature);
                if (ptrs.Length > 0)
                    return ptrs;
            }
            return null;
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
                Logger.Warning($"Failed to find module \"{moduleName}\"");
                return IntPtr.Zero;
            }

            return moduleAddress;
        }

        private static Type MonoTlsProviderFactory_Type;
        private static FieldInfo MonoTlsProviderFactory_providerRegistration;
        private static FieldInfo MonoTlsProviderFactory_providerCache;
        private static void CleanProviderRegistration()
        {
            if (MonoTlsProviderFactory_Type == null)
                MonoTlsProviderFactory_Type = AccessTools.TypeByName("Mono.Net.Security.MonoTlsProviderFactory");
            if (MonoTlsProviderFactory_providerRegistration == null)
                MonoTlsProviderFactory_providerRegistration = MonoTlsProviderFactory_Type.GetField("providerRegistration", BindingFlags.NonPublic | BindingFlags.Static);
            if (MonoTlsProviderFactory_providerCache == null)
                MonoTlsProviderFactory_providerCache = MonoTlsProviderFactory_Type.GetField("providerCache", BindingFlags.NonPublic | BindingFlags.Static);
            MonoTlsProviderFactory_providerRegistration.SetValue(null, null);
            MonoTlsProviderFactory_providerCache.SetValue(null, null);
        }

        private static IntPtr GetUnityTlsInterface()
            => UnityTlsInterface;
        private static void SetUnityTlsInterface(IntPtr ptr)
        {
            if(UnityTlsInterface != IntPtr.Zero)
                return;
            
            UnityTlsInterface = ptr;
            OriginalSetUnityTlsInterface(ptr);
            Environment.SetEnvironmentVariable("MONO_TLS_PROVIDER", "unitytls");
            CleanProviderRegistration();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void dSetUnityTlsInterface(IntPtr ptr);
        private static dSetUnityTlsInterface OriginalSetUnityTlsInterface;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidDelegate();
    }
}