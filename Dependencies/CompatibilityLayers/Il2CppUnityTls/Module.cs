using System;
using System.Reflection;
using MelonLoader.MonoInternals;

namespace MelonLoader.CompatibilityLayers
{
    internal class Il2CppUnityTls_Module : MelonCompatibilityLayer.Module
    {
        internal static MelonLogger.Instance Logger = new MelonLogger.Instance("Il2CppUnityTls");

        public override void Setup()
        {
            if (!PatchMonoExport())
            {
                Logger.Error("Failed to Bridge Il2Cpp Unity TLS! Web Connection based C# Methods may not work as intended.");
                return;
            }

            // Call InstallUnityTlsInterface Signature - Bootstrap - Il2Cpp::CallInstallUnityTLSInterface
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

        private static IntPtr GetUnityTlsInterface()
        {
            try
            {
                return Il2CppMono.Unity.UnityTls.GetUnityTlsInterface();
            }
            catch (Exception ex)
            {
                Logger.Error($"Il2CppMono.Unity.UnityTls.GetUnityTlsInterface threw Exception: {ex}");
                return IntPtr.Zero;
            }
        }
    }
}