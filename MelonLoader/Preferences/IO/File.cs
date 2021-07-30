﻿using System.Linq;
using MelonLoader.Utils;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace MelonLoader.Preferences.IO
{
    internal class File
    {
        private bool _waserror;

        internal bool WasError
        {
            get => _waserror;
            set
            {
                if (value)
                {
                    MelonLogger.Warning($"Defaulting {FilePath} to Fallback Functionality to further avoid File Corruption...");
                    IsSaving = false;
                    FileWatcher.Destroy();
                }

                _waserror = value;
            }
        }

        internal string FilePath;
        internal string LegacyFilePath;
        internal bool IsSaving;
        internal bool ShouldSave = true;
        internal TomlDocument document = TomlDocument.CreateEmpty();
        internal Watcher FileWatcher;

        internal File(string filepath, string legacyfilepath = null, bool shouldsave = true)
        {
            FilePath = filepath;
            LegacyFilePath = legacyfilepath;
            ShouldSave = shouldsave;
            FileWatcher = new Watcher(this);
        }

        internal void LegacyLoad()
        {
            if (string.IsNullOrEmpty(LegacyFilePath) || !System.IO.File.Exists(LegacyFilePath))
                return;
            string filestr = System.IO.File.ReadAllText(LegacyFilePath);
            string[] lines = filestr.Split('\n');
            string category = null;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string newline = line.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                if (newline.Contains("[") && newline.Contains("]"))
                {
                    category = newline.Replace("[", "").Replace("]", "");
                    continue;
                }

                if (!newline.Contains("="))
                    continue;
                string[] parts = line.Split('=');
                if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                    continue;
                if (parts[1].ToLower().StartsWith("true") || parts[1].ToLower().StartsWith("false"))
                    InsertIntoDocument(category, parts[0], TomletMain.ValueFrom(parts[1].ToLower().StartsWith("true")));
                else if (int.TryParse(parts[1], out int val_int))
                    InsertIntoDocument(category, parts[0], TomletMain.ValueFrom(val_int));
                else if (float.TryParse(parts[1], out float val_float))
                    InsertIntoDocument(category, parts[0], TomletMain.ValueFrom(val_float));
                else
                    InsertIntoDocument(category, parts[0], TomletMain.ValueFrom(parts[1].Replace("\r", "")));
            }
        }

        internal void Load()
        {
            if (_waserror)
                return;
            if (!System.IO.File.Exists(FilePath))
                return;
            document = TomlParser.ParseFile(FilePath);
        }

        internal void Save()
        {
            if (_waserror || !ShouldSave)
                return;
            IsSaving = true;
            System.IO.File.WriteAllText(FilePath, document.SerializedValue);
            if (LegacyFilePath != null && System.IO.File.Exists(LegacyFilePath))
                System.IO.File.Delete(LegacyFilePath);
        }
        
        private static string QuoteKey(string key) =>
            key.Contains('"') 
                ? $"'{key}'"
                : $"\"{key}\"";

        internal void InsertIntoDocument(string category, string key, TomlValue value)
        {
            if (!document.ContainsKey(category))
                document.PutValue(category, new TomlTable());
            
            try
            {
                var categoryTable = document.GetSubTable(category);
                categoryTable.PutValue(QuoteKey(key), value);
            }
            catch (TomlTypeMismatchException)
            {
                //Ignore
            }
            catch (TomlNoSuchValueException)
            {
                //Ignore
            }
        }

        internal TomlTable TryGetCategoryTable(string category)
        {
            lock (document)
            {
                try
                {
                    return document.GetSubTable(category);
                }
                catch (TomlTypeMismatchException)
                {
                    //Ignore
                }
                catch (TomlNoSuchValueException)
                {
                    //Ignore
                }

                return null;
            }
        }

        internal void SetupEntryFromRawValue(MelonPreferences_Entry entry)
        {
            lock (document)
            {
                try
                {
                    var categoryTable = document.GetSubTable(entry.Category.Identifier);
                    var value = categoryTable.GetValue(QuoteKey(entry.Identifier));
                    entry.Load(value);
                }
                catch (TomlTypeMismatchException)
                {
                    //Ignore
                }
                catch (TomlNoSuchValueException)
                {
                    //Ignore
                }
            }
        }
    }
}