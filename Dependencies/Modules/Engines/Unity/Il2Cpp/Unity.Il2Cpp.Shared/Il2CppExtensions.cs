using System.Collections;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    public static class Il2CppExtensions
    {
        #region Coroutine

        internal static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new Il2CppEnumeratorWrapper(enumerator).Pointer));

        internal static void StopCoroutine<T>(this T behaviour, Coroutine routine)
            where T : MonoBehaviour
            => behaviour.StopCoroutine(routine);

        #endregion
    }
}
