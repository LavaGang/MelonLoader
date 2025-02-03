﻿using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader.Preferences;

namespace MelonLoader.Engine.Unity
{
    internal class Config
    {
        private static string FilePath;
        private static MelonPreferences_ReflectiveCategory Category;
        internal static AssemblyGeneratorConfiguration Values;

        internal static void Initialize(string BasePath)
        {
            FilePath = Path.Combine(BasePath, "Config.cfg");

            Category = MelonPreferences.CreateCategory<AssemblyGeneratorConfiguration>("Il2CppAssemblyGenerator");
            Category.SetFilePath(FilePath, printmsg: false);
            Category.DestroyFileWatcher();

            Values = Category.GetValue<AssemblyGeneratorConfiguration>();

            if (!File.Exists(FilePath))
                Save();
        }

        internal static void Save() => Category.SaveToFile(false);

        public class AssemblyGeneratorConfiguration
        {
            public string GameAssemblyHash = null;
            public string DeobfuscationRegex = null;
            public string UnityVersion = "0.0.0.0";
            public string DumperVersion = "0.0.0.0";
            public string DumperSCRSVersion = "0.0.0.0";

            [Obsolete("Il2CppAssemblyUnhollower support was discontinued. This will be removed in a future update.", true)]
            public bool UseInterop = true;

            public List<string> OldFiles = new List<string>();
        }
    }
}