using System.Collections;

namespace MelonLoader.SupportModule
{
    public interface ISupportModule_To
    {
        object StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(object coroutineToken);
        void UnityDebugLog(string msg);
    }
}