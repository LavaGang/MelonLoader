namespace MelonLoader.Mono.Bootstrap
{
    public class MonoRuntimeInfo
    {
        #region Public Members

        public string Variant { get; private set; }
        public string LibPath { get; private set; }
        public string ConfigPath { get; private set; }
        public string PosixPath { get; private set; }

        #endregion

        #region Constructors

        public MonoRuntimeInfo(
            string variant,
            string libPath,
            string configPath)
            => SetInfo(variant, libPath, configPath, null);

        public MonoRuntimeInfo(
            string variant, 
            string libPath, 
            string configPath,
            string posixPath)
            => SetInfo(variant, libPath, configPath, posixPath);

        #endregion

        #region Private Methods

        private void SetInfo(
            string variant, 
            string libPath,
            string configPath,
            string posixPath)
        {
            Variant = variant;
            LibPath = libPath;
            ConfigPath = configPath;
            PosixPath = posixPath;
        }

        #endregion
    }
}
