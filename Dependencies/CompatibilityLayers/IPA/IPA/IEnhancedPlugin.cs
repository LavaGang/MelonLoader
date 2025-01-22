#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IllusionPlugin;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public interface IEnhancedPlugin : IPlugin
{
    string[] Filter { get; }
    void OnLateUpdate();
}
