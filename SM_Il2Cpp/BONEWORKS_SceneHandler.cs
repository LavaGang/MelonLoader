namespace MelonLoader.Support
{
    class BONEWORKS_SceneHandler
    {
        private static int LastSceneIndex = -9;
        private static string LastSceneName = null;
        private static bool IsLoading = false;
        private static bool HasFinishedLoading = false;
        private static bool ShouldWait = false;
        private static bool HasGotLoadingSceneIndex = false;
        private static int LoadingSceneIndex = -9;

        internal static void OnSceneLoad(int current_scene, string current_scene_name)
        {
            bool should_run = true;
            if (!HasGotLoadingSceneIndex)
            {
                if (current_scene != 0)
                {
                    LoadingSceneIndex = current_scene;
                    HasGotLoadingSceneIndex = true;
                    LastSceneIndex = current_scene;
                    LastSceneName = current_scene_name;
                    IsLoading = true;
                    should_run = false;
                    ShouldWait = false;
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
                    LastSceneIndex = current_scene;
                    LastSceneName = current_scene_name;
                    IsLoading = true;
                    should_run = false;
                    ShouldWait = false;
                }
            }
            if (should_run)
            {
                LastSceneIndex = current_scene;
                LastSceneName = current_scene_name;
                if (!ShouldWait)
                    IsLoading = true;
            }
        }

        internal static void OnUpdate()
        {
            if (HasFinishedLoading)
            {
                if (LastSceneIndex == LoadingSceneIndex)
                    Main.Interface.BONEWORKS_OnLoadingScreen();
                else
                    Main.Interface.OnSceneWasInitialized(LastSceneIndex, LastSceneName);
                HasFinishedLoading = false;
            }
            if (IsLoading)
            {
                if (LastSceneIndex != LoadingSceneIndex)
                    Main.Interface.OnSceneWasLoaded(LastSceneIndex, LastSceneName);
                HasFinishedLoading = true;
                IsLoading = false;
            }
        }
    }
}
