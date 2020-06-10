using System.Collections;
using UnityEngine;

namespace MelonLoader.Support
{
    public class Module : ISupportModule
    {
        public string GetUnityVersion() => Application.unityVersion;
        public float GetUnityDeltaTime() => Time.deltaTime;
        public int GetActiveSceneIndex() => 0;
        public object StartCoroutine(IEnumerator coroutine) => Main.comp.StartCoroutine(coroutine);
        public void StopCoroutine(object coroutineToken) => Main.comp.StopCoroutine((Coroutine) coroutineToken);
        public void UnityDebugLog(string msg) => Debug.Log(msg);
        public ModSettingsMenu_RenderHelper GetModSettingsMenuRenderHelper() => new RenderHelper();
    }
}