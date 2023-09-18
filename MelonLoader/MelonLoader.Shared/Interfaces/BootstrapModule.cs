namespace MelonLoader.Shared.Interfaces
{
    public interface BootstrapModule
    {
        string EngineName { get; }
        bool IsMyEngine { get; }
    }   
}