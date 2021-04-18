using System;
using Tomlet;
using Tomlet.Models;

namespace MelonLoader.Preferences
{
    public class MelonPreferences_ReflectiveCategory
    {
        private Type type;
        private object value;
        private string categoryName;
        private string displayName;
        internal IO.File File = null;
        
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }

        public static MelonPreferences_ReflectiveCategory Create<T>(string categoryName, string displayName) => new MelonPreferences_ReflectiveCategory(typeof(T), categoryName, displayName);
        
        private MelonPreferences_ReflectiveCategory(Type type, string categoryName, string displayName)
        {
            this.type = type;
            this.categoryName = categoryName;
            this.displayName = displayName;
            MelonPreferences.ReflectiveCategories.Add(type, this);
        }

        public void Load(TomlValue tomlValue) => value = TomletMain.To(type, tomlValue);

        public TomlValue Save() => TomletMain.ValueFrom(type, value);

        public T GetValue<T>() where T : new()
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
    }
}