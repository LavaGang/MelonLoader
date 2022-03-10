namespace MelonLoader
{
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
        void BONEWORKS_OnLoadingScreen();
        void SetUnhollowerSupportInterface(UnhollowerSupport.Interface inter);
    }
}