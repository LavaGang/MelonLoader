using System;
using System.Reflection;
#pragma warning disable 0618

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

        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public int Priority { get; internal set; }

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
        public HarmonyLib.Harmony HarmonyInstance { get; internal set; }

        /// <summary>
        /// Runs after Game Initialization.
        /// </summary>
        public virtual void OnApplicationStart() { }

        /// <summary>
        /// Runs after OnApplicationStart.
        /// </summary>
        public virtual void OnApplicationLateStart() { }

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

        [Obsolete("OnModSettingsApplied is obsolete. Please use OnPreferencesSaved instead.")]
        public virtual void OnModSettingsApplied() { }

        private Harmony.HarmonyInstance _OldHarmonyInstance;
        [Obsolete("harmonyInstance is obsolete. Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance harmonyInstance { get { if (_OldHarmonyInstance == null) _OldHarmonyInstance = new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
        [Obsolete("Harmony is obsolete. Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance Harmony { get { if (_OldHarmonyInstance == null) _OldHarmonyInstance = new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
    }
}