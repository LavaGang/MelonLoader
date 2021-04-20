using System.Collections.Generic;
using System.IO;
using MelonLoader.Preferences;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Config
    {
        private static string FilePath;
        private static MelonPreferences_ReflectiveCategory Category;

        internal static AssemblyGeneratorConfiguration Values;

        static Config()
        {
            FilePath = Path.Combine(Core.BasePath, "Config.cfg");

            Category = MelonPreferences.CreateCategory<AssemblyGeneratorConfiguration>("Il2CppAssemblyGenerator");
            Category.SetFilePath(FilePath);
            Category.DestroyFileWatcher();

            Values = MelonPreferences.GetCategory<AssemblyGeneratorConfiguration>("Il2CppAssemblyGenerator");

            if (!File.Exists(FilePath))
                Save();
        }

        internal static void Save() => Category.SaveToFile(false);

        public class AssemblyGeneratorConfiguration
        {
            public string GameAssemblyHash = null;
            public string DeobfuscationMapHash = null;
            public string ObfuscationRegex = null;
            public string UnityVersion = "0.0.0.0";
            public string DumperVersion = "0.0.0.0";
            public string UnhollowerVersion = "0.0.0.0";
            public List<string> OldFiles = new List<string>();
        }
    }
}