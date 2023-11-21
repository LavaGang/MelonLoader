using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Unity
{
    internal unsafe static class Mono
    {
        private static RuntimeInfo runtimeInfo;
        private static MonoNativeLibrary monoNativeLibrary;
        private static void* monoDomain;

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

            // Check if there is a Posix Helper included with the Mono variant library
            if (!string.IsNullOrEmpty(runtimeInfo.PosixPath))
            {
                // Force-Load the Posix Helper before the actual Mono variant library
                // Otherwise can cause crashing in some cases
                if (!NativeLibrary.TryLoad(runtimeInfo.PosixPath, out IntPtr monoPosixHandle))
                {
                    Assertion.ThrowInternalFailure($"Failed to load {runtimeInfo.FolderVariant} Posix Helper from {runtimeInfo.PosixPath}!");
                    return;
                }
            }

            // Load the Mono variant library
            monoNativeLibrary = MelonNativeLibrary.ReflectiveLoad<MonoNativeLibrary>(runtimeInfo.FilePath);
            if (monoNativeLibrary == null)
            {
                Assertion.ThrowInternalFailure($"Failed to load {runtimeInfo.FolderVariant} Library from {runtimeInfo.FilePath}!");
                return;
            }

            // Validate Export References
            string failedExport = monoNativeLibrary.Validation();
            if (!string.IsNullOrEmpty(failedExport))
            {
                Assertion.ThrowInternalFailure($"Failed to load Export {failedExport} from {runtimeInfo.FolderVariant} Library!");
                return;
            }

            // Hook mono_jit_init_version
            BootstrapInterop.HookAttach(ref monoNativeLibrary.mono_jit_init_version, mono_jit_init_version);
        }

        private static void* mono_jit_init_version(void* name, void* version)
        {
            // Detach Hook as its no longer needed
            BootstrapInterop.HookDetach(monoNativeLibrary.mono_jit_init_version);

            // Check if in Debug Mode
            if (MelonDebug.IsEnabled())
            {
                // TO-DO: Parse DNSPY Environment Options
                // Mono = DNSPY_UNITY_DBG
                // MonoBleedingEdge = DNSPY_UNITY_DBG2
            }

            // Call original and get Mono Domain
            monoDomain = monoNativeLibrary.mono_jit_init_version(name, version);

            // Check if in Debug Mode and Debug Domain needs to be Initialized
            if (MelonDebug.IsEnabled())
            {

            }

            // Set Mono Main Thread to Current Thread

            // Check if MonoBleedingEdge
            if (!runtimeInfo.IsOldMono)
            {
                // TO-DO: Set Domain Config
            }

            // Load Assemblies

            // Hook mono_runtime_invoke
            BootstrapInterop.HookAttach(ref monoNativeLibrary.mono_runtime_invoke, mono_runtime_invoke);

            // Return Mono Domain
            return monoDomain;
        }

        private static void* mono_runtime_invoke(void* method, void* obj, void** prams, void** exec)
        {
            // Check Method Name
            string methodName = Marshal.PtrToStringAnsi(new IntPtr(method));
            if (!string.IsNullOrEmpty(methodName)
                && IsTargetInvokedMethod(methodName))
            {
                // Application has officially Started
                // Run Melons

                // Detach Hook as its no longer needed
                BootstrapInterop.HookDetach(monoNativeLibrary.mono_runtime_invoke);
            }

            return monoNativeLibrary.mono_runtime_invoke(method, obj, prams, exec);
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
            internal delegate void* d_mono_jit_init_version(void* name, void* version);
            internal d_mono_jit_init_version mono_jit_init_version;

            internal delegate void* d_mono_runtime_invoke(void* method, void* obj, void** prams, void** exec);
            internal d_mono_runtime_invoke mono_runtime_invoke;

            internal string Validation()
            {
                (string, Delegate)[] ValidationList = new (string, Delegate)[]
                {
                    (nameof(mono_jit_init_version), mono_jit_init_version),
                    (nameof(mono_runtime_invoke), mono_runtime_invoke)
                };

                foreach (var obj in ValidationList)
                    if (obj.Item2 == null)
                        return obj.Item1;

                return null;
            }
        }

        private class RuntimeInfo
        {
            internal readonly string FilePath;
            internal readonly string PosixPath;
            internal readonly string FolderVariant;
            internal readonly bool IsOldMono;

            private RuntimeInfo(string filePath, string posixPath, string folderVariant, bool isOldMono)
            {
                FilePath = filePath;
                PosixPath = posixPath;
                FolderVariant = folderVariant;
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
                                return new RuntimeInfo(filePath, posixPath, variant, isOldMono);
                        }
                    }

                    isOldMono = false;
                }

                return null;
            }
        }
    }
}
