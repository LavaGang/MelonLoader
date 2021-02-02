using System;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    public static class MelonPreferences
    {
        public static readonly List<MelonPreferences_Category> Categories = new List<MelonPreferences_Category>();

        public static readonly TomlMapper Mapper = new TomlMapper();

        static MelonPreferences()
        {
            string FilePath = Path.Combine(MelonUtils.UserDataDirectory, "MelonPreferences.cfg");
            string LegacyFilePath = Path.Combine(MelonUtils.UserDataDirectory, "modprefs.ini");
            Preferences.IO.File.Setup(FilePath, LegacyFilePath);
        }

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
        public static MelonPreferences_Entry CreateEntry<T>(string category_name, string entry_name, T default_value, string display_name = null, bool is_hidden = false) => GetCategory(category_name)?.CreateEntry(entry_name, default_value, display_name, is_hidden);
        public static bool HasEntry(string category_identifier, string entry_identifier) => (GetEntry(category_identifier, entry_identifier) != null);
        public static void SetEntryValue<T>(string category_identifier, string entry_identifier, T value)
        {
            var entry = GetCategory(category_identifier)?.GetEntry<T>(entry_identifier);
            if (entry != null) entry.Value = value;
        }

        public static T GetEntryValue<T>(string category_identifier, string entry_identifier)
        {
            MelonPreferences_Category cat = GetCategory(category_identifier);
            if (cat == null)
                return default;
            var entry = cat.GetEntry<T>(entry_identifier);
            if (entry == null)
                return default;
            return entry.Value;
        }
    }
}
