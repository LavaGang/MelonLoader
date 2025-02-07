using System.Collections;
using System.Collections.Generic;

namespace MelonLoader
{
    public abstract class MelonCoroutineInterop
    {
        public abstract object Start(IEnumerator routine);
        public abstract void Stop(object coroutineToken);
    }

    public static class MelonCoroutines
    {
        private static List<IEnumerator> QueuedCoroutines = new List<IEnumerator>();
        private static bool HasProcessedQueue;
        public static MelonCoroutineInterop Interop;

        public static void ProcessQueue()
        {
            if (HasProcessedQueue)
                return;
            HasProcessedQueue = true;
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
            return Queue(routine);
        }

        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine</param>
        /// <returns>An object that can be passed to Stop to stop this coroutine</returns>
        public static object Queue(IEnumerator routine)
        {
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
            Dequeue(coroutineToken as IEnumerator);
        }

        public static void Dequeue(object coroutineToken)
        {
            IEnumerator routine = coroutineToken as IEnumerator;
            if (QueuedCoroutines.Contains(routine))
                QueuedCoroutines.Remove(routine);
        }
    }
}