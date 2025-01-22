namespace MelonLoader.Modules;

internal interface ISupportModuleFrom
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