using System.Collections;

namespace MelonLoader;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "It's public API")]
public interface ISupportModule_To
{
    object StartCoroutine(IEnumerator coroutine);
    void StopCoroutine(object coroutineToken);
    void UnityDebugLog(string msg);
}