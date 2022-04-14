namespace MelonLoader
{
    internal class SupportModule_From : ISupportModule_From
    {
        private static bool initializeScene;
        private static bool sceneWasJustLoaded;
        private static int currentSceneBuildIndex = -1;
        private static string currentSceneName;

        public void OnApplicationLateStart() 
            => Core.OnApplicationLateStart();

        public void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!MelonUtils.IsBONEWORKS)
            {
                sceneWasJustLoaded = true;
                currentSceneBuildIndex = buildIndex;
                currentSceneName = sceneName;
            }

            MelonEvents.OnSceneWasLoaded.Invoke(buildIndex, sceneName);
        }

        public void OnSceneWasInitialized(int buildIndex, string sceneName) 
            => MelonEvents.OnSceneWasInitialized.Invoke(buildIndex, sceneName);

        public void OnSceneWasUnloaded(int buildIndex, string sceneName) 
            => MelonEvents.OnSceneWasUnloaded.Invoke(buildIndex, sceneName);

        public void Update()
        {
            if (initializeScene)
            {
                initializeScene = false;
                OnSceneWasInitialized(currentSceneBuildIndex, currentSceneName);
            }
            if (sceneWasJustLoaded)
            {
                sceneWasJustLoaded = false;
                initializeScene = true;
            }

            MelonEvents.OnUpdate.Invoke();
        }

        public void FixedUpdate() 
            => MelonEvents.OnFixedUpdate.Invoke();

        public void LateUpdate() 
            => MelonEvents.OnLateUpdate.Invoke();

        public void OnGUI() 
            => MelonEvents.OnGUI.Invoke();

        public void Quit() 
            => Core.Quit();

        public void BONEWORKS_OnLoadingScreen() 
            => MelonEvents.BONEWORKS_OnLoadingScreen.Invoke();

        public void SetUnhollowerSupportInterface(UnhollowerSupport.Interface inter) 
            => UnhollowerSupport.SMInterface ??= inter;
    }
}