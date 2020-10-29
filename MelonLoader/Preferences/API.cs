using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader
{
    public static class MelonPreferences
    {
        private static string FilePath = null;
        private static string LegacyFilePath = null;
        internal static List<MelonPreferences_Category> categorytbl = new List<MelonPreferences_Category>();
        public static List<MelonPreferences_Category> Categories { get => categorytbl.AsReadOnly().ToList(); }
        private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

        static MelonPreferences()
        {
            FilePath = Path.Combine(Core.UserDataPath, "MelonPreferences.cfg");
            LegacyFilePath = Path.Combine(Core.UserDataPath, "modprefs.ini");
            FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            FileWatcher.Path = Core.UserDataPath;
            FileWatcher.Filter = "MelonPreferences.cfg";
            FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.EnableRaisingEvents = true;
            FileWatcher.BeginInit();
        }

        internal static bool WasLegacyLoaded = false;
        internal static void LegacyCheck()
        {
            if (!File.Exists(LegacyFilePath))
                return;
            string filestr = File.ReadAllText(LegacyFilePath);
            string[] lines = filestr.Split('\n');
            MelonPreferences_Category category = null;
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string newline = line.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                if (newline.Contains("[") && newline.Contains("]"))
                {
                    category = CreateCategory(newline.Replace("[", "").Replace("]", ""));
                    continue;
                }
                if (!newline.Contains("="))
                    continue;
                string[] parts = line.Split('=');
                if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                    continue;
                int val_int = 0;
                float val_float = 0f;
                if (parts[1].ToLower().StartsWith("true") || parts[1].ToLower().StartsWith("false"))
                    category.LoadEntry(parts[0], parts[1].ToLower().StartsWith("true"));
                else if (Int32.TryParse(parts[1], out val_int))
                    category.LoadEntry(parts[0], val_int);
                else if (float.TryParse(parts[1], out val_float))
                    category.LoadEntry(parts[0], val_float);
                else
                    category.LoadEntry(parts[0], parts[1].Replace("\r", ""));
            }
            File.Delete(LegacyFilePath);
            WasLegacyLoaded = true;
        }

        private static void OnFileWatcherTriggered(object source, FileSystemEventArgs e) { if (!IsSaving) Load(); }
        public static void Load()
        {
            MelonLogger.Msg("1");
            if (!File.Exists(FilePath))
                return;
            MelonLogger.Msg("2");
            string filestr = File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return;
            MelonLogger.Msg("3");
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return;
            MelonLogger.Msg("4");
            TomlTable model = docsyn.ToModel();
            if (model.Count <= 0)
                return;
            MelonLogger.Msg("5");
            foreach (KeyValuePair<string, object> keypair in model)
            {
                string category_name = keypair.Key;
                MelonPreferences_Category category = CreateCategory(category_name);
                TomlTable tbl = (TomlTable)keypair.Value;
                if (tbl.Count <= 0)
                    continue;
                foreach (KeyValuePair<string, object> tblkeypair in tbl)
                {
                    string name = tblkeypair.Key;
                    Type type = tblkeypair.Value.GetType();
                    if (type == typeof(string))
                        category.LoadEntry(name, (string)tblkeypair.Value);
                    else if (type == typeof(bool))
                        category.LoadEntry(name, (bool)tblkeypair.Value);
                    else if (type == typeof(int))
                        category.LoadEntry(name, (int)tblkeypair.Value);
                    else if (type == typeof(float))
                        category.LoadEntry(name, (float)tblkeypair.Value);
                }
            }
        }

        private static bool IsSaving = false;
        public static void Save()
        {
            if (categorytbl.Count <= 0)
                return;
            DocumentSyntax doc = new DocumentSyntax();
            foreach (MelonPreferences_Category category in categorytbl)
            {
                TableSyntax tbl = new TableSyntax(category.Name);
                foreach (MelonPreferences_Entry entry in category.prefstbl)
                {
                    KeyValueSyntax key = null;
                    if (entry.Type == MelonPreferences_Entry.TypeEnum.STRING)
                        key = new KeyValueSyntax(entry.Name, new StringValueSyntax(entry.GetString()));
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.BOOL)
                        key = new KeyValueSyntax(entry.Name, new BooleanValueSyntax(entry.GetBool()));
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.INT)
                        key = new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetInt()));
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.FLOAT)
                        key = new KeyValueSyntax(entry.Name, new FloatValueSyntax(entry.GetFloat()));
                    if (key != null)
                        tbl.Items.Add(key);
                }
                doc.Tables.Add(tbl);
            }
            IsSaving = true;
            File.WriteAllText(FilePath, doc.ToString());
            IsSaving = false;
            MelonLogger.Msg("Config Saved!");
            MelonHandler.OnPreferencesSaved();
        }

        public static MelonPreferences_Category GetCategory(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling GetCategory");
            if (categorytbl.Count <= 0)
                return null;
            return categorytbl.Find(x => x.Name.Equals(name));
        }

        public static MelonPreferences_Category CreateCategory(string name, string displayname = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateCategory");
            MelonPreferences_Category category = GetCategory(name);
            if (category != null)
            {
                category.DisplayName = displayname;
                return category;
            }
            return new MelonPreferences_Category(name, displayname);
        }

        public static string GetCategoryDisplayName(string category_name) => GetCategory(category_name)?.DisplayName;
        public static MelonPreferences_Entry GetEntry(string category_name, string entry_name) => GetCategory(category_name)?.GetEntry(entry_name);
        public static bool HasEntry(string category_name, string entry_name) => (GetEntry(category_name, entry_name) != null);
        public static void CreateEntry(string category_name, string entry_name, string defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static void CreateEntry(string category_name, string entry_name, bool defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static void CreateEntry(string category_name, string entry_name, int defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static void CreateEntry(string category_name, string entry_name, float defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static string GetEntryString(string category_name, string entry_name) => GetCategory(category_name)?.GetEntry(entry_name)?.GetString();
        public static void SetEntryString(string category_name, string entry_name, string value) => GetCategory(category_name)?.GetEntry(entry_name)?.SetString(value);
        public static bool GetEntryBool(string category_name, string entry_name) => (bool)GetCategory(category_name)?.GetEntry(entry_name)?.GetBool();
        public static void SetEntryBool(string category_name, string entry_name, bool value) => GetCategory(category_name)?.GetEntry(entry_name)?.SetBool(value);
        public static int GetEntryInt(string category_name, string entry_name) => (int)GetCategory(category_name)?.GetEntry(entry_name)?.GetInt();
        public static void SetEntryInt(string category_name, string entry_name, int value) => GetCategory(category_name)?.GetEntry(entry_name)?.SetInt(value);
        public static float GetEntryFloat(string category_name, string entry_name) => (float)GetCategory(category_name)?.GetEntry(entry_name)?.GetFloat();
        public static void SetEntryFloat(string category_name, string entry_name, float value) => GetCategory(category_name)?.GetEntry(entry_name)?.SetFloat(value);
    }
}