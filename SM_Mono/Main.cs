using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MelonLoader.Support.Preferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static SM_Component component = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            string game_version = MelonUtils.Application_Version;
            if (string.IsNullOrEmpty(game_version) || game_version.Equals("0"))
                game_version = MelonUtils.Application_BuildGUID;
            MelonLogger.Msg($"Game Version: {game_version}");
            SetDefaultConsoleTitleWithGameName(game_version);
            UnityMappers.RegisterMappers();

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
            return new SupportModule_To();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (obj == null) SM_Component.Create(); if (!scene.Equals(null)) Interface.OnSceneWasLoaded(scene.buildIndex, scene.name); }
        private static void OnSceneUnload(Scene scene) { if (scene == null) return; Interface.OnSceneWasUnloaded(scene.buildIndex, scene.name); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}