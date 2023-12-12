using System.IO;
using System.Linq;
using MelonLoader.CoreCLR.Bootstrap;
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
            if (GodotEnvironment.EngineVersion.Major >= 4)
            {
                string hostFxrPath = FindHostFxr();
                if (string.IsNullOrEmpty(hostFxrPath))
                {
                    MelonAssertion.ThrowInternalFailure("Failed to find HostFxr Library!");
                    return;
                }

                DotnetLoader.Startup(new DotnetRuntimeInfo(hostFxrPath));
                
                MelonDebug.Msg($"HostFxr Path: {hostFxrPath}");
                MelonDebug.Msg($"Using .NET {DotnetLoader.RuntimeInfo.RuntimeVersion}");
                return;
            }

            if (GodotEnvironment.EngineVersion.Major == 3)
            {
                //TODO: Implement mono
            }
        }
        
        private string[] HostFxrPaths = new string[]
        {
            "hostfxr.dll",
            "libhostfxr.so",
            "libhostfxr.dylib"
        };
        private string FindHostFxr()
        {
            return HostFxrPaths.Select(path => Path.Combine(GodotEnvironment.GameDataPath, path)).FirstOrDefault(File.Exists);
        }
    }   
}