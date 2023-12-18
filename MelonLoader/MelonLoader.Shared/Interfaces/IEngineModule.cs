namespace MelonLoader.Interfaces
{
    public interface IEngineModule
    {
        string EngineName { get; }

        void Initialize();
    }   
}