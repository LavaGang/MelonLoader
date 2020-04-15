using MelonLoader;

namespace TestMod
{
    public static class BuildInfo
    {
        public const string Name = "TestMod"; // Name of the Mod.  (MUST BE SET)
        public const string Author = null; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class TestMod : MelonMod
    {
        void OnApplicationStart()
        {
            MelonModLogger.Log("OnApplicationStart");
        }

        void OnLevelWasLoaded(int level)
        {
            MelonModLogger.Log("OnLevelWasLoaded: " + level.ToString());
        }

        void OnLevelWasInitialized(int level)
        {
            MelonModLogger.Log("OnLevelWasInitialized: " + level.ToString());
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

        void VRChat_OnUiManagerInit() // Only works in VRChat
        {
            MelonModLogger.Log("VRChat_OnUiManagerInit");
        }
    }
}
