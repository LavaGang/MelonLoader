using System;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;
using MelonLoader.Utils;
#pragma warning disable 0649

namespace MelonLoader.Mono.Bootstrap
{
    public unsafe static class MonoLoader
    {
        #region Private Members

        private static Type mlCoreType = typeof(Core);

        private static IntPtr monoDomain;
        private static MelonNativeDetour<MonoLibrary.d_mono_init_version> mono_init_version_detour;
        private static MelonNativeDetour<MonoLibrary.d_mono_runtime_invoke> mono_runtime_invoke_detour;

        #endregion

        #region Public Members

        public static MonoRuntimeInfo RuntimeInfo { get; private set; }

        #endregion

        #region Bootstrap

        public unsafe static void Startup(MonoRuntimeInfo runtimeInfo)
        {
            // Apply the information
            RuntimeInfo = runtimeInfo;

            // Check if it found any Mono variant library
            if (RuntimeInfo == null
                || string.IsNullOrEmpty(RuntimeInfo.LibPath))
            {
                Assertion.ThrowInternalFailure($"Failed to find {RuntimeInfo.VariantName} Library!");
                return;
            }

            // Load Posix Helper
            if (!LoadPosixHelper())
                return;

            // Load Library
            if (!LoadLib())
                return;

            // Check Exports
            if (!CheckExports())
                return;

            // Create mono_runtime_invoke Detour
            mono_runtime_invoke_detour = new(MonoLibrary.Instance.mono_runtime_invoke, h_mono_runtime_invoke, false);

            // Get Mono Domain
            monoDomain = MonoLibrary.Instance.mono_domain_get();

            // Handle loading if Domain is already Created
            if (monoDomain == IntPtr.Zero)
            {
                // Attach mono_init_version Detour
                if (RuntimeInfo.Variant == eMonoRuntimeVariant.Mono)
                    mono_init_version_detour = new(MonoLibrary.Instance.mono_init_version, h_mono_init_version);
                else
                    mono_init_version_detour = new(MonoLibrary.Instance.mono_jit_init_version, h_mono_init_version);
            }
            else
            {
                // Initiate Stage 1 of Domain Injection
                Stage1();
            }
        }

        private static bool LoadPosixHelper()
        {
            // Check if there is a Posix Helper included with the Mono variant library
            if (string.IsNullOrEmpty(RuntimeInfo.PosixPath))
                return true;

            // Force-Load the Posix Helper before the actual Mono variant library
            // Otherwise can cause crashing in some cases
            MelonDebug.Msg(RuntimeInfo.PosixPath);
            MelonDebug.Msg("Posix Helper found! Loading...");
            if (!MelonNativeLibrary.TryLoad(RuntimeInfo.PosixPath, out IntPtr _))
            {
                Assertion.ThrowInternalFailure($"Failed to load {RuntimeInfo.VariantName} Posix Helper from {RuntimeInfo.PosixPath}!");
                return false;
            }

            // Return Success
            return true;
        }

        private static bool LoadLib()
        {
            // Load the Mono variant library
            MelonDebug.Msg(RuntimeInfo.LibPath);
            MelonDebug.Msg($"Loading {RuntimeInfo.VariantName} Library...");
            MonoLibrary.Instance = MelonNativeLibrary.ReflectiveLoad<MonoLibrary>(RuntimeInfo.LibPath);

            // Check for Failure
            if (MonoLibrary.Instance == null)
            {
                Assertion.ThrowInternalFailure($"Failed to load {RuntimeInfo.VariantName} Library from {RuntimeInfo.LibPath}!");
                return false;
            }

            // See if any Exports Failed
            if (MonoLibrary.Instance.mono_runtime_invoke == null)
            {
                Assertion.ThrowInternalFailure($"Failed to find {nameof(MonoLibrary.Instance.mono_runtime_invoke)} Export in {RuntimeInfo.VariantName} Library!");
                return false;
            }

            return true;
        }

        private static bool CheckExports()
        {
            if (MonoLibrary.Instance.mono_runtime_invoke == null)
            {
                Assertion.ThrowInternalFailure($"Failed to find {nameof(MonoLibrary.Instance.mono_runtime_invoke)} Export in {RuntimeInfo.VariantName} Library!");
                return false;
            }

            return true;
        }

        private static void Stage1()
        {
            MelonDebug.Msg("STAGE 1");

            // Load MelonLoader.Shared.dll

            // Invoke MelonLoader.Core.Startup()

            // Invoke MelonLoader.Core.PreStart()

            // Attach mono_runtime_invoke Detour
            mono_runtime_invoke_detour.Attach();
        }

        private static void Stage2()
        {
            MelonDebug.Msg("STAGE 2");

            // Invoke MelonLoader.Core.OnApplicationStart()
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_init_version([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string version)
        {
            // Invoke Domain Creation
            monoDomain = mono_init_version_detour.Trampoline(name, version);

            // Detach mono_init_version Detour
            mono_init_version_detour.Detach();

            // Initiate Stage 1 of Domain Injection
            Stage1();

            // Return Domain
            return monoDomain;
        }

        private static IntPtr h_mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc)
        {
            // Get Method Name
            string methodName = Marshal.PtrToStringAnsi(MonoLibrary.Instance.mono_method_get_name(method));

            // Check for Trigger Methods
            if (methodName.Contains("Internal_ActiveSceneChanged")
                || methodName.Contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize")
                || ((RuntimeInfo.Variant == eMonoRuntimeVariant.Mono)
                    && (methodName.Contains("Awake") 
                        || methodName.Contains("DoSendMouseEvents"))))
            {
                // Detach mono_runtime_invoke Detour
                mono_runtime_invoke_detour.Detach();

                // Initiate Stage 1 of Domain Injection
                Stage2();

                // Return original Invoke without Trampoline
                return MonoLibrary.Instance.mono_runtime_invoke(method, obj, param, ref exc);
            }

            // Return original Invoke
            return mono_runtime_invoke_detour.Trampoline(method, obj, param, ref exc);
        }

        #endregion
    }
}
