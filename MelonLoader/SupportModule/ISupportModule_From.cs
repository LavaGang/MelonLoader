﻿namespace MelonLoader.SupportModule
{
    public interface ISupportModule_From
    {
        void OnSceneWasLoaded(int buildIndex, string sceneName);
        void OnSceneWasInitialized(int buildIndex, string sceneName);
        void OnSceneWasUnloaded(int buildIndex, string sceneName);
        void Update();
        void FixedUpdate();
        void LateUpdate();
        void OnGUI();
        void Quit();
        void BONEWORKS_OnLoadingScreen();
    }
}