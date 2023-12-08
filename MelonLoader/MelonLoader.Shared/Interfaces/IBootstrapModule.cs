namespace MelonLoader.Interfaces
{
    public interface IBootstrapModule
    {
        string EngineName { get; }
        bool IsMyEngine { get; }

        void Startup();
    }   
}