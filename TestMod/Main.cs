using MelonLoader;
using System.Collections;

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
        public override void OnPreInitialization() // Runs before Game Initialization. Only runs if the Mod is in the Plugins folder.
        {
            MelonModLogger.Log("OnPreInitialization");
        }

        public override void OnApplicationStart() // Runs after Game Initialization.
        {
            MelonModLogger.Log("OnApplicationStart");
        }

        public override void OnLevelIsLoading() // Runs when a Scene is Loading. Currently only runs if the Mod is used in BONEWORKS.
        {
            MelonModLogger.Log("OnLevelIsLoading");
        }

        public override void OnLevelWasLoaded(int level) // Runs when a Scene has Loaded.
        {
            MelonModLogger.Log("OnLevelWasLoaded: " + level.ToString());
        }

        public override void OnLevelWasInitialized(int level) // Runs when a Scene has Initialized.
        {
            MelonModLogger.Log("OnLevelWasInitialized: " + level.ToString());
        }

        public override void OnUpdate() // Runs once per frame.
        {
            MelonModLogger.Log("OnUpdate");
        }

        public override void OnFixedUpdate() // Can run multiple times per frame. Mostly used for Physics.
        {
            MelonModLogger.Log("OnFixedUpdate");
        }

        public override void OnLateUpdate() // Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        {
            MelonModLogger.Log("OnLateUpdate");
        }

        public override void OnGUI() // Can run multiple times per frame. Mostly used for Unity's IMGUI.
        {
            MelonModLogger.Log("OnGUI");
        }

        public override void OnApplicationQuit() // Runs when the Game is told to Close.
        {
            MelonModLogger.Log("OnApplicationQuit");
        }

        public override void OnModSettingsApplied() // Runs when Mod Preferences get saved to UserData/modprefs.ini.
        {
            MelonModLogger.Log("OnModSettingsApplied");
        }

        public override void VRChat_OnUiManagerInit() // Runs upon VRChat's UiManager Initialization. Only runs if the Mod is used in VRChat.
        {
            MelonModLogger.Log("VRChat_OnUiManagerInit");
        }
    }
}