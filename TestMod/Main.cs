using MelonLoader;

namespace TestMod
{
    public static class BuildInfo
    {
        public const string Name = "TestMod";
        public const string Author = "AUTHOR";
        public const string Company = "COMPANY";
        public const string Version = "1.0.0";
        public const string DownloadLink = "";
    }

    public class TestMod : MelonMod
    {
        void OnApplicationStart()
        {
            MelonModLogger.Log("OnApplicationStart");
        }

        void OnLevelWasLoaded(int level)
        {
            MelonModLogger.Log("OnLevelWasLoaded");
        }

        void OnLevelWasInitialized(int level)
        {
            MelonModLogger.Log("OnLevelWasInitialized");
        }

        void OnUpdate()
        {
            MelonModLogger.Log("OnUpdate");
        }
        void OnFixedUpdate()
        {
            MelonModLogger.Log("OnFixedUpdate");
        }

        void OnLateUpdate()
        {
            MelonModLogger.Log("OnLateUpdate");
        }
        
        void OnGUI()
        {
            MelonModLogger.Log("OnGUI");
        }

        void OnApplicationQuit()
        {
            MelonModLogger.Log("OnApplicationQuit");
        }

        void OnModSettingsApplied()
        {
            MelonModLogger.Log("OnModSettingsApplied");
        }
    }
}
