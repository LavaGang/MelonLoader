namespace MelonLoader.Interfaces;

public struct EngineModuleInfo
{
    public string EngineName;
    public string RuntimeName;
    public string GameName;
    public string EngineVersion;
    public string GameDeveloper;
    public string GameVersion;
}

public interface IEngineModule
{
    public EngineModuleInfo GameInfo { get; }

    void Initialize();
}