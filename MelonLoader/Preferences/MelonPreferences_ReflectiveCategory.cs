using System;
using MelonLoader.Melons;
using MelonLoader.Preferences.IO;
using MelonLoader.Utils;
using Tomlet;
using Tomlet.Models;

namespace MelonLoader.Preferences
{
    public class MelonPreferences_ReflectiveCategory
    {
        private Type SystemType;
        private object value;
        internal File File;
        
        public string Identifier { get;  internal set; }
        public string DisplayName { get; internal set; }

        internal static MelonPreferences_ReflectiveCategory Create<T>(string categoryName, string displayName) => new MelonPreferences_ReflectiveCategory(typeof(T), categoryName, displayName);
        
        private MelonPreferences_ReflectiveCategory(Type type, string categoryName, string displayName)
        {
            SystemType = type;
            Identifier = categoryName;
            DisplayName = displayName;

            File currentFile = File;
            if (currentFile == null)
                currentFile = MelonPreferences.DefaultFile;
            if (!(currentFile.TryGetCategoryTable(Identifier) is { } table))
                LoadDefaults();
            else
                Load(table);

            MelonPreferences.ReflectiveCategories.Add(type, this);
        }

        internal void LoadDefaults() => value = Activator.CreateInstance(SystemType);

        internal void Load(TomlValue tomlValue) => value = TomletMain.To(SystemType, tomlValue);

        internal TomlValue Save()
        {
            if(value == null)
                LoadDefaults();
            
            return TomletMain.ValueFrom(SystemType, value);
        }

        public T GetValue<T>() where T : new()
        {
            if (typeof(T) != SystemType)
                return default;
            if (value == null)
                LoadDefaults();
            return (T) value;
        }
        
        public void SetFilePath(string filepath, bool autoload = true, bool printmsg = true)
        {
            if (File != null)
            {
                File oldfile = File;
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
                    File = new File(filepath);
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
            File oldfile = File;
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
            File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;

            currentfile.document.PutValue(Identifier, Save());
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
            File currentfile = File;
            if (currentfile == null)
                currentfile = MelonPreferences.DefaultFile;
            MelonPreferences.LoadFileAndRefreshCategories(currentfile, printmsg);
        }

        public void DestroyFileWatcher() => File?.FileWatcher.Destroy();
    }
}