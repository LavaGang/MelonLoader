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
                    category.CreateEntry(parts[0], parts[1].ToLower().StartsWith("true"));
                else if (Int32.TryParse(parts[1], out val_int))
                    category.CreateEntry(parts[0], val_int);
                else if (float.TryParse(parts[1], out val_float))
                    category.CreateEntry(parts[0], val_float);
                else
                    category.CreateEntry(parts[0], parts[1].Replace("\r", ""));
            }
            File.Delete(LegacyFilePath);
            WasLegacyLoaded = true;
        }

        private static void OnFileWatcherTriggered(object source, FileSystemEventArgs e) { if (!IsSaving) Load(); }
        public static void Load()
        {
            if (!File.Exists(FilePath))
                return;
            string filestr = File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return;
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return;
            TomlTable model = docsyn.ToModel();
            if (model.Count <= 0)
                return;
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
                            entry = category.CreateEntry(name, ((TomlString)obj).Value);
                        else if (obj.Kind == ObjectKind.Boolean)
                            entry = category.CreateEntry(name, ((TomlBoolean)obj).Value);
                        else if (obj.Kind == ObjectKind.Integer)
                            entry = category.CreateEntry(name, ((TomlInteger)obj).Value);
                        else if (obj.Kind == ObjectKind.Float)
                            entry = category.CreateEntry(name, ((TomlFloat)obj).Value);
                    }
                    if ((entry.Type == MelonPreferences_Entry.TypeEnum.STRING) && (obj.Kind != ObjectKind.String))
                        continue;
                    else if ((entry.Type == MelonPreferences_Entry.TypeEnum.STRING) && (obj.Kind == ObjectKind.String))
                        entry.SetValue((string)tblkeypair.Value);
                    if (entry.Type == MelonPreferences_Entry.TypeEnum.BOOL)
                    {
                        bool val = false;
                        if (obj.Kind == ObjectKind.Boolean)
                            val = ((TomlBoolean)obj).Value;
                        else if (obj.Kind == ObjectKind.Integer)
                            val = (((TomlInteger)obj).Value > 0);
                        else if (obj.Kind == ObjectKind.Float)
                            val = (((TomlFloat)obj).Value > 0);
                        entry.SetValue(val);
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.INT)
                    {
                        int val = 0;
                        if (obj.Kind == ObjectKind.Boolean)
                            val = (((TomlBoolean)obj).Value ? 1 : 0);
                        else if (obj.Kind == ObjectKind.Integer)
                            val = (int)((TomlInteger)obj).Value;
                        else if (obj.Kind == ObjectKind.Float)
                            val = (int)((TomlFloat)obj).Value;
                        entry.SetValue(val);
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.LONG)
                    {
                        long val = 0;
                        if (obj.Kind == ObjectKind.Boolean)
                            val = (((TomlBoolean)obj).Value ? 1 : 0);
                        else if (obj.Kind == ObjectKind.Integer)
                            val = ((TomlInteger)obj).Value;
                        else if (obj.Kind == ObjectKind.Float)
                            val = (long)((TomlFloat)obj).Value;
                        entry.SetValue(val);
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.FLOAT)
                    {
                        float val = 0;
                        if (obj.Kind == ObjectKind.Boolean)
                            val = (((TomlBoolean)obj).Value ? 1f : 0f);
                        else if (obj.Kind == ObjectKind.Integer)
                            val = ((TomlInteger)obj).Value;
                        else if (obj.Kind == ObjectKind.Float)
                            val = (float)((TomlFloat)obj).Value;
                        entry.SetValue(val);
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.DOUBLE)
                    {
                        double val = 0;
                        if (obj.Kind == ObjectKind.Boolean)
                            val = (((TomlBoolean)obj).Value ? 1 : 0);
                        else if (obj.Kind == ObjectKind.Integer)
                            val = ((TomlInteger)obj).Value;
                        else if (obj.Kind == ObjectKind.Float)
                            val = ((TomlFloat)obj).Value;
                        entry.SetValue(val);
                    }
                }
            }
            MelonLogger.Msg("Config Loaded!");
            MelonHandler.OnPreferencesLoaded();
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
                    {
                        entry.SetValue(entry.GetEditedValue<string>());
                        key = new KeyValueSyntax(entry.Name, new StringValueSyntax(entry.GetValue<string>()));
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.BOOL)
                    {
                        entry.SetValue(entry.GetEditedValue<bool>());
                        key = new KeyValueSyntax(entry.Name, new BooleanValueSyntax(entry.GetValue<bool>()));
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.INT)
                    {
                        entry.SetValue(entry.GetEditedValue<int>());
                        key = new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetValue<int>()));
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.LONG)
                    {
                        entry.SetValue(entry.GetEditedValue<long>());
                        key = new KeyValueSyntax(entry.Name, new IntegerValueSyntax(entry.GetValue<long>()));
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.FLOAT)
                    {
                        entry.SetValue(entry.GetEditedValue<float>());
                        key = new KeyValueSyntax(entry.Name, new FloatValueSyntax(entry.GetValue<float>()));
                    }
                    else if (entry.Type == MelonPreferences_Entry.TypeEnum.DOUBLE)
                    {
                        entry.SetValue(entry.GetEditedValue<double>());
                        key = new KeyValueSyntax(entry.Name, new FloatValueSyntax(entry.GetValue<double>()));
                    }
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

        public static MelonPreferences_Entry.TypeEnum TypeToTypeEnum<T>() =>
            ((typeof(T) == typeof(string)) ? MelonPreferences_Entry.TypeEnum.STRING
                : (typeof(T) == typeof(bool)) ? MelonPreferences_Entry.TypeEnum.BOOL
                : ((typeof(T) == typeof(int)) ? MelonPreferences_Entry.TypeEnum.INT
                : ((typeof(T) == typeof(float)) ? MelonPreferences_Entry.TypeEnum.FLOAT
                : ((typeof(T) == typeof(long)) ? MelonPreferences_Entry.TypeEnum.LONG
                : ((typeof(T) == typeof(double)) ? MelonPreferences_Entry.TypeEnum.DOUBLE
                : MelonPreferences_Entry.TypeEnum.UNKNOWN)))));
      
        public static Type TypeEnumToType(MelonPreferences_Entry.TypeEnum type) =>
            ((type == MelonPreferences_Entry.TypeEnum.STRING) ? typeof(string)
                : (type == MelonPreferences_Entry.TypeEnum.BOOL) ? typeof(bool)
                : ((type == MelonPreferences_Entry.TypeEnum.INT) ? typeof(int)
                : ((type == MelonPreferences_Entry.TypeEnum.FLOAT) ? typeof(float)
                : ((type == MelonPreferences_Entry.TypeEnum.LONG) ? typeof(long)
                : ((type == MelonPreferences_Entry.TypeEnum.DOUBLE) ? typeof(double)
                : null)))));

        public static string TypeEnumToTypeName(MelonPreferences_Entry.TypeEnum type) => 
            ((type == MelonPreferences_Entry.TypeEnum.STRING) ? "string"
                : (type == MelonPreferences_Entry.TypeEnum.BOOL) ? "bool"
                : ((type == MelonPreferences_Entry.TypeEnum.INT) ? "int"
                : ((type == MelonPreferences_Entry.TypeEnum.FLOAT) ? "float"
                : ((type == MelonPreferences_Entry.TypeEnum.LONG) ? "long"
                : ((type == MelonPreferences_Entry.TypeEnum.DOUBLE) ? "double"
                : null)))));

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