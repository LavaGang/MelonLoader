using System;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Unity.Mono
{
    internal static class BootstrapMono
    {
        #region Private Members

        private static Type mlCoreType = typeof(Shared.Core);
        private static MonoNativeLibrary lib;

        private static MonoNativeLibrary.d_mono_jit_init_version o_mono_jit_init_version;
        private static MonoNativeLibrary.d_mono_runtime_invoke o_mono_runtime_invoke;

        private static MonoRuntimeInfo monoRuntimeInfo;
        private static IntPtr monoDomain;

        private static IntPtr mlAsm;
        private static IntPtr mlAsmImg;
        private static IntPtr mlCoreClass;
        private static IntPtr mlCoreMethodStartup;
        private static IntPtr mlCoreMethodOnAppPreStart;
        private static IntPtr mlCoreMethodOnAppStart;

        #endregion

        #region Bootstrap

        internal unsafe static void Startup(MonoRuntimeInfo runtimeInfo)
        {
            // Check if it found any Mono variant library
            if (runtimeInfo == null
                || string.IsNullOrEmpty(runtimeInfo.FilePath))
            {
                Assertion.ThrowInternalFailure($"Failed to find Mono or MonoBleedingEdge Library!");
                return;
            }

            // Apply the information
            monoRuntimeInfo = runtimeInfo;

            // Log Engine Variant
            MelonLogger.Msg($"Engine Variant: {runtimeInfo.Variant}");

            // Check if there is a Posix Helper included with the Mono variant library
            if (!string.IsNullOrEmpty(runtimeInfo.PosixPath))
            {
                // Force-Load the Posix Helper before the actual Mono variant library
                // Otherwise can cause crashing in some cases
                if (!MelonNativeLibrary.TryLoad(runtimeInfo.PosixPath, out IntPtr monoPosixHandle))
                {
                    Assertion.ThrowInternalFailure($"Failed to load {runtimeInfo.Variant} Posix Helper from {runtimeInfo.PosixPath}!");
                    return;
                }
            }

            // Load the Mono variant library
            lib = MelonNativeLibrary.ReflectiveLoad<MonoNativeLibrary>(runtimeInfo.FilePath);
            if (lib == null)
            {
                Assertion.ThrowInternalFailure($"Failed to load {runtimeInfo.Variant} Library from {runtimeInfo.FilePath}!");
                return;
            }

            // Validate Export References
            string failedExport = lib.Validation();
            if (!string.IsNullOrEmpty(failedExport))
            {
                Assertion.ThrowInternalFailure($"Failed to load Export {failedExport} from {runtimeInfo.Variant} Library!");
                return;
            }

            // Attach Hook to mono_jit_init_version
            o_mono_jit_init_version = BootstrapInterop.HookAttach(lib.mono_jit_init_version, h_mono_jit_init_version);
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_jit_init_version([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string version)
        {
            // Detach Hook
            BootstrapInterop.HookDetach(lib.mono_jit_init_version);

            // Run Original
            monoDomain = o_mono_jit_init_version(name, version);

            // Attempt to load ML into the Mono Domain
            if (LoadML())
            {
                // Run ML OnApplicationPreStart
                InvokeMethod(mlCoreMethodOnAppPreStart);

                // Attach Hook to mono_runtime_invoke
                o_mono_runtime_invoke = BootstrapInterop.HookAttach(lib.mono_runtime_invoke, h_mono_runtime_invoke);
            }

            // Return Domain
            return monoDomain;
        }

        private static IntPtr h_mono_runtime_invoke(IntPtr method, IntPtr obj, IntPtr prams, IntPtr exec)
        {
            // Run Original
            IntPtr returnval = o_mono_runtime_invoke(method, obj, prams, exec);

            // Get Invoked Method Name
            string methodName = lib.mono_method_get_name(method);

            // Check if Invoked Method is one that is Targeted
            if (IsTargetInvokedMethod(methodName))
            {
                // Detach Hook
                BootstrapInterop.HookDetach(lib.mono_runtime_invoke);

                // Run ML OnApplicationStart
                InvokeMethod(mlCoreMethodOnAppStart);
            }

            // Return Result
            return returnval;
        }

        #endregion

        #region Private Methods

        private static bool IsTargetInvokedMethod(string methodName)
        {
            // Check for Mono/MonoBleedingEdge Methods
            switch (methodName)
            {
                case "Internal_ActiveSceneChanged":
                case "UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize":
                    return true;

                default:
                    break;
            }

            // Check for Mono Methods
            if (monoRuntimeInfo.IsOldMono)
                switch (methodName)
                {
                    case "Awake":
                    case "DoSendMouseEvents":
                        return true;

                    default:
                        break;
                }

            return false;
        }

        private static void InvokeMethod(IntPtr method)
        {
            if (method == IntPtr.Zero)
                return;

            // Invoke Method
            if (o_mono_runtime_invoke == null)
                lib.mono_runtime_invoke(method, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            else
                o_mono_runtime_invoke(method, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            // TO-DO: Rethrow and Log Exception
        }

        private static bool LoadML()
        {
            // Get File Path
            string mlFilePath = Path.Combine(MelonEnvironment.MelonLoaderDirectory, "netfx35", Path.GetFileName(mlCoreType.Assembly.Location));
            
            // Load Assembly into Mono Domain
            mlAsm = lib.mono_domain_assembly_open(monoDomain, mlFilePath);
            if (mlAsm == IntPtr.Zero)
                return false;

            // Get Assembly Image
            mlAsmImg = lib.mono_assembly_get_image(mlAsm);
            if (mlAsmImg == IntPtr.Zero)
                return false;

            // Get ML Core class
            mlCoreClass = lib.mono_class_from_name(mlAsmImg, mlCoreType.Namespace, mlCoreType.Name);
            if (mlCoreClass == IntPtr.Zero)
                return false;

            // Get Startup Method
            mlCoreMethodStartup = lib.mono_class_get_method_from_name(mlCoreClass, nameof(Shared.Core.Startup), 0);
            if (mlCoreMethodStartup == IntPtr.Zero)
                return false;

            // Get Application Pre-Start Method
            mlCoreMethodOnAppPreStart = lib.mono_class_get_method_from_name(mlCoreClass, nameof(Shared.Core.OnApplicationPreStart), 0);
            if (mlCoreMethodOnAppPreStart == IntPtr.Zero)
                return false;

            // Get Application Start Method
            mlCoreMethodOnAppStart = lib.mono_class_get_method_from_name(mlCoreClass, nameof(Shared.Core.OnApplicationStart), 0);
            if (mlCoreMethodOnAppStart == IntPtr.Zero)
                return false;

            // Initiate ML by invoking Startup Method
            InvokeMethod(mlCoreMethodStartup);

            // Return Success
            return true;
        }

        #endregion
    }
}
