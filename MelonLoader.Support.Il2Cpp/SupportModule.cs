using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.Support
{
    public class Module : ISupportModule
    {
        public string GetUnityVersion() => Application.unityVersion;
        public float GetUnityDeltaTime() => Time.deltaTime;
        public int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        public object StartCoroutine(IEnumerator coroutine) => MelonCoroutines.Start(coroutine);
        public void StopCoroutine(object coroutineToken) => MelonCoroutines.Stop((IEnumerator)coroutineToken);
        public void UnityDebugLog(string msg) => Debug.Log(msg);
        public ModSettingsMenu_RenderHelper GetModSettingsMenuRenderHelper() => new RenderHelper();
    }
}