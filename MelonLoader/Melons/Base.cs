using System;
using System.Reflection;

namespace MelonLoader
{
    public class MelonBase
    {
        /// <summary>
        /// Assembly of the Melon.
        /// </summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>
        /// File Location of the Melon.
        /// </summary>
        public string Location { get; internal set; }

        // <summary>
        /// Enum for Melon Compatibility.
        /// </summary>
        public enum MelonCompatibility
        {
            INCOMPATIBLE,
            COMPATIBLE,
            NOATTRIBUTE,
            UNIVERSAL,
        }

        /// <summary>
        /// Compatibility of the Melon.
        /// </summary>
        public MelonCompatibility Compatibility { get; internal set; }


        // <summary>
        /// Enum for Melon Priority.
        /// </summary>
        public enum MelonPriority
        {
            HIGHEST,
            HIGH,
            NORMAL,
            LOW,
            LOWEST
        }

        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public MelonPriority Priority { get; internal set; }

        /// <summary>
        /// Gets if the Melon is Universal.
        /// </summary>
        public bool IsUniversal { get { return (Compatibility < MelonCompatibility.COMPATIBLE); } }

        /// <summary>
        /// Gets if the Melon is Compatible with the Game.
        /// </summary>
        public bool IsCompatible { get { return (Compatibility < MelonCompatibility.INCOMPATIBLE); } }

        /// <summary>
        /// Console Color of the Melon.
        /// </summary>
        public ConsoleColor ConsoleColor { get; internal set; }

        /// <summary>
        /// Info Attribute of the Melon.
        /// </summary>
        public MelonInfoAttribute Info { get; internal set; }

        /// <summary>
        /// Game Attributes of the Melon.
        /// </summary>
        public MelonGameAttribute[] Games { get; internal set; }

        /// <summary>
        /// Optional Dependencies Attribute of the Melon.
        /// </summary>
        public MelonOptionalDependenciesAttribute OptionalDependencies { get; internal set; }

        /// <summary>
        /// Auto-Created Harmony Instance of the Melon.
        /// </summary>
        public Harmony.HarmonyInstance Harmony { get; internal set; }

        /// <summary>
        /// Runs after Game Initialization.
        /// </summary>
        public virtual void OnApplicationStart() { }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Unity's IMGUI.
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// Runs when the Game is told to Close.
        /// </summary>
        public virtual void OnApplicationQuit() { }

        /// <summary>
        /// Runs when Melon Preferences get saved.
        /// </summary>
        public virtual void OnPreferencesSaved() { }

        /// <summary>
        /// Runs when Melon Preferences get loaded.
        /// </summary>
        public virtual void OnPreferencesLoaded() { }

        /// <summary>
        /// Runs upon VRChat's UiManager Initialization. Only runs if the Melon is used in VRChat.
        /// </summary>
        public virtual void VRChat_OnUiManagerInit() { }

        [Obsolete("OnModSettingsApplied is obsolete. Please use OnPreferencesSaved instead.")]
        public virtual void OnModSettingsApplied() { }
        [Obsolete("harmonyInstance is obsolete. Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance harmonyInstance { get => Harmony; }
    }
}