using System;
using Tomlet;
using Tomlet.Models;

namespace MelonLoader.Preferences
{
    public class MelonPreferences_ReflectiveCategory
    {
        private Type type;
        private object value;
        internal IO.File File = null;
        
        public string Identifier { get;  internal set; }
        public string DisplayName { get; internal set; }

        internal static MelonPreferences_ReflectiveCategory Create<T>(string categoryName, string displayName) => new MelonPreferences_ReflectiveCategory(typeof(T), categoryName, displayName);
        
        private MelonPreferences_ReflectiveCategory(Type type, string categoryName, string displayName)
        {
            this.type = type;
            this.Identifier = categoryName;
            this.DisplayName = displayName;
            MelonPreferences.ReflectiveCategories.Add(type, this);
        }

        internal void LoadDefaults() => value = Activator.CreateInstance(type);

        internal void Load(TomlValue tomlValue) => value = TomletMain.To(type, tomlValue);

        internal TomlValue Save() => TomletMain.ValueFrom(type, value);

        internal T GetValue<T>() where T : new()
        {
            if (typeof(T) != type)
                return default;

            return (T) value;
        }
        
        public void SetFilePath(string filepath, bool autoload = true)
        {
            if (File != null)
            {
                IO.File oldfile = File;
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
                    File = new IO.File(filepath);
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
            IO.File oldfile = File;
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
            IO.File currentfile = File;
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

        public void DestroyFileWatcher() => File?.FileWatcher.Destroy();
    }
}