using System;
using System.Collections;

namespace MelonLoader
{
    public class MelonCoroutines
    {
        private static readonly NotSupportedException nse = new("Support module must be initialized before starting coroutines");

        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine</param>
        /// <returns>An object that can be passed to Stop to stop this coroutine</returns>
        public static object Start(IEnumerator routine)
        {
            if (SupportModule.supportModule == null)
                throw nse;
            return SupportModule.supportModule.StartCoroutine(routine);
        }

        /// <summary>
        /// Stop a currently running coroutine
        /// </summary>
        /// <param name="coroutineToken">The coroutine to stop</param>
        public static void Stop(object coroutineToken)
        {
            if (SupportModule.supportModule == null)
                throw nse;
            SupportModule.supportModule.StopCoroutine(coroutineToken);
        }

        [Obsolete("Use version with IEnumerator parameter", true)]
        public static void Start<T>(T routine) => Start((IEnumerator) routine);
    }
}
