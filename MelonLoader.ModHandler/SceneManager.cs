using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace MelonLoader
{
    public static class SceneManager
    {
        private static int LastSceneIndex = -9;
        private static bool IsLoading = false;
        private static bool HasFinishedLoading = false;
        private static bool ShouldWait = false;
        private static bool Boneworks_HasGotLoadingSceneIndex = false;
        private static int Boneworks_LoadingSceneIndex = -9;

        internal static void CheckForSceneChange()
        {
            CheckForSceneInitialized();
            CheckForSceneFinishedLoading();
            CheckForSceneLoading();
        }

        unsafe public static int GetActiveSceneIndex()
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene != null)
                return scene.buildIndex;
            return 3000;
        }

        private static void CheckForSceneLoading()
        {
            int current_scene = GetActiveSceneIndex();
            if (LastSceneIndex != current_scene)
            {
                bool should_run = true;
                if (Imports.IsIl2CppGame())
                {
                    if (Main.IsBoneworks)
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
                                    ShouldWait = true;
                                else
                                {
                                    LastSceneIndex = current_scene;
                                    IsLoading = true;
                                    should_run = false;
                                    ShouldWait = false;
                                }
                            }
                        }
                    }
                }
                if (should_run)
                {
                    LastSceneIndex = current_scene;
                    Main.OnLevelIsLoading();
                    if (!ShouldWait)
                        IsLoading = true;
                }
            }
        }

        private static void CheckForSceneFinishedLoading()
        {
            if (IsLoading)
            {
                Main.OnLevelWasLoaded(LastSceneIndex);
                HasFinishedLoading = true;
                IsLoading = false;
            }
        }

        private static void CheckForSceneInitialized()
        {
            if (HasFinishedLoading)
            {
                Main.OnLevelWasInitialized(LastSceneIndex);
                HasFinishedLoading = false;
            }
        }
    }
}
