using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Unity.EngineModule;

public class EngineModule : IEngineModule
{
    public string EngineName => "Unity";

    public void Initialize()
    {
        MelonDebug.Msg("Unity.EngineModule.Initialize");
        
    }
}