using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.InternalUtils;
using MelonLoader.Modules;
using MelonLoader.NativeUtils;
using MelonLoader.Utils;
#pragma warning disable 0649

namespace MelonLoader.Runtime.Mono
{
    public unsafe static class MonoLoader
    {
        #region Private Members

        private static MelonNativeDetour<MonoLibrary.d_mono_init_version> mono_init_version_detour;
        private static MelonNativeDetour<MonoLibrary.d_mono_runtime_invoke> mono_runtime_invoke_detour;

        private static IntPtr monoDomain;

        private static IntPtr mlAsm;
        private static IntPtr mlAsmImage;

        private static IntPtr mlMonoLibraryAsm;
        private static IntPtr mlMonoLibraryAsmImage;
        private static IntPtr mlMonoLibraryType;
        private static IntPtr mlMonoLibraryLoad;
        private static IntPtr mlMonoLibraryResolverType;
        private static IntPtr mlMonoLibraryResolverInit;

        private static IntPtr mlInteropType;
        private static IntPtr mlStage1;

        private static IntPtr mlCoreType;
        private static IntPtr mlStage2;
        private static IntPtr mlStage3;

        private static IntPtr mlEnvironmentType;
        private static IntPtr mlSetAppInfo;
        private static IntPtr mlSetEngineInfo2;
        private static IntPtr mlSetEngineInfo3;

        #endregion

        #region Public Members

        public static MelonEngineModule EngineModule { get; private set; }
        public static MonoRuntimeInfo RuntimeInfo { get; private set; }

        #endregion

        #region Loader

        public static void Initialize(MelonEngineModule engineModule,
            MonoRuntimeInfo runtimeInfo)
        {
            // Apply the information
            EngineModule = engineModule;
            RuntimeInfo = runtimeInfo;

            // Check if it found any Mono variant library
            if (RuntimeInfo == null)
            {
                MelonLogger.ThrowInternalFailure("Invalid Runtime Info Passed!");
                return;
            }

            // Load Posix Helper
            if (!LoadPosixHelper())
                return;

            // Load Library
            if (!LoadLibrary())
                return;

            // Check Exports
            if (!CheckExports())
                return;

            // Get Mono Domain
            monoDomain = MonoLibrary.Instance.mono_domain_get();

            // Handle loading if Domain is already Created
            if (monoDomain == IntPtr.Zero)
            {
                // Attach mono_init_version Detour
                MelonDebug.Msg($"Attaching mono_init_version Detour...");
                if (!RuntimeInfo.IsBleedingEdge)
                    mono_init_version_detour = new(MonoLibrary.Instance.mono_init_version, h_mono_init_version);
                else
                    mono_init_version_detour = new(MonoLibrary.Instance.mono_jit_init_version, h_mono_init_version);
                mono_init_version_detour.Attach();
            }
        }

        private static bool IsNetStandard()
        {
            // To-Do: Read from Mono Assembly
            return false;
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
            if (!MelonNativeLibrary.TryLoadLib(RuntimeInfo.PosixPath, out IntPtr _))
            {
                MelonLogger.ThrowInternalFailure($"Failed to load {(RuntimeInfo.IsBleedingEdge ? "MonoBleedingEdge" : "Mono")} Posix Helper from {RuntimeInfo.PosixPath}");
                return false;
            }

            // Return Success
            return true;
        }

        private static bool LoadLibrary()
        {
            // Check if there is a Posix Helper included with the Mono variant library
            if (string.IsNullOrEmpty(RuntimeInfo.LibraryPath))
                return true;

            // Load Library
            MonoLibrary.Load(RuntimeInfo.LibraryPath);
            if (MonoLibrary.Instance == null)
            {
                MelonLogger.ThrowInternalFailure($"Failed to load {(RuntimeInfo.IsBleedingEdge ? "MonoBleedingEdge" : "Mono")} from {RuntimeInfo.LibraryPath}");
                return false;
            }

            // Return Success
            return true;
        }

        private static bool CheckExports()
        {
            Dictionary<string, Delegate> listOfExports = new();

            string initName = !RuntimeInfo.IsBleedingEdge
                ? nameof(MonoLibrary.Instance.mono_init_version)
                : nameof(MonoLibrary.Instance.mono_jit_init_version);

            Delegate initDel = !RuntimeInfo.IsBleedingEdge
                ? MonoLibrary.Instance.mono_init_version
                : MonoLibrary.Instance.mono_jit_init_version;

            listOfExports[initName] = initDel;

            if (RuntimeInfo.IsBleedingEdge)
                listOfExports[nameof(MonoLibrary.Instance.mono_jit_init_version)] = MonoLibrary.Instance.mono_jit_init_version;
            else
                listOfExports[nameof(MonoLibrary.Instance.mono_init_version)] = MonoLibrary.Instance.mono_init_version;

            listOfExports[nameof(MonoLibrary.Instance.mono_domain_get)] = MonoLibrary.Instance.mono_domain_get;
            listOfExports[nameof(MonoLibrary.Instance.mono_domain_set)] = MonoLibrary.Instance.mono_domain_set;
            listOfExports[nameof(MonoLibrary.Instance.mono_domain_assembly_open)] = MonoLibrary.Instance.mono_domain_assembly_open;
            listOfExports[nameof(MonoLibrary.Instance.mono_assembly_get_image)] = MonoLibrary.Instance.mono_assembly_get_image;
            listOfExports[nameof(MonoLibrary.Instance.mono_class_from_name)] = MonoLibrary.Instance.mono_class_from_name;
            listOfExports[nameof(MonoLibrary.Instance.mono_method_get_name)] = MonoLibrary.Instance.mono_method_get_name;
            listOfExports[nameof(MonoLibrary.Instance.mono_runtime_invoke)] = MonoLibrary.Instance.mono_runtime_invoke;
            listOfExports[nameof(MonoLibrary.Instance.mono_thread_set_main)] = MonoLibrary.Instance.mono_thread_set_main;
            listOfExports[nameof(MonoLibrary.Instance.mono_string_new)] = MonoLibrary.Instance.mono_string_new;
            listOfExports[nameof(MonoLibrary.Instance.mono_class_get_method_from_name)] = MonoLibrary.Instance.mono_class_get_method_from_name;

            foreach (var exportPair in listOfExports)
            {
                if (exportPair.Value != null)
                    continue;

                MelonLogger.ThrowInternalFailure($"Failed to find {exportPair.Key} Export in Mono Library!");
                return false;
            }

            return true;
        }

        #endregion

        #region Hooks

        private static IntPtr h_mono_init_version(IntPtr name, IntPtr version)
        {
            // Invoke Domain Creation
            monoDomain = mono_init_version_detour.Trampoline(name, version);

            // Detach mono_init_version Detour
            mono_init_version_detour.Detach();

            if (LoaderConfig.Current.Loader.DebugMode && (MonoLibrary.Instance.mono_debug_domain_create != null))
            {
                MelonDebug.Msg("Creating Mono Debug Domain");   
                MonoLibrary.Instance.mono_debug_domain_create(monoDomain);
            }

            MelonDebug.Msg("Setting Mono Main Thread");
            MonoLibrary.Instance.mono_thread_set_main(MonoLibrary.Instance.mono_thread_current());
            MonoLibrary.Instance.mono_domain_set(monoDomain, 1);

            if (RuntimeInfo.IsBleedingEdge && (MonoLibrary.Instance.mono_domain_set_config != null))
            {
                MelonDebug.Msg("Setting Mono Config");
                MonoLibrary.Instance.mono_domain_set_config(monoDomain, MelonEnvironment.ApplicationRootDirectory, name);
            }

            Stage1and2();

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
                if (string.IsNullOrEmpty(triggerMethod)
                    || !methodName.Contains(triggerMethod))
                    continue;

                // Detach mono_runtime_invoke Detour
                mono_runtime_invoke_detour.Detach();

                Stage3();

                // Return original Invoke without Trampoline
                return MonoLibrary.Instance.mono_runtime_invoke(method, obj, param, ref exc);
            }

            // Return original Invoke
            return mono_runtime_invoke_detour.Trampoline(method, obj, param, ref exc);
        }

        #endregion

        #region MelonLoader Assembly Wrapping

        private static void Stage1and2()
        {
            if (!SetupAssembly())
                return;

            // Initiate Stage1
            MelonDebug.Msg("Initiating Stage1...");
            nint bootstrapHandle = BootstrapInterop._handle;
            nint loadLibFunc = BootstrapInterop.NativeLoadLibInteropPtr;
            nint getExportFunc = BootstrapInterop.NativeGetExportInteropPtr;
            var stage1Args = stackalloc nint*[]
            {
                &bootstrapHandle,
                &loadLibFunc,
                &getExportFunc
            };
            MonoLibrary.Instance.InvokeMethod(mlStage1, IntPtr.Zero, (void**)stage1Args);

            // Load MonoLibrary
            MelonDebug.Msg("Loading MonoLibrary in Mono Domain...");
            MonoLibrary.Instance.InvokeMethod(mlMonoLibraryLoad, IntPtr.Zero,
                MonoLibrary.Instance.mono_string_new(monoDomain, RuntimeInfo.LibraryPath.ToAnsiPointer()));

            // Patch Mono Assembly Resolver
            MelonDebug.Msg("Patching Mono Assembly Resolver...");
            MonoLibrary.Instance.InvokeMethod(mlMonoLibraryResolverInit, IntPtr.Zero,
                MonoLibrary.Instance.mono_string_new(monoDomain, RuntimeInfo.ManagedPath.ToAnsiPointer()));

            // Apply Engine Info
            MelonDebug.Msg("Applying Engine Information...");
            nint engineNameHandle = MelonEnvironment.CurrentEngineInfo.Name.ToAnsiPointer();
            nint engineVersionHandle = MelonEnvironment.CurrentEngineInfo.Version.ToAnsiPointer();
            if (string.IsNullOrEmpty(MelonEnvironment.CurrentEngineInfo.Variant))
            {
                MonoLibrary.Instance.InvokeMethod(mlSetEngineInfo2, IntPtr.Zero,
                    MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Name.ToAnsiPointer()),
                    MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Version.ToAnsiPointer()));
            }
            else
            {
                MonoLibrary.Instance.InvokeMethod(mlSetEngineInfo3, IntPtr.Zero,
                    MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Name.ToAnsiPointer()),
                    MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Version.ToAnsiPointer()),
                    MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Variant.ToAnsiPointer()));
            }

            // Apply Application Info
            MelonDebug.Msg("Applying Application Information...");
            MonoLibrary.Instance.InvokeMethod(mlSetAppInfo, IntPtr.Zero, 
                MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentApplicationInfo.Name.ToAnsiPointer()),
                MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentApplicationInfo.Developer.ToAnsiPointer()),
                MonoLibrary.Instance.mono_string_new(monoDomain, MelonEnvironment.CurrentEngineInfo.Version.ToAnsiPointer()));

            // Initiate Stage2
            MelonDebug.Msg("Initiating Stage2...");
            MonoLibrary.Instance.InvokeMethod(mlStage2, IntPtr.Zero);

            // Attach mono_runtime_invoke Detour
            MelonDebug.Msg("Attaching mono_runtime_invoke Detour...");
            mono_runtime_invoke_detour = new(MonoLibrary.Instance.mono_runtime_invoke, h_mono_runtime_invoke, false);
            mono_runtime_invoke_detour.Attach();
        }

        public static void Stage3()
        {
            // Initiate Stage3
            MelonDebug.Msg("Initiating Stage3...");
            MonoLibrary.Instance.InvokeMethod(mlStage3, IntPtr.Zero, 
                MonoLibrary.Instance.mono_string_new(monoDomain, RuntimeInfo.SupportModulePath.ToAnsiPointer()));
        }

        private static void LoadNetStandardPatches()
        {
            var corlibPath = Path.Combine(RuntimeInfo.ManagedPath, "mscorlib.dll");
            if (File.Exists(corlibPath))
            {
                var corlibVersion = FileVersionInfo.GetVersionInfo(corlibPath);
                if (corlibVersion.FileMajorPart <= 2)
                {
                    MelonDebug.Msg("Loading .NET Standard 2.0 overrides");
                    var overridesDir = Path.Combine(MelonEnvironment.DependenciesDirectory, "NetStandardPatches");
                    if (Directory.Exists(overridesDir))
                    {
                        foreach (var dll in Directory.EnumerateFiles(overridesDir, "*.dll"))
                        {
                            MelonDebug.Msg("Loading assembly: " + dll);
                            if (MonoLibrary.Instance.mono_domain_assembly_open(monoDomain, dll.ToAnsiPointer()) == IntPtr.Zero)
                                MelonDebug.Msg($"Assembly failed to load {dll}");
                        }
                    }
                }
            }
        }

        private static bool SetupAssembly()
        {
            LoadNetStandardPatches();

            // Get File Path for MelonLoader.dll
            string mainAssemblyLocation = typeof(MelonLogger).Assembly.Location;
            string fileName = Path.GetFileName(mainAssemblyLocation);
            string sharedPath = Path.Combine(
                MelonEnvironment.DependenciesDirectory,
                IsNetStandard() ? "netstandard2.1" : "net35",
                fileName);

            Type monoLibType = typeof(MonoLibrary);
            string monoLibPath = Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(monoLibType.Assembly.Location)),
                IsNetStandard() ? "netstandard2.1" : "net35",
                Path.GetFileName(monoLibType.Assembly.Location));

            // Load MelonLoader Assembly
            MelonDebug.Msg($"Loading Assembly {sharedPath}...");
            mlAsm = MonoLibrary.Instance.mono_domain_assembly_open(monoDomain, sharedPath.ToAnsiPointer());
            if (mlAsm == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to load Assembly from {sharedPath}!");
                return false;
            }

            // Get Assembly Image
            MelonDebug.Msg("Getting Assembly Image...");
            mlAsmImage = MonoLibrary.Instance.mono_assembly_get_image(mlAsm);
            if (mlAsmImage == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Assembly Image from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.BootstrapInterop
            Type bootstrapInteropType = typeof(BootstrapInterop);
            MelonDebug.Msg($"Getting Class {bootstrapInteropType.FullName}...");
            mlInteropType = MonoLibrary.Instance.mono_class_from_name(mlAsmImage, bootstrapInteropType.Namespace.ToAnsiPointer(), bootstrapInteropType.Name.ToAnsiPointer());
            if (mlInteropType == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Class {bootstrapInteropType.FullName} from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.BootstrapInterop::Stage1
            MelonDebug.Msg($"Getting Method Stage1 from {bootstrapInteropType.FullName}...");
            mlStage1 = MonoLibrary.Instance.mono_class_get_method_from_name(mlInteropType, "Stage1".ToAnsiPointer(), 3);
            if (mlStage1 == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {bootstrapInteropType.FullName}::Stage1 from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Core
            Type coreType = bootstrapInteropType.Assembly.GetType("MelonLoader.Core");
            MelonDebug.Msg($"Getting Class {coreType.FullName}...");
            mlCoreType = MonoLibrary.Instance.mono_class_from_name(mlAsmImage, coreType.Namespace.ToAnsiPointer(), coreType.Name.ToAnsiPointer());
            if (mlCoreType == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Class {coreType.FullName} from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Core::Stage2
            MelonDebug.Msg($"Getting Method Stage2 from {coreType.FullName}...");
            mlStage2 = MonoLibrary.Instance.mono_class_get_method_from_name(mlCoreType, "Stage2".ToAnsiPointer(), 0);
            if (mlStage2 == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {coreType.FullName}::Stage2 from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Core::Stage3
            MelonDebug.Msg($"Getting Method Stage3 from {coreType.FullName}...");
            mlStage3 = MonoLibrary.Instance.mono_class_get_method_from_name(mlCoreType, "Stage3".ToAnsiPointer(), 1);
            if (mlStage3 == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {coreType.FullName}::Stage3 from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Utils.MelonEnvironment
            Type environmentType = typeof(MelonEnvironment);
            MelonDebug.Msg($"Getting Class {environmentType.FullName}...");
            mlEnvironmentType = MonoLibrary.Instance.mono_class_from_name(mlAsmImage, environmentType.Namespace.ToAnsiPointer(), environmentType.Name.ToAnsiPointer());
            if (mlEnvironmentType == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Class {environmentType.FullName} from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Utils.MelonEnvironment::SetApplicationInfo
            MelonDebug.Msg($"Getting Method SetApplicationInfo from {environmentType.FullName}...");
            mlSetAppInfo = MonoLibrary.Instance.mono_class_get_method_from_name(mlEnvironmentType, "SetApplicationInfo".ToAnsiPointer(), 3);
            if (mlSetAppInfo == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {environmentType.FullName}::SetApplicationInfo from {sharedPath}!");
                return false;
            }

            // Get MelonLoader.Utils.MelonEnvironment::SetEngineInfo 2
            MelonDebug.Msg($"Getting Method SetEngineInfo 2 from {environmentType.FullName}...");
            mlSetEngineInfo2 = MonoLibrary.Instance.mono_class_get_method_from_name(mlEnvironmentType, "SetEngineInfo".ToAnsiPointer(), 2);
            if (mlSetEngineInfo2 == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {environmentType.FullName}::SetEngineInfo 2 from {sharedPath}!");
                return false;
            }
            
            // Get MelonLoader.Utils.MelonEnvironment::SetEngineInfo 3
            MelonDebug.Msg($"Getting Method SetEngineInfo 3 from {environmentType.FullName}...");
            mlSetEngineInfo3 = MonoLibrary.Instance.mono_class_get_method_from_name(mlEnvironmentType, "SetEngineInfo".ToAnsiPointer(), 3);
            if (mlSetEngineInfo3 == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {environmentType.FullName}::SetEngineInfo 3 from {sharedPath}!");
                return false;
            }

            // Load Mono.Shared Assembly
            MelonDebug.Msg($"Loading Assembly {monoLibPath}...");
            mlMonoLibraryAsm = MonoLibrary.Instance.mono_domain_assembly_open(monoDomain, monoLibPath.ToAnsiPointer());
            if (mlMonoLibraryAsm == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to load Assembly from {monoLibPath}!");
                return false;
            }

            // Get Assembly Image
            MelonDebug.Msg("Getting Assembly Image...");
            mlMonoLibraryAsmImage = MonoLibrary.Instance.mono_assembly_get_image(mlMonoLibraryAsm);
            if (mlMonoLibraryAsmImage == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Assembly Image from {monoLibPath}!");
                return false;
            }
            
            // Get MonoLibrary
            MelonDebug.Msg($"Getting Class {monoLibType.FullName}...");
            mlMonoLibraryType = MonoLibrary.Instance.mono_class_from_name(mlMonoLibraryAsmImage, monoLibType.Namespace.ToAnsiPointer(), monoLibType.Name.ToAnsiPointer());
            if (mlMonoLibraryType == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Class {monoLibType.FullName} from {monoLibPath}!");
                return false;
            }

            // Get MonoLibrary::Load
            MelonDebug.Msg($"Getting Method {nameof(MonoLibrary.Load)} from {monoLibType.FullName}...");
            mlMonoLibraryLoad = MonoLibrary.Instance.mono_class_get_method_from_name(mlMonoLibraryType, nameof(MonoLibrary.Load).ToAnsiPointer(), 1);
            if (mlMonoLibraryLoad == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method {monoLibType.FullName}::{nameof(MonoLibrary.Load)} from {monoLibPath}!");
                return false;
            }

            // Get MonoAssemblyResolver
            MelonDebug.Msg($"Getting Class MelonLoader.Runtime.Mono.MonoAssemblyResolver ...");
            mlMonoLibraryResolverType = MonoLibrary.Instance.mono_class_from_name(mlMonoLibraryAsmImage, "MelonLoader.Runtime.Mono".ToAnsiPointer(), "MonoAssemblyResolver".ToAnsiPointer());
            if (mlMonoLibraryResolverType == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Class MelonLoader.Runtime.Mono.MonoAssemblyResolver from {monoLibPath}!");
                return false;
            }

            // Get MonoLibrary::Initialize
            MelonDebug.Msg($"Getting Method Initialize from MelonLoader.Runtime.Mono.MonoAssemblyResolver...");
            mlMonoLibraryResolverInit = MonoLibrary.Instance.mono_class_get_method_from_name(mlMonoLibraryResolverType, "Initialize".ToAnsiPointer(), 1);
            if (mlMonoLibraryResolverInit == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure($"Failed to get Method MelonLoader.Runtime.Mono.MonoAssemblyResolver::Initialize from {monoLibPath}!");
                return false;
            }

            // Return Success
            return true;
        }

        #endregion
    }
}
