using System.IO;
using MelonLoader.Mono;
using MelonLoader.Shared.Interfaces;
using MelonLoader.Shared.Utils;
using MelonLoader.Unity.Il2Cpp;

namespace MelonLoader.Unity
{
    public class Bootstrap : BootstrapModule
    {
        public string EngineName => "Unity";

        /// <summary>
        /// TODO: Implement this properly, read the assets/files and see if we can determine engine version, etc.
        /// </summary>
        public bool IsMyEngine
        {
            get
            {
                if (!Directory.Exists(GameDataPath))
                    return false;

                return File.Exists(Path.Combine(GameDataPath, "globalgamemanagers"))
                    || File.Exists(Path.Combine(GameDataPath, "data.unity3d"))
                    || File.Exists(Path.Combine(GameDataPath, "mainData"));
            }
        }

        internal static string GameDataPath { get; private set; } = $"{Path.Combine(MelonEnvironment.GameRootDirectory, MelonEnvironment.GameExecutableName)}_Data";

        public void Startup()
        {
            // Get GameAssembly Name
            string gameAssemblyName = "GameAssembly";
            if (MelonUtils.IsUnix)
                gameAssemblyName += ".so";
            if (MelonUtils.IsWindows)
                gameAssemblyName += ".dll";
            if (MelonUtils.IsMac)
                gameAssemblyName += ".dylib";

            // Check if GameAssembly exists
            string gameAssemblyPath = Path.Combine(MelonEnvironment.GameRootDirectory, gameAssemblyName);
            if (File.Exists(gameAssemblyPath))
            {
                // Start Il2Cpp Support
                MelonLogger.Msg("Engine Variant: Il2Cpp");
                Il2CppLoader.Startup(gameAssemblyPath); 
            }
            else
            { 
                // Start Mono Support
                MonoRuntimeInfo runtimeInfo = GetMonoRuntimeInfo();
                MelonLogger.Msg($"Engine Variant: {runtimeInfo.Variant}");
                MonoLoader.Startup(runtimeInfo);
            }
        }

        internal static MonoRuntimeInfo GetMonoRuntimeInfo()
        {
            // Folders the Mono folders might be located in
            string[] directoriesToSearch = new string[]
            {
                    MelonEnvironment.GameRootDirectory,
                    GameDataPath
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

            // Iterate through Variations in Mono types
            bool isOldMono = true;
            foreach (var variant in monoFolderVariants)
            {
                // Iterate through Variations in Mono Directory Positions
                foreach (var dir in directoriesToSearch)
                {
                    // Get Directory Path
                    string dirPath = Path.Combine(dir, variant, "EmbedRuntime");
                    if (!Directory.Exists(dirPath))
                        continue;

                    // Get All Containing Files in Directory
                    string[] foundFiles = Directory.GetFiles(dirPath);
                    if (foundFiles == null
                        || foundFiles.Length <= 0)
                        continue;

                    // Get Posix Helper Path
                    string posixPath = Path.Combine(dirPath, $"{monoPosixFileNameWithoutExt}{monoFileExt}");

                    // Get Config Directory Path
                    string configPath = Path.Combine(dir, variant, "etc");

                    // Iterate through all found Files in EmbedRuntime
                    foreach (var filePath in foundFiles)
                    {
                        // Check if its a Runtime library
                        string fileName = Path.GetFileName(filePath);
                        if (!fileName.Equals($"{monoFileNameWithoutExt}{monoFileExt}")
                            && !fileName.StartsWith($"{monoFileNameWithoutExt}-") && fileName.EndsWith(monoFileExt))
                            continue;

                        // Return Information
                        return new Mono.MonoRuntimeInfo(
                            filePath,
                            posixPath, 
                            configPath, 
                            variant,
                            isOldMono 
                        );
                    }
                }

                // Flip this since Index of 1 is MonoBleedingEdge
                isOldMono = false;
            }

            // Return Nothing
            return null;
        }
    }   
}