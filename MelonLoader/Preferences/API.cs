using System;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    public static class MelonPreferences
    {
        public static readonly List<MelonPreferences_Category> Categories = new List<MelonPreferences_Category>();
        private static event EventHandler<MelonPreferences_Entry.ResolveEventArgs> EntryTypeResolveEvents;

        static MelonPreferences()
        {
            string FilePath = Path.Combine(MelonUtils.UserDataDirectory, "MelonPreferences.cfg");
            string LegacyFilePath = Path.Combine(MelonUtils.UserDataDirectory, "modprefs.ini");
            Preferences.IO.File.Setup(FilePath, LegacyFilePath);

            AddEntryTypeResolveEvent(Preferences.Types.Array_Boolean.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_Byte.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_Double.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_Float.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_Integer.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_Long.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Array_String.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Boolean.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Byte.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Double.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Float.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Integer.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.Long.Resolve);
            AddEntryTypeResolveEvent(Preferences.Types.String.Resolve);
        }
        public static void AddEntryTypeResolveEvent(EventHandler<MelonPreferences_Entry.ResolveEventArgs> evt) => EntryTypeResolveEvents += evt;
        internal static void InvokeEntryTypeResolveEvents(MelonPreferences_Entry.ResolveEventArgs args) => EntryTypeResolveEvents?.Invoke(null, args);

        public static void Load()
        {
            try
            {
                Preferences.IO.File.LegacyLoad();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Legacy settings load failed: {ex}");
                Preferences.IO.File.WasError = true;
            }

            try
            {
                Preferences.IO.File.Load();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Settings load failed: {ex}");
                Preferences.IO.File.WasError = true;
            }
            if (!Preferences.IO.File.WasError && (Categories.Count > 0))
            {
                foreach (MelonPreferences_Category cat in Categories)
                {
                    if (cat.Entries.Count < 0)
                        continue;
                    foreach (MelonPreferences_Entry entry in cat.Entries)
                        Preferences.IO.File.SetupEntryFromRawValue(entry);
                }
            }
            MelonLogger.Msg("Preferences Loaded!");
            MelonHandler.OnPreferencesLoaded();
        }

        public static void Save()
        {
            foreach (MelonPreferences_Category category in Categories)
                foreach (MelonPreferences_Entry entry in category.Entries)
                    Preferences.IO.File.SetupRawValue(category.Identifier, entry.Identifier, entry.Save());
            try
            {
                Preferences.IO.File.Save();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error while saving settings: {ex}");
                Preferences.IO.File.WasError = true;
            }
            MelonLogger.Msg("Preferences Saved!");
            MelonHandler.OnPreferencesSaved();
        }

        public static MelonPreferences_Category CreateCategory(string identifier, string displayname = null)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier is null or empty when calling CreateCategory");
            MelonPreferences_Category category = GetCategory(identifier);
            if (category != null)
                throw new Exception($"Calling CreateCategory for { identifier } when it Already Exists");
            return new MelonPreferences_Category(identifier, displayname);
        }

        public static MelonPreferences_Category GetCategory(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier is null or empty when calling GetCategory");
            if (Categories.Count <= 0)
                return null;
            return Categories.Find(x => x.Identifier.Equals(identifier));
        }
        public static MelonPreferences_Entry GetEntry(string category_identifier, string entry_identifier) => GetCategory(category_identifier)?.GetEntry(entry_identifier);
        public static MelonPreferences_Entry CreateEntry<T>(string category_name, string entry_name, T defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static bool HasEntry(string category_identifier, string entry_identifier) => (GetEntry(category_identifier, entry_identifier) != null);
        public static void SetEntryValue<T>(string category_identifier, string entry_identifier, T value) => GetCategory(category_identifier)?.GetEntry(entry_identifier)?.SetValue(value);
        public static T GetEntryValue<T>(string category_identifier, string entry_identifier)
        {
            MelonPreferences_Category cat = GetCategory(category_identifier);
            if (cat == null)
                return default;
            MelonPreferences_Entry entry = cat.GetEntry(entry_identifier);
            if (entry == null)
                return default;
            return entry.GetValue<T>();
        }
    }
}
