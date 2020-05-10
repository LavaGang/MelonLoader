using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MelonLoader.SupportModule
{
    public class Il2CppSupportModule : ISupportModule
    {
        public float GetUnityDeltaTime() => Time.deltaTime;
        public int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        public object StartCoroutine(IEnumerator coroutine) => MelonCoroutinesIl2Cpp.Start(coroutine);
        public void StopCoroutine(object coroutineToken) => MelonCoroutinesIl2Cpp.Stop(coroutineToken);

        public void ProcessWaitForEndOfFrame() => MelonCoroutinesIl2Cpp.ProcessWaitForEndOfFrame();
    }
}