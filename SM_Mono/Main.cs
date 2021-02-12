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
        internal static Component component = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            string game_version = Application.version;
            MelonLogger.Msg($"Game Version: {game_version}\n");
            MelonUtils.SetConsoleTitle(GetVersionStrWithGameName(game_version));
            UnityMappers.RegisterMappers();

            SceneManager.sceneLoaded += OnSceneLoad;
            return new SupportModule_To();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (obj == null) Component.Create(); if (!scene.Equals(null)) Interface.OnSceneWasLoaded(scene.buildIndex, scene.name); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string GetVersionStrWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}