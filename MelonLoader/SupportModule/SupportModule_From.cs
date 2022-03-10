namespace MelonLoader
{
    internal class SupportModule_From : ISupportModule_From
    {
        public void OnApplicationLateStart() => Core.OnApplicationLateStart();
        public void OnSceneWasLoaded(int buildIndex, string sceneName) => MelonHandler.OnSceneWasLoaded(buildIndex, sceneName);
        public void OnSceneWasInitialized(int buildIndex, string sceneName) => MelonHandler.OnSceneWasInitialized(buildIndex, sceneName);
        public void OnSceneWasUnloaded(int buildIndex, string sceneName) => MelonHandler.OnSceneWasUnloaded(buildIndex, sceneName);
        public void Update() => MelonHandler.OnUpdate();
        public void FixedUpdate() => MelonHandler.OnFixedUpdate();
        public void LateUpdate() => MelonHandler.OnLateUpdate();
        public void OnGUI() => MelonHandler.OnGUI();
        public void Quit() => Core.Quit();
        public void BONEWORKS_OnLoadingScreen() => MelonHandler.BONEWORKS_OnLoadingScreen();
        public void SetUnhollowerSupportInterface(UnhollowerSupport.Interface inter) { if (UnhollowerSupport.SMInterface == null) UnhollowerSupport.SMInterface = inter; }
    }
}