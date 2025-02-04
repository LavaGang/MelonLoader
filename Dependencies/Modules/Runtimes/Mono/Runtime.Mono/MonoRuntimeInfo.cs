using MelonLoader.Modules;

namespace MelonLoader.Runtime.Mono
{
    public class MonoRuntimeInfo : MelonRuntimeInfo
    {
        #region Public Members

        public string[] TriggerMethods { get; private set; }

        #endregion

        #region Constructors

        public MonoRuntimeInfo(
            string libPath,
            string[] triggerMethods)
       {
            LibPath = libPath;
            TriggerMethods = triggerMethods;
        }

        #endregion
    }
}
