using System.IO;
using MelonLoader.Godot.Utils;
using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Godot.Bootstrap
{
    public class Bootstrap : IBootstrapModule
    {
        public string EngineName => "Godot";

        /// <summary>
        /// TODO: Implement this properly, read the PCK file and see if we can determine engine version, etc.
        /// </summary>
        public bool IsMyEngine
        {
            get
            {
                string pckPath = Path.Combine(MelonEnvironment.MelonBaseDirectory,
                    $"{MelonEnvironment.GameExecutableName}.pck");
                
                if (!File.Exists(pckPath))
                    return false;
                
                GodotEnvironment.Initialize(pckPath);
                
                MelonDebug.Msg($"Engine Version: {GodotEnvironment.EngineVersion.ToString()}");
                
                return true;
            }
            
        }

        public void Startup()
        {

        }
    }   
}