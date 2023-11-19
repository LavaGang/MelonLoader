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
                string appData = $"{Path.Combine(MelonEnvironment.GameRootDirectory, MelonEnvironment.GameExecutableName)}_Data";
                if (!Directory.Exists(appData))
                    return false;

                return File.Exists(Path.Combine(appData, "globalgamemanagers"))
                    || File.Exists(Path.Combine(appData, "data.unity3d"))
                    || File.Exists(Path.Combine(appData, "mainData"));
            }
        }

        public void Startup()
        {
            
        }
    }   
}