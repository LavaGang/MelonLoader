using MelonLoader;

namespace TestMod
{
    public static class FileInfo
    {
        public const string Name = "TestMod";
        public const string Author = "Herp Derpinstine";
        public const string Company = "NanoNuke @ nanonuke.net";
        public const string Version = "1.0.0";
    }
    [MelonModInfo(FileInfo.Name, FileInfo.Version, FileInfo.Author)]

    public class TestMod : MelonMod
    {
        void OnApplicationStart()
        {
            Logger.Log("OnApplicationStart");
        }

        void OnApplicationQuit()
        {
            Logger.Log("OnApplicationQuit");
        }

        void OnModSettingsApplied()
        {
            Logger.Log("OnModSettingsApplied");
        }
    }
}
