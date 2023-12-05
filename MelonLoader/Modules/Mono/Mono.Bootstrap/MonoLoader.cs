using System;
using System.IO;
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

        private static IntPtr mlSharedAsm;
        private static IntPtr mlSharedAsmImage;
        private static IntPtr mlSharedCore;

        private static IntPtr mlShared_Startup;
        private static IntPtr mlShared_OnApplicationPreStart;
        private static IntPtr mlShared_OnApplicationStart;

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
                MelonDebug.Msg($"Attaching mono_init_version Detour...");
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

            return true;
        }

        private static bool CheckExports()
        {
            string initName = (RuntimeInfo.Variant == eMonoRuntimeVariant.Mono) 
                ? nameof(MonoLibrary.Instance.mono_init_version) 
                : nameof(MonoLibrary.Instance.mono_jit_init_version);

            Delegate initDel = (RuntimeInfo.Variant == eMonoRuntimeVariant.Mono) 
                ? MonoLibrary.Instance.mono_init_version
                : MonoLibrary.Instance.mono_jit_init_version;

            (string, Delegate)[] listOfExports = new[]
            {
                (initName, initDel),
                (nameof(MonoLibrary.Instance.mono_domain_get), MonoLibrary.Instance.mono_domain_get),

                (nameof(MonoLibrary.Instance.mono_assembly_open_full), MonoLibrary.Instance.mono_assembly_open_full),
                (nameof(MonoLibrary.Instance.mono_assembly_get_image), MonoLibrary.Instance.mono_assembly_get_image),

                (nameof(MonoLibrary.Instance.mono_class_from_name), MonoLibrary.Instance.mono_class_from_name),

                (nameof(MonoLibrary.Instance.mono_method_get_name), MonoLibrary.Instance.mono_method_get_name),
                (nameof(MonoLibrary.Instance.mono_runtime_invoke), MonoLibrary.Instance.mono_runtime_invoke),
            };

            foreach (var exportPair in listOfExports)
            {
                if (exportPair.Item2 != null)
                    continue;

                Assertion.ThrowInternalFailure($"Failed to find {exportPair.Item1} Export in {RuntimeInfo.VariantName} Library!");
                return false;
            }

            return true;
        }

        private static void Stage1()
        {
            // Get File Path for MelonLoader.Shared.dll
            string fileName = Path.GetFileName(mlCoreType.Assembly.Location);
            string sharedPath = Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(mlCoreType.Assembly.Location)),
                "net35",
                fileName);

            // Inject BootstrapInterop Internal Calls

            // Load Assembly
            MelonDebug.Msg($"Loading Assembly {sharedPath}...");
            mlSharedAsm = MonoLibrary.Instance.mono_assembly_open_full(sharedPath.ToAnsiPointer(), IntPtr.Zero, false);
            if (mlSharedAsm == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to load Assembly from {sharedPath}!");
                return;
            }

            // Get Assembly Image
            MelonDebug.Msg("Getting Assembly Image...");
            mlSharedAsmImage = MonoLibrary.Instance.mono_assembly_get_image(mlSharedAsm);
            if (mlSharedAsmImage == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to get Assembly Image from {fileName}!");
                return;
            }

            // Get MelonLoader.Core
            MelonDebug.Msg($"Getting Class {mlCoreType.Namespace}.{mlCoreType.Name}...");
            mlSharedCore = MonoLibrary.Instance.mono_class_from_name(mlSharedAsmImage, mlCoreType.Namespace.ToAnsiPointer(), mlCoreType.Name.ToAnsiPointer());
            if (mlSharedCore == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to get Class {mlCoreType.Namespace}.{mlCoreType.Name} from {fileName}!");
                return;
            }

            // Get MelonLoader.Core.Startup
            MelonDebug.Msg($"Getting Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.Startup)}...");
            mlShared_Startup = MonoLibrary.Instance.mono_class_get_method_from_name(mlSharedCore, nameof(Core.Startup).ToAnsiPointer(), 0);
            if (mlShared_Startup == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to get Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.Startup)} from {fileName}!");
                return;
            }

            // Get MelonLoader.Core.OnApplicationPreStart
            MelonDebug.Msg($"Getting Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationPreStart)}...");
            mlShared_OnApplicationPreStart = MonoLibrary.Instance.mono_class_get_method_from_name(mlSharedCore, nameof(Core.OnApplicationPreStart).ToAnsiPointer(), 0);
            if (mlShared_OnApplicationPreStart == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to get Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationPreStart)} from {fileName}!");
                return;
            }

            // Get MelonLoader.Core.OnApplicationStart
            MelonDebug.Msg($"Getting Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationStart)}...");
            mlShared_OnApplicationStart = MonoLibrary.Instance.mono_class_get_method_from_name(mlSharedCore, nameof(Core.OnApplicationStart).ToAnsiPointer(), 0);
            if (mlShared_OnApplicationStart == IntPtr.Zero)
            {
                Assertion.ThrowInternalFailure($"Failed to get Method {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationStart)} from {fileName}!");
                return;
            }

            // Invoke MelonLoader.Core.Startup()
            MelonDebug.Msg($"Invoking {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.Startup)}...");
            IntPtr exc = IntPtr.Zero;
            MonoLibrary.Instance.mono_runtime_invoke(mlShared_Startup, IntPtr.Zero, (void**)null, ref exc);

            // Invoke MelonLoader.Core.PreStart()
            MelonDebug.Msg($"Invoking {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationPreStart)}...");
            MonoLibrary.Instance.mono_runtime_invoke(mlShared_OnApplicationPreStart, IntPtr.Zero, (void**)null, ref exc);

            // Attach mono_runtime_invoke Detour
            MelonDebug.Msg($"Attaching mono_runtime_invoke Detour...");
            mono_runtime_invoke_detour.Attach();
        }

        private static void Stage2()
        {
            // Invoke MelonLoader.Core.OnApplicationStart()
            MelonDebug.Msg($"Invoking {mlCoreType.Namespace}.{mlCoreType.Name}.{nameof(Core.OnApplicationStart)}...");
            IntPtr exc = IntPtr.Zero;
            MonoLibrary.Instance.mono_runtime_invoke(mlShared_OnApplicationStart, IntPtr.Zero, (void**)null, ref exc);
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_init_version(IntPtr name, IntPtr version)
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
            string methodName = MonoLibrary.Instance.mono_method_get_name(method).ToAnsiString();

            // Check for Trigger Method
            foreach (string triggerMethod in RuntimeInfo.TriggerMethods)
            {
                if (!methodName.Contains(triggerMethod))
                    continue;

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
