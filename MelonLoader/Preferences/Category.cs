using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool IsHidden { get; internal set; }
        public readonly List<MelonPreferences_Entry> Entries = new List<MelonPreferences_Entry>();
        internal Preferences.IO.File File = null;

        internal MelonPreferences_Category(string identifier, string display_name, bool is_hidden = false)
        {
            Identifier = identifier;
            DisplayName = display_name;
            IsHidden = is_hidden;
            MelonPreferences.Categories.Add(this);
        }

        public MelonPreferences_Entry CreateEntry<T>(string identifier, T default_value, string display_name, bool is_hidden) 
            => CreateEntry(identifier, default_value, display_name, null, is_hidden, false);
        public MelonPreferences_Entry<T> CreateEntry<T>(string identifier, T default_value, string display_name = null, 
            string description = null, bool is_hidden = false, bool dont_save_default = false, Preferences.ValueValidator validator = null)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier is null or empty when calling CreateEntry");

            if (display_name == null)
                display_name = identifier;

            var entry = GetEntry<T>(identifier);
            if (entry != null)
                throw new Exception($"Calling CreateEntry for { display_name } when it Already Exists");

            if (validator != null && !validator.IsValid(default_value))
                throw new ArgumentException($"Default value '{default_value}' is invalid according to the provided ValueValidator!");

            entry = new MelonPreferences_Entry<T>
            {
                Identifier = identifier,
                DisplayName = display_name,
                Description = description,
                IsHidden = is_hidden,
                DontSaveDefault = dont_save_default,
                Category = this,
                DefaultValue = default_value,
                Value = default_value,
                Validator = validator,
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

        public void SetFilePath(string filepath, bool autoload = true)
        {
            if (File != null)
            {
                Preferences.IO.File oldfile = File;
                File = null;
                if (!MelonPreferences.IsFileInUse(oldfile))
                {
                    oldfile.FileWatcher.Destroy();
                    MelonPreferences.PrefFiles.Remove(oldfile);
                }
            }
            if (!string.IsNullOrEmpty(filepath) && !MelonPreferences.IsFilePathDefault(filepath))
            {
                File = MelonPreferences.GetPrefFileFromFilePath(filepath);
                if (File == null)
                {
                    File = new Preferences.IO.File(filepath);
                    MelonPreferences.PrefFiles.Add(File);
                }
            }
            if (autoload)
                MelonPreferences.LoadFileAndRefreshCategories(File);
        }

        public void ResetFilePath()
        {
            if (File == null)
                return;
            Preferences.IO.File oldfile = File;
            File = null;
            if (!MelonPreferences.IsFileInUse(oldfile))
            {
                oldfile.FileWatcher.Destroy();
                MelonPreferences.PrefFiles.Remove(oldfile);
            }
            MelonPreferences.LoadFileAndRefreshCategories(MelonPreferences.DefaultFile);
        }

        public void SaveToFile(bool printmsg = true)
        {
            Preferences.IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;
            foreach (MelonPreferences_Entry entry in Entries)
                if (!(entry.DontSaveDefault && entry.GetValueAsString() == entry.GetDefaultValueAsString()))
                    currentfile.InsertIntoDocument(Identifier, entry.Identifier, entry.Save());
            try
            {
                currentfile.Save();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error while Saving Preferences to {currentfile.FilePath}: {ex}");
                currentfile.WasError = true;
            }
            if (printmsg)
                MelonLogger.Msg($"MelonPreferences Saved to {currentfile.FilePath}");
            MelonHandler.OnPreferencesSaved();
        }

        public void LoadFromFile(bool printmsg = true)
        {
            Preferences.IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;
            MelonPreferences.LoadFileAndRefreshCategories(currentfile, printmsg);
        }

        public void DestroyFileWatcher() => File?.FileWatcher.Destroy();
    }
}
