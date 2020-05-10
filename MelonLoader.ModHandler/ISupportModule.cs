using System.Collections;

namespace MelonLoader
{
    public interface ISupportModule
    {
        float GetUnityDeltaTime();
        int GetActiveSceneIndex();

        object StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(object coroutineToken);

        void ProcessWaitForEndOfFrame();
    }
}