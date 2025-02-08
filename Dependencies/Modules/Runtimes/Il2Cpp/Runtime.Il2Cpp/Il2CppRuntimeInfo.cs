using MelonLoader.Modules;

namespace MelonLoader.Runtime.Il2Cpp
{
    public class Il2CppRuntimeInfo : MelonRuntimeInfo
    {
        #region Public Members

        public string[] TriggerMethods { get; private set; }

        #endregion

        #region Constructors

        public Il2CppRuntimeInfo(
            string libraryPath,
            string supportModulePath,
            string[] triggerMethods)
        {
            LibraryPath = libraryPath;
            SupportModulePath = supportModulePath;
            TriggerMethods = triggerMethods;
        }

        #endregion
    }
}
