using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public readonly List<MelonPreferences_Entry> Entries = new List<MelonPreferences_Entry>();
        internal Preferences.IO.File File = null;

        internal MelonPreferences_Category(string identifier, string display_name)
        {
            Identifier = identifier;
            DisplayName = display_name;
            MelonPreferences.Categories.Add(this);
        }

        [Obsolete]
        public MelonPreferences_Entry CreateEntry<T>(string identifier, T default_value, string display_name,
            bool is_hidden) => CreateEntry(identifier, default_value, display_name, is_hidden, false);
        
        public MelonPreferences_Entry<T> CreateEntry<T>(string identifier, T default_value, string display_name = null, bool is_hidden = false, bool dont_save_default = false)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier is null or empty when calling CreateEntry");
            if (display_name == null)
                display_name = identifier;
            var entry = GetEntry<T>(identifier);
            if (entry != null)
                throw new Exception($"Calling CreateEntry for { display_name } when it Already Exists");
            entry = new MelonPreferences_Entry<T>
            {
                Identifier = identifier,
                DisplayName = display_name,
                IsHidden = is_hidden,
                DontSaveDefault = dont_save_default,
                Category = this,
                DefaultValue = default_value,
                Value = default_value
            };
            Preferences.IO.File currentFile = File;
            if (currentFile == null)
                currentFile = MelonPreferences.DefaultFile;
            currentFile.SetupEntryFromRawValue(entry);
            Entries.Add(entry);
            return entry;
        }

        public MelonPreferences_Entry GetEntry(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier cannot be null or empty when calling GetEntry");
            if (Entries.Count <= 0)
                return null;
            return Entries.Find(x => x.Identifier.Equals(identifier));
        }
        public MelonPreferences_Entry<T> GetEntry<T>(string identifier) => (MelonPreferences_Entry<T>)GetEntry(identifier);
        public bool HasEntry(string identifier) => GetEntry(identifier) != null;

        public void SetFilePath(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return;
            if (File != null)
                ResetFilePath();
            File = MelonPreferences.GetPrefFileFromFilePath(filepath);
            if (File == null)
            {
                File = new Preferences.IO.File(filepath);
                MelonPreferences.PrefFiles.Add(File);
                try
                {
                    File.Load();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error while Loading Preferences from {File.FilePath}: {ex}");
                    File.WasError = true;
                }
            }
            if ((Entries.Count < 0) || File.WasError)
                return;
            foreach (MelonPreferences_Entry entry in Entries)
                File.SetupEntryFromRawValue(entry);
        }

        public void ResetFilePath()
        {
            if (File == null)
                return;
            Preferences.IO.File oldfile = File;
            File = null;
            if (!MelonPreferences.IsFileInUse(oldfile))
            {
                File.FileWatcher.Destroy();
                MelonPreferences.PrefFiles.Remove(File);
            }
        }
    }
}
