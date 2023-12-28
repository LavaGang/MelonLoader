namespace MelonLoader.Interfaces
{
    public interface IEngineModule
    {
        string EngineName { get; }
        string RuntimeName { get; }

        void Initialize();
    }   
}