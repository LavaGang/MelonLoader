using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    internal class Il2CppSceneHandler : IDisposable
    {
        private Il2CppSupportModule SupportModule;

        internal class SceneInitEvent
        {
            internal int buildIndex;
            internal string name;
            internal bool wasLoadedThisTick;
        }
        private Queue<SceneInitEvent> scenesLoaded = new Queue<SceneInitEvent>();

        internal Il2CppSceneHandler(Il2CppSupportModule supportModule)
        {
            SupportModule = supportModule;

            try
            {
                SceneManager.sceneLoaded = (
                    (ReferenceEquals(SceneManager.sceneLoaded, null))
                    ? new Action<Scene, LoadSceneMode>(OnSceneLoad)
                    : Il2CppSystem.Delegate.Combine(SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)OnSceneLoad).Cast<UnityAction<Scene, LoadSceneMode>>()
                    );
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneLoaded override failed: {ex}"); }

            try
            {
                SceneManager.sceneUnloaded = (
                    (ReferenceEquals(SceneManager.sceneUnloaded, null))
                    ? new Action<Scene>(OnSceneUnload)
                    : Il2CppSystem.Delegate.Combine(SceneManager.sceneUnloaded, (UnityAction<Scene>)OnSceneUnload).Cast<UnityAction<Scene>>()
                    );
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneUnloaded override failed: {ex}"); }
        }

        ~Il2CppSceneHandler()
            => Dispose();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2013:Do not use ReferenceEquals with value types")]
        public void Dispose()
        {
            try
            {
                SceneManager.sceneLoaded = (
                    (ReferenceEquals(SceneManager.sceneLoaded, null))
                    ? null
                    : Il2CppSystem.Delegate.Remove(SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)OnSceneLoad).Cast<UnityAction<Scene, LoadSceneMode>>()
                    );
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneLoaded override failed: {ex}"); }

            try
            {
                SceneManager.sceneUnloaded = (
                    (ReferenceEquals(SceneManager.sceneUnloaded, null))
                    ? null
                    : Il2CppSystem.Delegate.Remove(SceneManager.sceneUnloaded, (UnityAction<Scene>)OnSceneUnload).Cast<UnityAction<Scene>>()
                    );
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneUnloaded override failed: {ex}"); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2013:Do not use ReferenceEquals with value types")]
        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (SupportModule.obj == null)
                SupportModule.CreateGameObject();

            if (ReferenceEquals(scene, null))
                return;

            MelonUnityEvents.OnSceneWasLoaded.Invoke(scene.buildIndex, scene.name);
            scenesLoaded.Enqueue(new SceneInitEvent { buildIndex = scene.buildIndex, name = scene.name });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2013:Do not use ReferenceEquals with value types")]
        private void OnSceneUnload(Scene scene)
        {
            if (ReferenceEquals(scene, null))
                return;

            MelonUnityEvents.OnSceneWasUnloaded.Invoke(scene.buildIndex, scene.name);
        }

        internal void OnUpdate()
        {
            if (scenesLoaded.Count > 0)
            {
                Queue<SceneInitEvent> requeue = new Queue<SceneInitEvent>();
                SceneInitEvent evt = null;
                while ((scenesLoaded.Count > 0) && ((evt = scenesLoaded.Dequeue()) != null))
                {
                    if (evt.wasLoadedThisTick)
                    {
                        MelonUnityEvents.OnSceneWasInitialized.Invoke(evt.buildIndex, evt.name);
                    }
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
