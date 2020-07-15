using System.Collections;
using UnityEngine;

namespace MelonLoader.Support
{
    public class Module : ISupportModule
    {
        public int GetActiveSceneIndex() => Main.CurrentScene;
        public object StartCoroutine(IEnumerator coroutine) => Main.comp.StartCoroutine(coroutine);
        public void StopCoroutine(object coroutineToken) => Main.comp.StopCoroutine((Coroutine) coroutineToken);
        public void UnityDebugLog(string msg) => Debug.Log(msg);
        public void Destroy() => MelonLoaderComponent.Destroy();
    }
}