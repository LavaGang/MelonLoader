using MelonLoader;
using NET_SDK;
using NET_SDK.Reflection;

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
        unsafe void OnApplicationStart()
        {
            MelonModLogger.Log("OnApplicationStart");
        }

        void OnLevelWasLoaded(int level)
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnLevelWasLoaded");
        }

        void OnLevelWasInitialized(int level)
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnLevelWasInitialized");
        }

        void OnUpdate()
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnUpdate");
        }

        void OnFixedUpdate()
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnFixedUpdate");
        }

        void OnLateUpdate()
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnLateUpdate");
        }
        
        void OnGUI()
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnGUI");
        }

        void OnApplicationQuit()
        {
            // Currently only works in MUPOT Mode
            MelonModLogger.Log("OnApplicationQuit");
        }

        void OnModSettingsApplied()
        {
            MelonModLogger.Log("OnModSettingsApplied");
        }
    }
}
