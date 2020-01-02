using MelonLoader;

namespace TestMod
{
    public static class FileInfo
    {
        public const string Name = "TestMod";
        public const string Author = "AUTHOR";
        public const string Company = "COMPANY";
        public const string Version = "1.0.0";
        public const string DownloadLink = "";
    }

    [MelonModInfo(FileInfo.Name, FileInfo.Version, FileInfo.Author, FileInfo.DownloadLink)]
    public class TestMod : MelonMod
    {
        void OnApplicationStart()
        {

        }

        void OnUpdate()
        {
            
        }

        void OnApplicationQuit()
        {
            
        }

        void OnModSettingsApplied()
        {
            
        }
    }
}
