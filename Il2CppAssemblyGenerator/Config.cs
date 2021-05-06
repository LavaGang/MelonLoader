using System.Collections.Generic;
using System.IO;
//using MelonLoader.Preferences;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Config
    {
        private static string FilePath;

        private static MelonPreferences_Category Category;
        private static MelonPreferences_Entry<string> GameAssemblyHash;
        private static MelonPreferences_Entry<string> DeobfuscationMapHash;
        private static MelonPreferences_Entry<string> ObfuscationRegex;
        private static MelonPreferences_Entry<string> UnityVersion;
        private static MelonPreferences_Entry<string> DumperVersion;
        private static MelonPreferences_Entry<string> UnhollowerVersion;
        private static MelonPreferences_Entry<List<string>> OldFiles;

        //private static MelonPreferences_ReflectiveCategory Category;
        //internal static AssemblyGeneratorConfiguration Values;
        internal static AssemblyGeneratorConfiguration Values = new AssemblyGeneratorConfiguration();

        internal static void Initialize()
        {
            FilePath = Path.Combine(Core.BasePath, "Config.cfg");

            //Category = MelonPreferences.CreateCategory<AssemblyGeneratorConfiguration>("Il2CppAssemblyGenerator");
            //Category.SetFilePath(FilePath);

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

            //Values = MelonPreferences.GetCategory<AssemblyGeneratorConfiguration>("Il2CppAssemblyGenerator");

            if (!File.Exists(FilePath))
                Save();
            else
                Load();
        }

        private static void Load() => Category.LoadFromFile(false);
        internal static void Save() => Category.SaveToFile(false);

        public class AssemblyGeneratorConfiguration
        {
            public string GameAssemblyHash { get { return Config.GameAssemblyHash.Value; } set { Config.GameAssemblyHash.Value = value; } }
            public string DeobfuscationMapHash { get { return Config.DeobfuscationMapHash.Value; } set { Config.DeobfuscationMapHash.Value = value; } }
            public string ObfuscationRegex { get { return Config.ObfuscationRegex.Value; } set { Config.ObfuscationRegex.Value = value; } }
            public string UnityVersion { get { return Config.UnityVersion.Value; } set { Config.UnityVersion.Value = value; } }
            public string DumperVersion { get { return Config.DumperVersion.Value; } set { Config.DumperVersion.Value = value; } }
            public string UnhollowerVersion { get { return Config.UnhollowerVersion.Value; } set { Config.UnhollowerVersion.Value = value; } }
            public List<string> OldFiles { get { return Config.OldFiles.Value; } set { Config.OldFiles.Value = value; } }

            /*
            public string GameAssemblyHash = null;
            public string DeobfuscationMapHash = null;
            public string ObfuscationRegex = null;
            public string UnityVersion = "0.0.0.0";
            public string DumperVersion = "0.0.0.0";
            public string UnhollowerVersion = "0.0.0.0";
            public List<string> OldFiles = new List<string>();
            */
        }
    }
}