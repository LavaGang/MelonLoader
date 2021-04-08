using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MelonLoader.Support.Preferences;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static Component component = null;
        private static Camera OnPostRenderCam = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;

            string game_version = Application.version;
            if (string.IsNullOrEmpty(game_version) || game_version.Equals("0"))
                game_version = Application.buildGUID;

            MelonLogger.Msg($"Game Version: {game_version}");
            SetDefaultConsoleTitleWithGameName(game_version); 
            UnityMappers.RegisterMappers();

            LogSupport.RemoveAllHandlers();
            if (MelonDebug.IsEnabled())
                LogSupport.InfoHandler += MelonLogger.Msg;
            LogSupport.WarningHandler += MelonLogger.Warning;
            LogSupport.ErrorHandler += MelonLogger.Error;
            if (MelonDebug.IsEnabled())
                LogSupport.TraceHandler += MelonLogger.Msg;
            ClassInjector.DoHook = MelonUtils.NativeHookAttach;
            InitializeUnityVersion();
            ConsoleCleaner();
            SceneManager.sceneLoaded = (
                   (SceneManager.sceneLoaded == null)
                   ? new Action<Scene, LoadSceneMode>(OnSceneLoad)
                   : Il2CppSystem.Delegate.Combine(SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)new Action<Scene, LoadSceneMode>(OnSceneLoad)).Cast<UnityAction<Scene, LoadSceneMode>>()
                   );
            Camera.onPostRender = (
                (Camera.onPostRender == null)
                ? new Action<Camera>(OnPostRender)
                : Il2CppSystem.Delegate.Combine(Camera.onPostRender, (Camera.CameraCallback)new Action<Camera>(OnPostRender)).Cast<Camera.CameraCallback>()
                );
            ClassInjector.RegisterTypeInIl2Cpp<Component>();
            Component.Create();
            return new SupportModule_To();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode) { if (scene == null) return; if (MelonUtils.IsBONEWORKS) BONEWORKS_SceneHandler.OnSceneLoad(scene.buildIndex, scene.name); else Interface.OnSceneWasLoaded(scene.buildIndex, scene.name); }
        private static void OnPostRender(Camera cam) { if (OnPostRenderCam == null) OnPostRenderCam = cam; if (OnPostRenderCam == cam) Coroutines.ProcessWaitForEndOfFrame(); }

        private static void ConsoleCleaner()
        {
            try
            {
                // Il2CppSystem.Console.SetOut(new Il2CppSystem.IO.StreamWriter(Il2CppSystem.IO.Stream.Null));
                Assembly il2CppSystemAssembly = Assembly.Load("Il2Cppmscorlib");
                Type consoleType = il2CppSystemAssembly.GetType("Il2CppSystem.Console");
                Type streamWriterType = il2CppSystemAssembly.GetType("Il2CppSystem.IO.StreamWriter");
                Type streamType = il2CppSystemAssembly.GetType("Il2CppSystem.IO.Stream");
                MethodInfo setOutMethod = consoleType.GetMethod("SetOut", BindingFlags.Static | BindingFlags.Public);
                MethodInfo nullStreamField = streamType.GetProperty("Null", BindingFlags.Static | BindingFlags.Public).GetGetMethod();
                ConstructorInfo streamWriterCtor = streamWriterType.GetConstructor(new[] { streamType });
                object nullStream = nullStreamField.Invoke(null, new object[0]);
                object steamWriter = streamWriterCtor.Invoke(new[] { nullStream });
                setOutMethod.Invoke(null, new[] { steamWriter });
            }
            catch (Exception ex) { MelonLogger.Error($"Console cleaning failed: {ex}"); }
        }

        private static void InitializeUnityVersion()
        {
            string unityVersion = string.Copy(MelonUtils.GetUnityVersion());
            if (string.IsNullOrEmpty(unityVersion))
                return;
            string[] unityVersionSplit = unityVersion.Split('.');
            if ((unityVersionSplit == null) || (unityVersionSplit.Length < 2))
                return;
            int major = int.Parse(unityVersionSplit[0]);
            int minor = int.Parse(unityVersionSplit[1]);
            int patch = 0;
            if (unityVersionSplit.Length > 2)
            {
                string patchString = unityVersionSplit[2];
                char firstBadChar = patchString.FirstOrDefault(it => it < '0' || it > '9');
                patch = int.Parse(firstBadChar == 0 ? patchString : patchString.Substring(0, patchString.IndexOf(firstBadChar)));
            }
            UnityVersionHandler.Initialize(major, minor, patch);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}