using System.Collections.Generic;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Config
    {
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
            Category = MelonPreferences.CreateCategory("AssemblyGenerator");
            Category.SetFilePath(Path.Combine(Core.BasePath, "Config.cfg"));

            GameAssemblyHash = Category.CreateEntry<string>("GameAssemblyHash", null);
            DeobfuscationMapHash = Category.CreateEntry<string>("DeobfuscationMapHash", null);
            ObfuscationRegex = Category.CreateEntry<string>("ObfuscationRegex", null);
            UnityVersion = Category.CreateEntry<string>("UnityVersion", null);
            DumperVersion = Category.CreateEntry<string>("DumperVersion", null);
            UnhollowerVersion = Category.CreateEntry<string>("UnhollowerVersion", null);
            OldFiles = Category.CreateEntry("OldFiles", new List<string>());
        }

        internal static void Save() => Category.SaveToFile();
    }
}