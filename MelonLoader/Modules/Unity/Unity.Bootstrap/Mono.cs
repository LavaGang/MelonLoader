using System;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Unity
{
    internal static class Mono
    {
        internal static unsafe void Startup()
        {
            // Scan the Game for Information about the Mono Runtime
            RuntimeInfo runtimeInfo = RuntimeInfo.Get();

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
            if (!NativeLibrary.TryLoad(runtimeInfo.FilePath, out IntPtr monoHandle))
            {
                Assertion.ThrowInternalFailure($"Failed to load {runtimeInfo.FolderVariant} Library from {runtimeInfo.FilePath}!");
                return;
            }

            // Get mono_jit_init_version Export
            if (!NativeLibrary.TryGetExport(monoHandle, "mono_jit_init_version", out IntPtr initPtr))
            {
                Assertion.ThrowInternalFailure($"Failed to get mono_jit_init_version Export from {runtimeInfo.FolderVariant} Library!");
                return;
            }

            // Get mono_runtime_invoke Export
            if (!NativeLibrary.TryGetExport(monoHandle, "mono_runtime_invoke", out IntPtr runtimeInvokePtr))
            {
                Assertion.ThrowInternalFailure($"Failed to get mono_runtime_invoke Export from {runtimeInfo.FolderVariant} Library!");
                return;
            }

            // Hook mono_jit_init_version
            BootstrapInterop.HookAttach(initPtr, ((d_mono_jit_init_version)mono_jit_init_version).GetFunctionPointer());

            // Hook mono_runtime_invoke
            BootstrapInterop.HookAttach(runtimeInvokePtr, ((d_mono_runtime_invoke)mono_runtime_invoke).GetFunctionPointer());
        }

        private unsafe delegate void* d_mono_jit_init_version(void* name, void* version);
        private static unsafe void* mono_jit_init_version(void* name, void* version)
        {
            return (void*)0;
        }

        private unsafe delegate void* d_mono_runtime_invoke(void* method, void* obj, void** prams, void** exec);
        private static unsafe void* mono_runtime_invoke(void* method, void* obj, void** prams, void** exec)
        {
            return (void*)0;
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
