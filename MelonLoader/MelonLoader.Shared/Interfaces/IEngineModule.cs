namespace MelonLoader.Interfaces
{
    public interface IEngineModule
    {
        string EngineName { get; }
        string RuntimeName { get; }
        string GameName { get; }
        string EngineVersion { get; }

        void Initialize();
    }   
}