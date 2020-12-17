namespace MelonLoader
{
    internal static class SceneHandler
    {
        private static int LastSceneIndex = -9;
        private static string LastSceneName = null;
        private static bool IsLoading = false;
        private static bool HasFinishedLoading = false;
        private static bool ShouldWait = false;
        private static bool Boneworks_HasGotLoadingSceneIndex = false;
        private static int Boneworks_LoadingSceneIndex = -9;

        internal static void OnSceneLoad(int current_scene, string current_scene_name)
        {
            bool should_run = true;
            if (MelonUtils.IsBONEWORKS)
            {
                if (!Boneworks_HasGotLoadingSceneIndex)
                {
                    if (current_scene != 0)
                    {
                        Boneworks_LoadingSceneIndex = current_scene;
                        Boneworks_HasGotLoadingSceneIndex = true;
                        should_run = false;
                    }
                    else
                    {
                        LastSceneIndex = current_scene;
                        LastSceneName = current_scene_name;
                        IsLoading = true;
                        should_run = false;
                    }
                }
                else
                {
                    if (current_scene == -1)
                        should_run = false;
                    else
                    {
                        if (current_scene == Boneworks_LoadingSceneIndex)
                        {
                            MelonHandler.BONEWORKS_OnLoadingScreen();
                            ShouldWait = true;
                        }
                        else
                        {
                            LastSceneIndex = current_scene;
                            LastSceneName = current_scene_name;
                            IsLoading = true;
                            should_run = false;
                            ShouldWait = false;
                        }
                    }
                }
            }
            if (should_run)
            {
                LastSceneIndex = current_scene;
                LastSceneName = current_scene_name;
                if (!ShouldWait)
                    IsLoading = true;
                if (!MelonUtils.IsBONEWORKS)
                    CheckForSceneFinishedLoading();
            }
        }

        internal static void CheckForSceneChange()
        {
            CheckForSceneInitialized();
            CheckForSceneFinishedLoading();
        }

        private static void CheckForSceneFinishedLoading()
        {
            if (IsLoading)
            {
                MelonHandler.OnSceneWasLoaded(LastSceneIndex, LastSceneName);
                HasFinishedLoading = true;
                IsLoading = false;
            }
        }

        private static void CheckForSceneInitialized()
        {
            if (HasFinishedLoading)
            {
                MelonHandler.OnSceneWasInitialized(LastSceneIndex, LastSceneName);
                HasFinishedLoading = false;
            }
        }
    }
}