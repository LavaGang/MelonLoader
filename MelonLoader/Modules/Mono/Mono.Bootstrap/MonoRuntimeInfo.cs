using System;

namespace MelonLoader.Mono.Bootstrap
{
    public class MonoRuntimeInfo
    {
        #region Public Members

        public eMonoRuntimeVariant Variant { get; private set; }
        public string VariantName { get; private set; }

        public string LibPath { get; private set; }
        public string ConfigPath { get; private set; }
        public string PosixPath { get; private set; }

        public string[] TriggerMethods { get; private set; }

        #endregion

        #region Constructors

        public MonoRuntimeInfo(
            eMonoRuntimeVariant variant,
            string libPath,
            string configPath,
            string[] triggerMethods)
            => SetInfo(variant, libPath, configPath, triggerMethods, null);

        public MonoRuntimeInfo(
            eMonoRuntimeVariant variant, 
            string libPath, 
            string configPath,
            string[] triggerMethods,
            string posixPath)
            => SetInfo(variant, libPath, configPath, triggerMethods, posixPath);

        #endregion

        #region Private Methods

        private void SetInfo(
            eMonoRuntimeVariant variant, 
            string libPath,
            string configPath,
            string[] triggerMethods,
            string posixPath)
        {
            Variant = variant;
            VariantName = Enum.GetName(variant);

            LibPath = libPath;
            ConfigPath = configPath;

            PosixPath = posixPath;

            TriggerMethods = triggerMethods;
        }

        #endregion
    }
}
