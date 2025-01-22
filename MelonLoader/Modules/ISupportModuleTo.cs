using System.Collections;

namespace MelonLoader.Modules;

internal interface ISupportModuleTo
{
    object StartCoroutine(IEnumerator coroutine);
    void StopCoroutine(object coroutineToken);
    void UnityDebugLog(string msg);
}