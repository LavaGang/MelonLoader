using System.IO;
using MelonLoader.Shared.Interfaces;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Godot
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
    }   
}