using System.Collections;

namespace MelonLoader.Engine.Unity
{
    public class MelonUnityPlugin : MelonPlugin
    {
        public override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MelonUnityEvents.OnUpdate.Subscribe(OnUpdate, Priority);
            MelonUnityEvents.OnLateUpdate.Subscribe(OnLateUpdate, Priority);
            MelonUnityEvents.OnFixedUpdate.Subscribe(OnFixedUpdate, Priority);
            MelonUnityEvents.OnGUI.Subscribe(OnGUI, Priority);
        }

        /// <summary>
        /// Start a new coroutine.<br />
        /// Coroutines are called at the end of the game Update loops.
        /// </summary>
        /// <param name="routine">The target routine</param>
        /// <returns>An object that can be passed to Stop to stop this coroutine</returns>
        public object StartCoroutine(IEnumerator routine)
            => MelonCoroutines.Start(routine);

        /// <summary>
        /// Stop a currently running coroutine
        /// </summary>
        /// <param name="coroutineToken">The coroutine to stop</param>
        public void StopCoroutine(object coroutineToken)
            => MelonCoroutines.Stop(coroutineToken);

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Physics.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Runs once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Unity's IMGUI.
        /// </summary>
        public virtual void OnGUI() { }
    }
}
