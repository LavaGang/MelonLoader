using MelonLoader.Modules;

namespace MelonLoader.Runtime.Mono
{
    public class MonoRuntimeInfo : MelonRuntimeInfo
    {
        #region Public Members

        public string PosixPath { get; private set; }
        public string ManagedPath { get; private set; }
        public bool IsBleedingEdge { get; private set; }
        public string[] TriggerMethods { get; private set; }

        #endregion

        #region Constructors

        public MonoRuntimeInfo(
            string libraryPath,
            string posixPath,
            string managedPath,
            bool isBleedingEdge,
            string supportModulePath,
            string[] triggerMethods)
        {
            LibraryPath = libraryPath;
            PosixPath = posixPath;
            ManagedPath = managedPath;
            IsBleedingEdge = isBleedingEdge;
            SupportModulePath = supportModulePath;
            TriggerMethods = triggerMethods;
        }

        #endregion
    }
}
