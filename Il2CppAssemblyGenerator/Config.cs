using System.Collections.Generic;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Config
    {
        private static string FilePath;
        private static MelonPreferences_Category Category;
        internal static MelonPreferences_Entry<string> GameAssemblyHash;
        internal static MelonPreferences_Entry<string> DeobfuscationMapHash;
        internal static MelonPreferences_Entry<string> ObfuscationRegex;
        internal static MelonPreferences_Entry<string> UnityVersion;
        internal static MelonPreferences_Entry<string> DumperVersion;
        internal static MelonPreferences_Entry<string> UnhollowerVersion;
        internal static MelonPreferences_Entry<List<string>> OldFiles;

        static Config()
        {
            FilePath = Path.Combine(Core.BasePath, "Config.cfg");

            Category = MelonPreferences.CreateCategory("Il2CppAssemblyGenerator", is_hidden: true);
            Category.SetFilePath(FilePath, false);
            Category.DestroyFileWatcher();

            GameAssemblyHash = Category.CreateEntry<string>("GameAssemblyHash", null, is_hidden: true);
            DeobfuscationMapHash = Category.CreateEntry<string>("DeobfuscationMapHash", null, is_hidden: true);
            ObfuscationRegex = Category.CreateEntry<string>("ObfuscationRegex", null, is_hidden: true);
            UnityVersion = Category.CreateEntry("UnityVersion", "0.0.0.0", is_hidden: true);
            DumperVersion = Category.CreateEntry("DumperVersion", "0.0.0.0", is_hidden: true);
            UnhollowerVersion = Category.CreateEntry("UnhollowerVersion", "0.0.0.0", is_hidden: true);
            OldFiles = Category.CreateEntry("OldFiles", new List<string>(), is_hidden: true);

            if (File.Exists(FilePath))
                Load();
            else
                Save();
        }

        internal static void Load() => Category.LoadFromFile(MelonLaunchOptions.Core.DebugMode);
        internal static void Save() => Category.SaveToFile(MelonLaunchOptions.Core.DebugMode);
    }
}