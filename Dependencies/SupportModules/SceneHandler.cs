using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if SM_Il2Cpp
using UnityEngine.Events;
#endif

namespace MelonLoader.Support
{
    internal static class SceneHandler
    {
        internal class SceneInitEvent
        {
            internal int buildIndex;
            internal string name;
            internal bool wasLoadedThisTick;
        }

        private static Queue<SceneInitEvent> scenesLoaded = new Queue<SceneInitEvent>();

        internal static void Init()
        {
            try
            {
#if SM_Il2Cpp
                SceneManager.sceneLoaded = (
                    (ReferenceEquals(SceneManager.sceneLoaded, null))
                    ? new Action<Scene, LoadSceneMode>(OnSceneLoad)
                    : Il2CppSystem.Delegate.Combine(SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)new Action<Scene, LoadSceneMode>(OnSceneLoad)).Cast<UnityAction<Scene, LoadSceneMode>>()
                    );
#else
                SceneManager.sceneLoaded += OnSceneLoad;
#endif
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneLoaded override failed: {ex}"); }

            try
            {
#if SM_Il2Cpp
                SceneManager.sceneUnloaded = (
                    (ReferenceEquals(SceneManager.sceneUnloaded, null))
                    ? new Action<Scene>(OnSceneUnload)
                    : Il2CppSystem.Delegate.Combine(SceneManager.sceneUnloaded, (UnityAction<Scene>)new Action<Scene>(OnSceneUnload)).Cast<UnityAction<Scene>>()
                    );
#else
                SceneManager.sceneUnloaded += OnSceneUnload;
#endif
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneUnloaded override failed: {ex}"); }
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (Main.obj == null)
                SM_Component.Create();

            if (ReferenceEquals(scene, null))
                return;

            Main.Interface.OnSceneWasLoaded(scene.buildIndex, scene.name);
            scenesLoaded.Enqueue(new SceneInitEvent { buildIndex = scene.buildIndex, name = scene.name });
        }

        private static void OnSceneUnload(Scene scene)
        {
            if (ReferenceEquals(scene, null))
                return;

            Main.Interface.OnSceneWasUnloaded(scene.buildIndex, scene.name);
        }

        internal static void OnUpdate()
        {
            if (scenesLoaded.Count > 0)
            {
                Queue<SceneInitEvent> requeue = new Queue<SceneInitEvent>();
                SceneInitEvent evt = null;
                while ((scenesLoaded.Count > 0) && ((evt = scenesLoaded.Dequeue()) != null))
                {
                    if (evt.wasLoadedThisTick)
                        Main.Interface.OnSceneWasInitialized(evt.buildIndex, evt.name);
                    else
                    {
                        evt.wasLoadedThisTick = true;
                        requeue.Enqueue(evt);
                    }
                }
                while ((requeue.Count > 0) && ((evt = requeue.Dequeue()) != null))
                    scenesLoaded.Enqueue(evt);
            }
        }
    }
}
