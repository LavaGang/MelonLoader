using System.IO;
using MelonLoader.Shared.Interfaces;
using MelonLoader.Shared.Utils;

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
                Il2Cpp.Startup(gameAssemblyPath);
            }
            else
            {
                // Start Mono Support
                Mono.Startup();
            }
        }
    }   
}