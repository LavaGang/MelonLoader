using System.Collections;
using System.Collections.Generic;

namespace MelonLoader
{
    internal abstract class MelonCoroutineInterop
    {
        internal abstract object Start(IEnumerator routine);
        internal abstract void Stop(object coroutineToken);
    }

    public static class MelonCoroutines
    {
        internal static List<IEnumerator> QueuedCoroutines = new List<IEnumerator>();
        internal static MelonCoroutineInterop Interop;

        internal static void ProcessQueue()
        {
            foreach (var queuedCoroutine in QueuedCoroutines)
                Start(queuedCoroutine);
            QueuedCoroutines.Clear();
        }

        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine</param>
        /// <returns>An object that can be passed to Stop to stop this coroutine</returns>
        public static object Start(IEnumerator routine)
        {
            if (Interop != null)
                return Interop.Start(routine);
            QueuedCoroutines.Add(routine);
            return routine;
        }

        /// <summary>
        /// Stop a currently running coroutine
        /// </summary>
        /// <param name="coroutineToken">The coroutine to stop</param>
        public static void Stop(object coroutineToken)
        {
            if (Interop != null)
                Interop.Stop(coroutineToken);
            else
                QueuedCoroutines.Remove(coroutineToken as IEnumerator);
        }
    }
}