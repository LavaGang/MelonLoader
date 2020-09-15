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

        static MelonPreferences()
        {
            FilePath = Path.Combine(Core.UserDataPath, "MelonPreferences.cfg");
            LegacyFilePath = Path.Combine(Core.UserDataPath, "modprefs.ini");
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
            File.WriteAllText(FilePath, doc.ToString());
            MelonLogger.Msg("Config Saved!");
            MelonHandler.OnPreferencesApplied();
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
    }
}
