using System.Collections;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Mono
{
    internal class MonoCoroutineInterop : MelonCoroutineInterop
    {
        private MonoSupportComponent Component;

        internal MonoCoroutineInterop(MonoSupportComponent component)
            => Component = component;

        public override object Start(IEnumerator coroutine)
        {
            if (Component != null)
                return Component.StartCoroutine(coroutine);
            MelonCoroutines.Queue(coroutine);
            return coroutine;
        }

        public override void Stop(object coroutineToken)
        {
            if (Component != null)
                Component.StopCoroutine(coroutineToken as Coroutine);
            else
                MelonCoroutines.Dequeue(coroutineToken as IEnumerator);
        }
    }
}
