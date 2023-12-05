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

        private static MelonNativeHook<MonoLibrary.d_mono_jit_init_version> mono_jit_init_version_hook;

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
                Assertion.ThrowInternalFailure($"Failed to find {RuntimeInfo.Variant} Library!");
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

            // Create Hooks
            CreateHooks();

            // Attach mono_jit_init_version Hook
            MelonDebug.Msg($"Attaching Hook to {nameof(MonoLibrary.Instance.mono_jit_init_version)}...");
            mono_jit_init_version_hook.Attach();
        }

        private static void CreateHooks()
        {
            mono_jit_init_version_hook = new MelonNativeHook<MonoLibrary.d_mono_jit_init_version>(
                Marshal.GetFunctionPointerForDelegate(MonoLibrary.Instance.mono_jit_init_version),
                Marshal.GetFunctionPointerForDelegate(h_mono_jit_init_version));
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
                Assertion.ThrowInternalFailure($"Failed to load {RuntimeInfo.Variant} Posix Helper from {RuntimeInfo.PosixPath}!");
                return false;
            }

            // Return Success
            return true;
        }

        private static bool LoadLib()
        {
            // Load the Mono variant library
            MelonDebug.Msg(RuntimeInfo.LibPath);
            MelonDebug.Msg($"Loading {RuntimeInfo.Variant} Library...");
            MonoLibrary.Instance = MelonNativeLibrary.ReflectiveLoad<MonoLibrary>(RuntimeInfo.LibPath);

            // Check for Failure
            if (MonoLibrary.Instance == null)
            {
                Assertion.ThrowInternalFailure($"Failed to load {RuntimeInfo.Variant} Library from {RuntimeInfo.LibPath}!");
                return false;
            }

            // See if any Exports Failed
            if (MonoLibrary.Instance.mono_jit_init_version == null)
            {
                Assertion.ThrowInternalFailure($"Failed to find {nameof(MonoLibrary.Instance.mono_jit_init_version)} Export in {RuntimeInfo.Variant} Library!");
                return false;
            }

            return true;
        }

        private static bool CheckExports()
        {
            if (MonoLibrary.Instance.mono_jit_init_version == null)
            {
                Assertion.ThrowInternalFailure($"Failed to find {nameof(MonoLibrary.Instance.mono_jit_init_version)} Export in {RuntimeInfo.Variant} Library!");
                return false;
            }

            return true;
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_jit_init_version(IntPtr name, IntPtr version)
        {
            // Detach Hook
            MelonDebug.Msg($"Detaching Hook from mono_jit_init_version...");
            mono_jit_init_version_hook.Detach();

            // Run Original
            MelonDebug.Msg("Creating Mono Domain...");
            monoDomain = MonoLibrary.Instance.mono_jit_init_version(name, version);

            // Attempt to load ML into the Mono Domain

            // Return Domain
            return monoDomain;
        }

        #endregion
    }
}
