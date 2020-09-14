using System.Collections;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SupportModule_To : ISupportModule_To
    {
        public object StartCoroutine(IEnumerator coroutine) { if (Main.component != null) return Main.component.StartCoroutine(coroutine); return coroutine; }
        public void StopCoroutine(object coroutineToken) { if (Main.component != null) Main.component.StopCoroutine(coroutineToken as Coroutine); }
        public void UnityDebugLog(string msg) => Debug.Log(msg);
    }
}