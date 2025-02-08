using System.IO;
using MelonLoader.Utils;
using MelonLoader.Modules;
using MelonLoader.Runtime.Il2Cpp;
using System;
using MelonLoader.Resolver;
using System.Runtime.Loader;
using MelonLoader.Engine.Unity.Il2Cpp;
using MelonLoader.Runtime.Mono;
using System.Collections.Generic;

namespace MelonLoader.Engine.Unity
{
    internal class UnityLoaderModule : MelonEngineModule
    {
        private readonly string GameDataPath = Path.Combine(MelonEnvironment.ApplicationRootDirectory, $"{MelonEnvironment.ApplicationExecutableName}_Data");
        private string UnityPlayerPath;
        private string GameAssemblyPath;
        private bool IsIl2Cpp;

        private string LoaderPath;
        private string SupportModulePath;

        private MonoRuntimeInfo MonoInfo;

        private static readonly string[] monoFolderNames =
        [
            "Mono",
            "MonoBleedingEdge"
        ];

        private static readonly string[] monoLibNames =
        [
            "mono",
#if WINDOWS
            "mono-2.0-bdwgc",
            "mono-2.0-sgen",
            "mono-2.0-boehm"
#elif LINUX
            "monobdwgc-2.0"
#endif
        ];

        private static readonly string[] monoPosixHelperNames =
        [
            "MonoPosixHelper",
        ];

        public override bool Validate()
        {
            UnityPlayerPath = Path.Combine(MelonEnvironment.ApplicationRootDirectory, $"UnityPlayer{OsUtils.NativeFileExtension}");
            if (File.Exists(UnityPlayerPath))
                return true;

            if (Directory.Exists(GameDataPath))
                return File.Exists(Path.Combine(GameDataPath, "globalgamemanagers"))
                    || File.Exists(Path.Combine(GameDataPath, "data.unity3d"))
                    || File.Exists(Path.Combine(GameDataPath, "mainData"));

            return false;
        }

        public override void Initialize()
        {
            MelonDebug.Msg("Initializing Unity Engine Module...");
            LoaderPath = Path.GetDirectoryName(typeof(UnityLoaderModule).Assembly.Location);

            UnityInformationHandler.Setup(GameDataPath, UnityPlayerPath);

            string gameAssemblyName = "GameAssembly";
            if (OsUtils.IsAndroid)
                gameAssemblyName = "libil2cpp";
            gameAssemblyName += OsUtils.NativeFileExtension;

            GameAssemblyPath = Path.Combine(MelonEnvironment.ApplicationRootDirectory, gameAssemblyName);
            IsIl2Cpp = File.Exists(GameAssemblyPath);
            if (!IsIl2Cpp)
            {
                // Android only has Il2Cpp currently
                if (OsUtils.IsAndroid)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to find {gameAssemblyName}!");
                    return;
                }

                // Attempt to find Library
                MonoInfo = GetMonoRuntimeInfo();
                if (MonoInfo == null)
                {
                    MelonLogger.ThrowInternalFailure("Failed to get Mono Runtime Info!");
                    return;
                }
            }

            string indentifier = IsIl2Cpp ? "Il2Cpp" : (MonoInfo.IsBleedingEdge ? "MonoBleedingEdge" : "Mono");
            SupportModulePath = Path.Combine(
                LoaderPath,
                IsIl2Cpp ? "net6" : "net35",
                $"MelonLoader.Unity.{(IsIl2Cpp ? "Il2Cpp" : "Mono")}.dll");

            SetEngineInfo("Unity", UnityInformationHandler.EngineVersion.ToStringWithoutType(), (IsIl2Cpp ? "Il2Cpp" : (MonoInfo.IsBleedingEdge ? "MonoBleedingEdge" : "Mono")));
            SetApplicationInfo(UnityInformationHandler.GameName, UnityInformationHandler.GameDeveloper, UnityInformationHandler.GameVersion);
            PrintAppInfo();

            if (IsIl2Cpp)
            {
                // Initialize Il2Cpp Loader
                Il2CppLoader.Initialize(this, new(GameAssemblyPath, SupportModulePath,
                    [
                        "Internal_ActiveSceneChanged",
                        "UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize"
                    ]));
            }
            else
            {
                // Android only has Il2Cpp currently
                if (OsUtils.IsAndroid)
                    return;

                // Initialize Mono Loader
                MonoInfo.SupportModulePath = SupportModulePath;
                MonoLoader.Initialize(this, MonoInfo);
            }
        }

        public override void Stage3(string supportModulePath)
        {
            if (!IsIl2Cpp)
            {
                // Run Stage3
                base.Stage3(supportModulePath);
                return;
            }

            string genBasePath = Path.Combine(LoaderPath, "Il2CppAssemblyGenerator");
            if (!Directory.Exists(genBasePath))
                Directory.CreateDirectory(genBasePath);

            string genOutputPath = Path.Combine(MelonEnvironment.DependenciesDirectory, "Il2CppAssemblies");
            if (!Directory.Exists(genOutputPath))
                Directory.CreateDirectory(genOutputPath);
            MelonAssemblyResolver.AddSearchDirectory(genOutputPath);

            // Apply Il2Cpp Fixes
            Il2CppInteropFixes.Install(genOutputPath);
            Il2CppICallInjector.Install();

            // Generate Il2Cpp Wrapper Assemblies
            try
            {
                if (!AssemblyGenerator.Run(genBasePath, GameAssemblyPath, genOutputPath))
                {
                    MelonDebug.Error("Il2Cpp Assembly Generation Failure!");
                    return;
                }

                foreach (var file in Directory.GetFiles(genOutputPath, "*.dll"))
                {
                    try
                    {
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MelonDebug.Error(ex.ToString());
                return;
            }

            // Run Stage3 after Assembly Generation
            base.Stage3(supportModulePath);
        }

        private MonoRuntimeInfo GetMonoRuntimeInfo()
        {
            // Folders the Mono folders might be located in
            string[] directoriesToSearch =
            [
                MelonEnvironment.ApplicationRootDirectory,
                GameDataPath
            ];

            // Iterate through Variations in Mono types
            (string, string, bool)? libPaths = null;
            foreach (var folderName in monoFolderNames)
            {
                // Iterate through Variations in Mono Directory Positions
                foreach (var dir in directoriesToSearch)
                {
                    // Iterate through Variations in Mono Lib Names
                    string folderPath = Path.Combine(dir, folderName);
                    foreach (var fileName in monoLibNames)
                    {
                        libPaths = SearchFolderForMonoLib(folderPath, fileName);
                        if (libPaths != null)
                            break;
                    }
                    if (libPaths != null)
                        break;
                }
                if (libPaths != null)
                    break;
            }
            if (libPaths == null)
                return null;

            // Attempt to find Posix Helper
            string posixPath = null;
            foreach (string fileName in monoPosixHelperNames)
            {
                string localFileName = fileName;
                if (OsUtils.IsUnix)
                    localFileName = $"lib{fileName}";
                localFileName += OsUtils.NativeFileExtension;
                string localFilePath = Path.Combine(libPaths.Value.Item1, localFileName);
                if (File.Exists(localFilePath))
                {
                    posixPath = localFilePath;
                    break;
                }
            }

            bool isBleedingEdge = libPaths.Value.Item3;
            return new MonoRuntimeInfo(libPaths.Value.Item2, posixPath, Path.Combine(GameDataPath, "Managed"), isBleedingEdge, null, [
                (!isBleedingEdge ? "Awake" : string.Empty),
                (!isBleedingEdge ? "DoSendMouseEvents" : string.Empty),
                "Internal_ActiveSceneChanged",
                "UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize",
            ]);
        }

        private (string, string, bool)? SearchFolderForMonoLib(string folderPath, string fileName)
        {
            bool isBleedingEdge = (fileName != monoLibNames[0]);

            if (OsUtils.IsUnix)
                fileName = $"lib{fileName}";
            fileName += OsUtils.NativeFileExtension;

            string filePath = Path.Combine(folderPath, fileName);
            if (File.Exists(filePath))
                return (folderPath, filePath, isBleedingEdge);

            string embedRuntimePath = Path.Combine(folderPath, "EmbedRuntime");
            if (Directory.Exists(embedRuntimePath))
            {
                filePath = Path.Combine(embedRuntimePath, fileName);
                if (File.Exists(filePath))
                    return (embedRuntimePath, filePath, isBleedingEdge);

                string x64Path = Path.Combine(embedRuntimePath, "x86_64");
                if (Directory.Exists(x64Path))
                {
                    filePath = Path.Combine(x64Path, fileName);
                    if (File.Exists(filePath))
                        return (x64Path, filePath, isBleedingEdge);
                }
            }

            return null;
        }
    }   
}