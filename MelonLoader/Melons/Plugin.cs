namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }
    }
}