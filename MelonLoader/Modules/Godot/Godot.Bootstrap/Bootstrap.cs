using System.IO;
using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Godot.Bootstrap
{
    public class Bootstrap : BootstrapModule
    {
        public string EngineName => "Godot";
    
        /// <summary>
        /// TODO: Implement this properly, read the PCK file and see if we can determine engine version, etc.
        /// </summary>
        public bool IsMyEngine =>
            File.Exists(Path.Combine(MelonEnvironment.MelonBaseDirectory,
                $"{MelonEnvironment.GameExecutableName}.pck"));

        public void Startup()
        {

        }
    }   
}