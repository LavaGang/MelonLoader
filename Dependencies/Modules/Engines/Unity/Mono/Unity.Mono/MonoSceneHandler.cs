using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MelonLoader.Engine.Unity.Mono
{
    internal class MonoSceneHandler : IDisposable
    {
        private MonoSupportModule SupportModule;

        internal class SceneInitEvent
        {
            internal int buildIndex;
            internal string name;
            internal bool wasLoadedThisTick;
        }
        private Queue<SceneInitEvent> scenesLoaded = new Queue<SceneInitEvent>();

        internal MonoSceneHandler(MonoSupportModule supportModule)
        {
            SupportModule = supportModule;

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

        ~MonoSceneHandler()
            => Dispose();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2013:Do not use ReferenceEquals with value types")]
        public void Dispose()
        {
            try
            {
                SceneManager.sceneLoaded -= OnSceneLoad;
            }
            catch (Exception ex) { MelonLogger.Error($"SceneManager.sceneLoaded override failed: {ex}"); }

            try
            {
                SceneManager.sceneUnloaded -= OnSceneUnload;
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
