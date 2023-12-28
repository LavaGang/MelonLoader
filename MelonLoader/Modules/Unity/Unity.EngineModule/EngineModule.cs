using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Unity.EngineModule;

public class EngineModule : IEngineModule
{
    public string EngineName => "Unity";

    public string RuntimeName => "Mono"; //TODO i.e. Mono/MonoBleedingEdge/IL2CPP

    public void Initialize()
    {
        MelonDebug.Msg("Unity.EngineModule.Initialize");
        
    }
}