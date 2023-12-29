using System.IO;
using MelonLoader.Interfaces;
using MelonLoader.Unity.Utils;
using MelonLoader.Utils;

namespace MelonLoader.Unity.EngineModule;

public class EngineModule : IEngineModule
{
    private static readonly string GameDataPath = $"{Path.Combine(MelonEnvironment.GameRootDirectory, MelonEnvironment.GameExecutableName)}_Data";
    
    public EngineModuleInfo GameInfo { get; private set; }
    
    public void Initialize()
    {
        MelonDebug.Msg("Unity.EngineModule.Initialize");
        UnityEnvironment.Initialize(GameDataPath);
        
        GameInfo = new EngineModuleInfo
        {
            EngineName = "Unity",
            RuntimeName = "Mono", //TODO: Implement this properly
            GameName = UnityEnvironment.GameName,
            EngineVersion = UnityEnvironment.EngineVersion.ToString(),
            GameVersion = UnityEnvironment.GameVersion,
            GameDeveloper = UnityEnvironment.GameDeveloper
        };
    }
}