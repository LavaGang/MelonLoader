using System.IO;
using MelonLoader.Interfaces;
using MelonLoader.Unity.Utils;
using MelonLoader.Utils;

namespace MelonLoader.Unity.EngineModule;

public class EngineModule : IEngineModule
{
    private static readonly string GameDataPath = $"{Path.Combine(MelonEnvironment.GameRootDirectory, MelonEnvironment.GameExecutableName)}_Data";
    public string EngineName => "Unity";
    public string GameName { get; private set;  }
    public string EngineVersion { get; private set;}

    public string RuntimeName => "Mono"; //TODO i.e. Mono/MonoBleedingEdge/IL2CPP

    public void Initialize()
    {
        MelonDebug.Msg("Unity.EngineModule.Initialize");
        UnityEnvironment.Initialize(GameDataPath);
        
        GameName = UnityEnvironment.GameName;
        EngineVersion = UnityEnvironment.EngineVersionString;
    }
}