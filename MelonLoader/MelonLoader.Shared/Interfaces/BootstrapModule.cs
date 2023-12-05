namespace MelonLoader.Interfaces
{
    public interface BootstrapModule
    {
        string EngineName { get; }
        bool IsMyEngine { get; }

        void Startup();
    }   
}