using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.SupportModule
{
    public class MonoSupportModule : ISupportModule
    {
        public float GetUnityDeltaTime() => Time.deltaTime;
        public int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        public object StartCoroutine(IEnumerator coroutine) => Main.comp.StartCoroutine(coroutine);
        public void StopCoroutine(object coroutineToken) => Main.comp.StopCoroutine((Coroutine) coroutineToken);
        public void ProcessWaitForEndOfFrame() { }
    }
}