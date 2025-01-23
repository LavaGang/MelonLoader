using System.Diagnostics.CodeAnalysis;

namespace MelonLoader;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "It's public API")]
public interface ISupportModule_From
{
    void OnApplicationLateStart();
    void OnSceneWasLoaded(int buildIndex, string sceneName);
    void OnSceneWasInitialized(int buildIndex, string sceneName);
    void OnSceneWasUnloaded(int buildIndex, string sceneName);
    void Update();
    void FixedUpdate();
    void LateUpdate();
    void OnGUI();
    void Quit();
    void DefiniteQuit();
    void SetInteropSupportInterface(InteropSupport.Interface interop);
}