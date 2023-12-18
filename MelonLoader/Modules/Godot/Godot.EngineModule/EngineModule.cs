using System.IO;
using MelonLoader.Fixes;
using MelonLoader.Godot.Utils;
using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Godot.EngineModule;

public class EngineModule : IEngineModule
{
    public string EngineName => "Godot";

    public void Initialize()
    {
        string pckPath = Path.Combine(MelonEnvironment.MelonBaseDirectory,
            $"{MelonEnvironment.GameExecutableName}.pck");
        
        GodotEnvironment.Initialize(pckPath);
        
        MelonDebug.Msg($"Engine Version: {GodotEnvironment.EngineVersion.ToString()}");
        
    }
}