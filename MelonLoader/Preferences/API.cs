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
        private static bool _waserror = false;
        internal static bool WasError { get => _waserror; set { if (value == true) MelonLogger.Warning("Disabling Saving and Loading of MelonPreferences to further avoid File Corruption..."); _waserror = value; } }
        private static string FilePath = null;
        private static string LegacyFilePath = null;
        internal static List<MelonPreferences_Category> categorytbl = new List<MelonPreferences_Category>();
        public static List<MelonPreferences_Category> Categories { get => categorytbl.AsReadOnly().ToList(); }
        private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

        static MelonPreferences()
        {
            FilePath = Path.Combine(MelonUtils.UserDataDirectory, "MelonPreferences.cfg");
            LegacyFilePath = Path.Combine(MelonUtils.UserDataDirectory, "modprefs.ini");
            FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            FileWatcher.Path = MelonUtils.UserDataDirectory;
            FileWatcher.Filter = "MelonPreferences.cfg";
            FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.EnableRaisingEvents = true;
            FileWatcher.BeginInit();
        }

        internal static bool SaveAfterEntryCreation = false;
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
                    category.CreateEntry(parts[0], parts[1].ToLower().StartsWith("true"), hidden: true);
                else if (Int32.TryParse(parts[1], out val_int))
                    category.CreateEntry(parts[0], val_int, hidden: true);
                else if (float.TryParse(parts[1], out val_float))
                    category.CreateEntry(parts[0], val_float, hidden: true);
                else
                    category.CreateEntry(parts[0], parts[1].Replace("\r", ""), hidden: true);
            }
            File.Delete(LegacyFilePath);
            WasLegacyLoaded = true;
        }

        private static void OnFileWatcherTriggered(object source, FileSystemEventArgs e) { if (!IsSaving) Load(); else IsSaving = false; }
        public static void Load()
        {
            if (WasError || !Load_Internal())
                return;
            MelonLogger.Msg("Config Loaded!");
            MelonHandler.OnPreferencesLoaded();
        }
        internal static bool Load_Internal()
        {
            if (WasError || !File.Exists(FilePath))
                return false;
            string filestr = File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return false;
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return false;
            TomlTable model = docsyn.ToModel();
            if (model.Count <= 0)
                return false;
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
                    if (string.IsNullOrEmpty(name))
                        continue;
                    TomlObject obj = TomlObject.ToTomlObject(tblkeypair.Value);
                    if (obj == null)
                        continue;
                    MelonPreferences_Entry entry = category.GetEntry(name);
                    if (entry == null)
                    {
                        if (obj.Kind == ObjectKind.String)
                            entry = category.CreateEntry(name, ((TomlString)obj).Value, hidden: true);
                        else if (obj.Kind == ObjectKind.Boolean)
                            entry = category.CreateEntry(name, ((TomlBoolean)obj).Value, hidden: true);
                        else if (obj.Kind == ObjectKind.Integer)
                            entry = category.CreateEntry(name, ((TomlInteger)obj).Value, hidden: true);
                        else if (obj.Kind == ObjectKind.Float)
                            entry = category.CreateEntry(name, ((TomlFloat)obj).Value, hidden: true);
                        else if(obj.Kind == ObjectKind.Array)
                        {
                            TomlArray arr = (TomlArray)obj;
                            if (arr.Count() <= 0)
                                continue;
                            TomlObject arrobj = TomlObject.ToTomlObject(arr[0]);
                            if (arrobj.Kind == ObjectKind.String)
                                entry = category.CreateEntry(name, arr.ToArray<string>(), hidden: true);
                            else if (arrobj.Kind == ObjectKind.Boolean)
                                entry = category.CreateEntry(name, arr.ToArray<bool>(), hidden: true);
                            else if (obj.Kind == ObjectKind.Integer)
                                entry = category.CreateEntry(name, arr.ToArray<long>(), hidden: true);
                            else if (obj.Kind == ObjectKind.Float)
                                entry = category.CreateEntry(name, arr.ToArray<double>(), hidden: true);
                        }
                    }
                    if (entry == null)
                        continue;
                    Preferences.TypeManager.Load(entry, obj);
                }
            }
            return true;
        }

        private static bool IsSaving = false;
        public static void Save()
        {
            if (WasError || !Save_Internal())
                return;
            MelonLogger.Msg("Config Saved!");
            MelonHandler.OnPreferencesSaved();
        }
        internal static bool Save_Internal()
        {
            if (WasError || (categorytbl.Count <= 0))
                return false;
            DocumentSyntax doc = new DocumentSyntax();
            foreach (MelonPreferences_Category category in categorytbl)
            {
                TableSyntax tbl = new TableSyntax(category.Name);
                foreach (MelonPreferences_Entry entry in category.prefstbl)
                {
                    KeyValueSyntax key = Preferences.TypeManager.Save(entry);
                    if (key != null)
                        tbl.Items.Add(key);
                }
                doc.Tables.Add(tbl);
            }
            IsSaving = true;
            File.WriteAllText(FilePath, doc.ToString());
            return true;
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

        public static MelonPreferences_Entry.TypeEnum TypeToTypeEnum<T>() => Preferences.TypeManager.TypeToTypeEnum<T>();
        public static Type TypeEnumToReflectedType(MelonPreferences_Entry.TypeEnum type) => Preferences.TypeManager.TypeEnumToReflectedType(type);
        public static string TypeEnumToTypeName(MelonPreferences_Entry.TypeEnum type) => Preferences.TypeManager.TypeEnumToTypeName(type);
        public static string GetCategoryDisplayName(string category_name) => GetCategory(category_name)?.DisplayName;
        public static MelonPreferences_Entry GetEntry(string category_name, string entry_name) => GetCategory(category_name)?.GetEntry(entry_name);
        public static bool HasEntry(string category_name, string entry_name) => (GetEntry(category_name, entry_name) != null);
        public static MelonPreferences_Entry CreateEntry<T>(string category_name, string entry_name, T defaultValue, string displayText = null, bool hideFromList = false) => GetCategory(category_name)?.CreateEntry(entry_name, defaultValue, displayText, hideFromList);
        public static void SetEntryValue<T>(string category_name, string entry_name, T value) => GetCategory(category_name)?.GetEntry(entry_name)?.SetValue(value);
        public static T GetEntryValue<T>(string category_name, string entry_name)
        {
            MelonPreferences_Category cat = GetCategory(category_name);
            if (cat == null)
                return default;
            MelonPreferences_Entry entry = cat.GetEntry(entry_name);
            if (entry == null)
                return default;
            return entry.GetValue<T>();
        }
    }
}