using System.Collections;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    internal class Il2CppCoroutineInterop : MelonCoroutineInterop
    {
        private Il2CppSupportComponent Component;

        internal Il2CppCoroutineInterop(Il2CppSupportComponent component)
            => Component = component;

        internal override object Start(IEnumerator coroutine)
        {
            if (Component != null)
                return Component.StartCoroutine(new Il2CppSystem.Collections.IEnumerator(new Il2CppEnumeratorWrapper(coroutine).Pointer));

            MelonCoroutines.QueuedCoroutines.Add(coroutine);
            return coroutine;
        }

        internal override void Stop(object coroutineToken)
        {
            if (Component != null)
                Component.StopCoroutine(coroutineToken as Coroutine);
            else
                MelonCoroutines.QueuedCoroutines.Remove(coroutineToken as IEnumerator);
        }
    }
}
