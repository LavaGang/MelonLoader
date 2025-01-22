using System;
using System.Collections;

namespace MelonLoader;

[Obsolete("Why is this public???", true)]
public interface ISupportModule_To
{
    object StartCoroutine(IEnumerator coroutine);
    void StopCoroutine(object coroutineToken);
    void UnityDebugLog(string msg);
}
