using System.IO;
using MelonLoader.Utils;
using MelonLoader.Modules;
using MelonLoader.Runtime.Il2Cpp;
using System;
using MelonLoader.Resolver;
using System.Runtime.Loader;
using MelonLoader.Engine.Unity.Il2Cpp;

namespace MelonLoader.Engine.Unity
{
    internal class UnityLoaderModule : MelonEngineModule
    {
        private static readonly string GameDataPath = Path.Combine(MelonEnvironment.ApplicationRootDirectory, $"{MelonEnvironment.ApplicationExecutableName}_Data");
        private static string LoaderPath;
        private static string UnityPlayerPath;
        private static string GameAssemblyPath;
        private static string SupportModulePath;
        private static bool IsIl2Cpp;

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

            string indentifier = IsIl2Cpp ? "Il2Cpp" : "Mono";
            SupportModulePath = Path.Combine(
                LoaderPath,
                IsIl2Cpp ? "net6" : "net35",
                $"MelonLoader.Unity.{indentifier}.dll");

            SetEngineInfo("Unity", UnityInformationHandler.EngineVersion.ToStringWithoutType(), indentifier);
            SetApplicationInfo(UnityInformationHandler.GameDeveloper, UnityInformationHandler.GameName, UnityInformationHandler.GameVersion);
            PrintAppInfo();

            if (IsIl2Cpp)
            {
                // Run Stage2
                Stage2();

                // Initialize Il2Cpp Loader
                Il2CppLoader.Initialize(this, new(GameAssemblyPath, SupportModulePath,
                    [
                        "Internal_ActiveSceneChanged",
                        "UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize"
                    ]));
            }
            else
            {
                MelonLogger.Error("UNITY MONO SUPPORT NOT IMPLEMENTED!");
                Stage2();
                Stage3(null);
            }
        }
    }   
}