using System;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap;
using MelonLoader.Shared.Utils;
#pragma warning disable CS0649 //Field is never assigned

namespace MelonLoader.Unity
{
    internal unsafe static class Mono
    {
        private static RuntimeInfo runtimeInfo;
        private static MonoNativeLibrary lib;
        private static void* domain;

        internal static void Startup()
        {
            // Scan the Game for Information about the Mono Runtime
            runtimeInfo = RuntimeInfo.Get();

            // Check if it found any Mono variant library
            if ((runtimeInfo == null)
                || string.IsNullOrEmpty(runtimeInfo.FilePath))
            {
                Assertion.ThrowInternalFailure($"Failed to find Mono or MonoBleedingEdge Library!");
                return;
            }

            MelonLogger.Msg($"Engine Variant: {runtimeInfo.Variant}");

            // Check if there is a Posix Helper included with the Mono variant library
            if (!string.IsNullOrEmpty(runtimeInfo.PosixPath))
            {
                // Force-Load the Posix Helper before the actual Mono variant library
                // Otherwise can cause crashing in some cases
                if (!NativeLibrary.TryLoad(runtimeInfo.PosixPath, out IntPtr monoPosixHandle))
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

            // Hook mono_jit_init_version
            BootstrapInterop.HookAttach(ref lib.mono_jit_init_version, mono_jit_init_version);
        }

        private static void* mono_jit_init_version(void* name, void* version)
        {
            // Detach Hook as its no longer needed
            BootstrapInterop.HookDetach(lib.mono_jit_init_version);

            // TO-DO: Parse DNSPY Environment Options
            // Mono = DNSPY_UNITY_DBG
            // MonoBleedingEdge = DNSPY_UNITY_DBG2

            // Call original and get Mono Domain
            domain = lib.mono_jit_init_version(name, version);

            // Initialize Debug Domain
            if (lib.mono_debug_domain_create != null)
                lib.mono_debug_domain_create(domain);

            // Set Mono Main Thread to Current Thread
            lib.mono_thread_set_main(lib.mono_thread_current());

            // Set Config for MonoBleedingEdge
            if (!runtimeInfo.IsOldMono)
                lib.mono_domain_set_config(domain, Marshal.StringToHGlobalAnsi(runtimeInfo.ConfigPath).ToPointer(), name);

            // Run Application Pre-Start
            Shared.Core.OnAppPreStart();

            // Hook mono_runtime_invoke
            BootstrapInterop.HookAttach(ref lib.mono_runtime_invoke, mono_runtime_invoke);

            // Return Mono Domain
            return domain;
        }

        private static void* mono_runtime_invoke(void* method, void* obj, void** prams, void** exec)
        {
            // Check Method Name
            string methodName = Marshal.PtrToStringAnsi(new IntPtr(method));
            if (!string.IsNullOrEmpty(methodName)
                && IsTargetInvokedMethod(methodName))
            {
                // Detach Hook as its no longer needed
                BootstrapInterop.HookDetach(lib.mono_runtime_invoke);

                // Run Application Start
                Shared.Core.OnAppStart();
            }

            return lib.mono_runtime_invoke(method, obj, prams, exec);
        }

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
            if (runtimeInfo.IsOldMono)
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

        private class MonoNativeLibrary
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void* d_mono_jit_init_version(void* name, void* version);
            internal d_mono_jit_init_version mono_jit_init_version;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void* d_mono_runtime_invoke(void* method, void* obj, void** prams, void** exec);
            internal d_mono_runtime_invoke mono_runtime_invoke;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void* d_mono_thread_current();
            internal d_mono_thread_current mono_thread_current;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void d_mono_thread_set_main(void* thread);
            internal d_mono_thread_set_main mono_thread_set_main;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void d_mono_domain_set_config(void* domain, void* configPath, void* name);
            internal d_mono_domain_set_config mono_domain_set_config;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void d_mono_debug_domain_create(void* domain);
            internal d_mono_debug_domain_create mono_debug_domain_create;

            internal string Validation()
            {
                (string, Delegate)[] majorList = new (string, Delegate)[]
                {
                    (nameof(mono_jit_init_version), mono_jit_init_version),
                    (nameof(mono_runtime_invoke), mono_runtime_invoke),
                    (nameof(mono_thread_current), mono_thread_current),
                    (nameof(mono_thread_set_main), mono_thread_set_main),
                    (nameof(mono_domain_set_config), mono_domain_set_config)
                };

                foreach (var obj in majorList)
                    if (obj.Item2 == null)
                        return obj.Item1;

                return null;
            }
        }

        private class RuntimeInfo
        {
            internal readonly string FilePath;
            internal readonly string PosixPath;
            internal readonly string ConfigPath;
            internal readonly string Variant;
            internal readonly bool IsOldMono;

            private RuntimeInfo(string filePath, string posixPath, string configPath, string variant, bool isOldMono)
            {
                FilePath = filePath;
                PosixPath = posixPath;
                ConfigPath = configPath;
                Variant = variant;
                IsOldMono = isOldMono;
            }

            internal static RuntimeInfo Get()
            {
                // Folders the Mono folders might be located in
                string[] directoriesToSearch = new string[]
                {
                    MelonEnvironment.GameRootDirectory,
                    Bootstrap.GameDataPath
                };

                // Variants of Mono folders
                string[] monoFolderVariants = new string[]
                {
                    "Mono",
                    "MonoBleedingEdge"
                };

                // Get Mono variant library file name
                string monoFileNameWithoutExt = "mono";
                if (MelonUtils.IsUnix || MelonUtils.IsMac)
                    monoFileNameWithoutExt = $"lib{monoFileNameWithoutExt}";

                // Get Mono Posix Helper file name
                string monoPosixFileNameWithoutExt = "MonoPosixHelper";
                if (MelonUtils.IsUnix || MelonUtils.IsMac)
                    monoPosixFileNameWithoutExt = "libmonoposixhelper";

                // Get Platform Used Extension
                string monoFileExt = ".dll";
                if (MelonUtils.IsUnix)
                    monoFileExt = ".so";
                if (MelonUtils.IsMac)
                    monoFileExt = ".dylib";

                bool isOldMono = true;
                foreach (var variant in monoFolderVariants)
                {
                    foreach (var dir in directoriesToSearch)
                    {
                        string dirPath = Path.Combine(Path.Combine(dir, variant), "EmbedRuntime");
                        if (!Directory.Exists(dirPath))
                            continue;

                        string[] foundFiles = Directory.GetFiles(dirPath);
                        if ((foundFiles == null)
                            || (foundFiles.Length <= 0))
                            continue;

                        string posixPath = Path.Combine(dirPath, $"{monoPosixFileNameWithoutExt}{monoFileExt}");
                        foreach (var filePath in foundFiles)
                        {
                            string fileName = Path.GetFileName(filePath);
                            if (fileName.Equals($"{monoFileNameWithoutExt}{monoFileExt}")
                                || (fileName.StartsWith($"{monoFileNameWithoutExt}-") && fileName.EndsWith(monoFileExt)))
                            {
                                string configPath = "";
                                return new RuntimeInfo(filePath, posixPath, configPath, variant, isOldMono);
                            }
                        }
                    }

                    isOldMono = false;
                }

                return null;
            }
        }
    }
}
