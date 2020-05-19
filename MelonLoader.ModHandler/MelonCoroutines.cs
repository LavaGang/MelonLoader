using System;
using System.Collections;

namespace MelonLoader
{
    public class MelonCoroutines
    {
        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine</param>
        /// <returns>An object that can be passed to Stop to stop this coroutine</returns>
        public static object Start(IEnumerator routine)
        {
            if (SupportModule.supportModule == null)
                throw new NotSupportedException("Support module must be initialized before starting coroutines");
            return SupportModule.supportModule.StartCoroutine(routine);
        }

        /// <summary>
        /// Stop a currently running coroutine
        /// </summary>
        /// <param name="coroutineToken">The coroutine to stop</param>
        public static void Stop(object coroutineToken)
        {
            SupportModule.supportModule?.StopCoroutine(coroutineToken);
        }
        
        [Obsolete("Use version with IEnumerator parameter", true)]
        public static CoroD Start<T>(T routine) => new CoroD { Value = Start((IEnumerator) routine)};

        [Obsolete("Use version with object parameter", true)]
        public static void Stop(CoroD coroD) => Stop(coroD.Value);
        
        [Obsolete("Don't use", true)]
        public class CoroD
        {
            public Object Value;
        }

        private static void ProcessWaitForEndOfFrame()
        {
            SupportModule.supportModule?.ProcessWaitForEndOfFrame();
        }
    }
}
