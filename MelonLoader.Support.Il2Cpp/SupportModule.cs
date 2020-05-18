using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    public class Module : ISupportModule
    {
        public void LoadZippedMods() { }
        public float GetUnityDeltaTime() => Time.deltaTime;
        public int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        public object StartCoroutine(IEnumerator coroutine) => MelonCoroutines.Start(coroutine);
        public void StopCoroutine(object coroutineToken) => MelonCoroutines.Stop(coroutineToken);
        public void ProcessWaitForEndOfFrame() => MelonCoroutines.ProcessWaitForEndOfFrame();
    }
}