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
        public override void OnApplicationStart()
        {
            MelonModLogger.Log("OnApplicationStart");
        }

        public override void OnLevelIsLoading()
        {
            MelonModLogger.Log("OnLevelIsLoading");
        }

        public override void OnLevelWasLoaded(int level)
        {
            MelonModLogger.Log("OnLevelWasLoaded: " + level.ToString());
        }

        public override void OnLevelWasInitialized(int level)
        {
            MelonModLogger.Log("OnLevelWasInitialized: " + level.ToString());
        }

        public override void OnUpdate()
        {
            //lonModLogger.Log("OnUpdate");
        }

        public override void OnFixedUpdate()
        {
            //lonModLogger.Log("OnFixedUpdate");
        }

        public override void OnLateUpdate()
        {
            //MelonModLogger.Log("OnLateUpdate");
        }

        /*
        public override void OnGUI()
        {
            MelonModLogger.Log("OnGUI");
        }
        */

        public override void OnApplicationQuit()
        {
            //MelonModLogger.Log("OnApplicationQuit");
        }

        public override void OnModSettingsApplied()
        {
            //MelonModLogger.Log("OnModSettingsApplied");
        }

        public override void VRChat_OnUiManagerInit() // Only works in VRChat
        {
            MelonModLogger.Log("VRChat_OnUiManagerInit");
        }
    }
}