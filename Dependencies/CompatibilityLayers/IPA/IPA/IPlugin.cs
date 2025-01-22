#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IllusionPlugin;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void OnApplicationStart();
    void OnApplicationQuit();
    void OnLevelWasLoaded(int level);
    void OnLevelWasInitialized(int level);
    void OnUpdate();
    void OnFixedUpdate();
}
