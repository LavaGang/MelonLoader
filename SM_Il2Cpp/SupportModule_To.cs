using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MelonLoader.Support
{
    internal class SupportModule_To : ISupportModule_To
    {
        internal static readonly List<IEnumerator> QueuedCoroutines = new List<IEnumerator>();
        public object StartCoroutine(IEnumerator coroutine)
        {
            if (Main.component != null)
                return Main.component.StartCoroutine(new Il2CppSystem.Collections.IEnumerator(new MonoEnumeratorWrapper(coroutine).Pointer));
            QueuedCoroutines.Add(coroutine);
            return coroutine;
        }
        public void StopCoroutine(object coroutineToken)
        {
            if (Main.component == null)
                QueuedCoroutines.Remove(coroutineToken as IEnumerator);
            else
                Main.component.StopCoroutine(coroutineToken as Coroutine);
        }
        public void UnityDebugLog(string msg) => Debug.Log(msg);
    }
}