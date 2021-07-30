using System.Collections;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SupportModule_To : ISupportModule_To
    {
        public object StartCoroutine(IEnumerator coroutine) => Coroutines.Start(coroutine);
        public void StopCoroutine(object coroutineToken) => Coroutines.Stop((IEnumerator)coroutineToken);
        public void UnityDebugLog(string msg) => Debug.Log(msg);
    }
}