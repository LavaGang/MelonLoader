using System;
using MelonLoader.Shared.NativeUtils;
using MelonLoader.Shared.Utils;
#pragma warning disable 0649

namespace MelonLoader.Mono
{
    public unsafe static class MonoLoader
    {
        #region Private Members

        private static Type mlCoreType = typeof(Shared.Core);

        private static MonoRuntimeInfo monoRuntimeInfo;
        private static IntPtr monoLib;

        private static IntPtr mono_jit_init_version;

        #endregion

        #region Bootstrap

        public unsafe static void Startup(MonoRuntimeInfo runtimeInfo)
        {
            // Apply the information
            monoRuntimeInfo = runtimeInfo;

            // Check if it found any Mono variant library
            if (monoRuntimeInfo == null
                || string.IsNullOrEmpty(monoRuntimeInfo.FilePath))
            {
                Assertion.ThrowInternalFailure($"Failed to find {runtimeInfo.Variant} Library!");
                return;
            }

            // Check if there is a Posix Helper included with the Mono variant library
            if (!string.IsNullOrEmpty(monoRuntimeInfo.PosixPath))
            {
                // Force-Load the Posix Helper before the actual Mono variant library
                // Otherwise can cause crashing in some cases
                MelonDebug.Msg(monoRuntimeInfo.PosixPath);
                MelonDebug.Msg("Posix Helper found! Loading...");
                if (!MelonNativeLibrary.TryLoad(runtimeInfo.PosixPath, out IntPtr _))
                {
                    Assertion.ThrowInternalFailure($"Failed to load {monoRuntimeInfo.Variant} Posix Helper from {monoRuntimeInfo.PosixPath}!");
                    return;
                }
            }

            // Load the Mono variant library
            MelonDebug.Msg(monoRuntimeInfo.FilePath);
            MelonDebug.Msg($"Loading {monoRuntimeInfo.Variant} Library...");
            if (!MelonNativeLibrary.TryLoad(monoRuntimeInfo.FilePath, out monoLib))
            {
                Assertion.ThrowInternalFailure($"Failed to load {monoRuntimeInfo.Variant} Library from {monoRuntimeInfo.FilePath}!");
                return;
            }

            // Get mono_jit_init_version Export
            MelonDebug.Msg($"Getting {nameof(mono_jit_init_version)} Export...");
            if (!MelonNativeLibrary.TryGetExport(monoLib, nameof(mono_jit_init_version), out mono_jit_init_version))
            {
                Assertion.ThrowInternalFailure($"Failed to find {nameof(mono_jit_init_version)} Export in {monoRuntimeInfo.Variant} Library!");
                return;
            }

            // Hook mono_jit_init_version
            MelonDebug.Msg($"Attaching Hook to {nameof(mono_jit_init_version)}...");
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_jit_init_version(IntPtr name, IntPtr version)
        {
            // Detach Hook
            MelonDebug.Msg($"Detaching Hook from mono_jit_init_version...");

            // Run Original
            MelonDebug.Msg("Creating Mono Domain...");
            //monoDomain = ;

            // Attempt to load ML into the Mono Domain

            // Return Domain
            return IntPtr.Zero;
        }

        #endregion
    }
}
