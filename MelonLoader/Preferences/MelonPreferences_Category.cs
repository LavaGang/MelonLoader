using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public readonly List<MelonPreferences_Entry> Entries = new List<MelonPreferences_Entry>();
        internal Preferences.IO.File File = null;

        public string Identifier { get; internal set; }
        public string DisplayName { get; set; }
        public bool IsHidden { get; set; }
        public bool IsInlined { get; set; }

        internal MelonPreferences_Category(string identifier, string display_name, bool is_hidden = false, bool is_inlined = false)
        {
            Identifier = identifier;
            DisplayName = display_name;
            IsHidden = is_hidden;
            IsInlined = is_inlined;
            MelonPreferences.Categories.Add(this);
        }

        public MelonPreferences_Entry CreateEntry<T>(string identifier, T default_value, string display_name, bool is_hidden) 
            => CreateEntry(identifier, default_value, display_name, null, is_hidden, false, null, null);
        public MelonPreferences_Entry<T> CreateEntry<T>(string identifier, T default_value, string display_name,
            string description, bool is_hidden, bool dont_save_default, Preferences.ValueValidator validator)
            => CreateEntry(identifier, default_value, display_name, description, is_hidden, dont_save_default, validator, null);
        public MelonPreferences_Entry<T> CreateEntry<T>(string identifier, T default_value, string display_name = null,
            string description = null, bool is_hidden = false, bool dont_save_default = false, Preferences.ValueValidator validator = null, string oldIdentifier = null)
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

            if (oldIdentifier != null)
            {
                if (HasEntry(oldIdentifier))
                    throw new Exception($"Unable to rename '{oldIdentifier}' when it got already loaded");

                RenameEntry(oldIdentifier, identifier);
            }

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

        public bool DeleteEntry(string identifier)
        {
            MelonPreferences_Entry entry = GetEntry(identifier);
            if (entry != null)
                Entries.Remove(entry);

            Preferences.IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;

            return currentfile.RemoveEntryFromDocument(Identifier, identifier);
        }

        public bool RenameEntry(string identifier, string newIdentifier)
        {
            MelonPreferences_Entry entry = GetEntry(identifier);
            if (entry != null)
                entry.Identifier = newIdentifier;

            Preferences.IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;

            return currentfile.RenameEntryInDocument(Identifier, identifier, newIdentifier);
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

        public void SetFilePath(string filepath) => SetFilePath(filepath, true, true);
        public void SetFilePath(string filepath, bool autoload) => SetFilePath(filepath, autoload, true);
        public void SetFilePath(string filepath, bool autoload, bool printmsg)
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
                MelonPreferences.LoadFileAndRefreshCategories(File, printmsg);
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
                if (!(entry.DontSaveDefault && entry.GetValueAsString() == entry.GetDefaultValueAsString()) && entry.GetValueAsString() != null)
                    currentfile.InsertIntoDocument(Identifier, entry.Identifier, entry.Save(), IsInlined);
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

            MelonPreferences.OnPreferencesSaved.Invoke(currentfile.FilePath);
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
