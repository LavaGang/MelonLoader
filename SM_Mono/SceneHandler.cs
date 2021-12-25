using System;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    internal static class SceneHandler
    {
        internal static void Init()
        {
            try
            {
                SceneManager.sceneLoaded += OnSceneLoad;
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneLoaded override failed: {ex}"); }
            try
            {
                SceneManager.sceneUnloaded += OnSceneUnload;
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneUnloaded override failed: {ex}"); }
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (Main.obj == null) SM_Component.Create(); if (!scene.Equals(null)) Main.Interface.OnSceneWasLoaded(scene.buildIndex, scene.name); }
        private static void OnSceneUnload(Scene scene) { if (scene == null) return; Main.Interface.OnSceneWasUnloaded(scene.buildIndex, scene.name); }
    }
}
