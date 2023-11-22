namespace MelonLoader.Unity.Mono
{
    public class MonoRuntimeInfo
    {
        public readonly string FilePath;
        public readonly string PosixPath;
        public readonly string ConfigPath;
        public readonly string Variant;
        public readonly bool IsOldMono;

        public MonoRuntimeInfo(string filePath, string posixPath, string configPath, string variant, bool isOldMono)
        {
            FilePath = filePath;
            PosixPath = posixPath;
            ConfigPath = configPath;
            Variant = variant;
            IsOldMono = isOldMono;
        }
    }
}
