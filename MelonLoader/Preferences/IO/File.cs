using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.IO
{
    internal static class File
    {
        private static bool _waserror = false;
        internal static bool WasError { get => _waserror; set { if (value == true) { MelonLogger.Warning("Defaulting MelonPreferences to Fallback Functionality to further avoid File Corruption..."); IsSaving = false; } _waserror = value; } }
        private static string FilePath = null;
        private static string LegacyFilePath = null;
        internal static bool IsSaving = false;
        internal static Dictionary<string, Dictionary<string, TomlObject>> RawValue = new Dictionary<string, Dictionary<string, TomlObject>>();

        internal static void Setup(string filepath, string legacyfilepath)
        {
            FilePath = filepath;
            LegacyFilePath = legacyfilepath;
            Watcher.Setup(FilePath);
        }

        internal static void LegacyLoad()
        {
            if (!System.IO.File.Exists(LegacyFilePath))
                return;
            string filestr = System.IO.File.ReadAllText(LegacyFilePath);
            string[] lines = filestr.Split('\n');
            string category = null;
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string newline = line.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                if (newline.Contains("[") && newline.Contains("]"))
                {
                    category = newline.Replace("[", "").Replace("]", "");
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
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(parts[1].ToLower().StartsWith("true")));
                else if (int.TryParse(parts[1], out val_int))
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(val_int));
                else if (float.TryParse(parts[1], out val_float))
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(val_float));
                else
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(parts[1].Replace("\r", "")));
            }
        }

        internal static void Load()
        {
            if (_waserror)
                return;
            if (!System.IO.File.Exists(FilePath))
                return;
            string filestr = System.IO.File.ReadAllText(FilePath);
            if (string.IsNullOrEmpty(filestr))
                return;
            DocumentSyntax docsyn = Toml.Parse(filestr);
            if (docsyn == null)
                return;
            TomlTable docmodel = docsyn.ToModel();
            if (docmodel.Count <= 0)
                return;
            foreach (KeyValuePair<string, object> keypair in docmodel)
            {
                string category = keypair.Key;
                if (string.IsNullOrEmpty(category))
                    continue;
                TomlTable tbl = (TomlTable)keypair.Value;
                if (tbl.Count <= 0)
                    continue;
                foreach (KeyValuePair<string, object> tblkeypair in tbl)
                {
                    string identifier = tblkeypair.Key;
                    if (string.IsNullOrEmpty(identifier))
                        continue;
                    TomlObject obj = null;
                    if (tblkeypair.Value is TomlObject tomlObj)
                        obj = tomlObj;
                    else
                    {
                        Type reflectedType = tblkeypair.Value.GetType();
                        if (reflectedType == typeof(bool))
                            obj = new TomlBoolean((bool)tblkeypair.Value);
                        else if (reflectedType == typeof(double))
                            obj = new TomlFloat((double)tblkeypair.Value);
                        else if (reflectedType == typeof(float))
                            obj = new TomlFloat((float)tblkeypair.Value);
                        else if (reflectedType == typeof(long))
                            obj = new TomlInteger((long)tblkeypair.Value);
                        else if (reflectedType == typeof(int))
                            obj = new TomlInteger((int)tblkeypair.Value);
                        else if (reflectedType == typeof(byte))
                            obj = new TomlInteger((byte)tblkeypair.Value);
                    }
                    if (obj == null)
                        continue;
                    SetupRawValue(category, identifier, obj);
                }
            }
        }

        internal static void Save()
        {
            if (_waserror)
                return;
            DocumentSyntax doc = new DocumentSyntax();
            foreach (KeyValuePair<string, Dictionary<string, TomlObject>> keyValuePair in RawValue.ToArray())
            {
                TableSyntax tbl = new TableSyntax(keyValuePair.Key);
                foreach (KeyValuePair<string, TomlObject> keyValuePair2 in keyValuePair.Value.ToArray())
                {
                    ValueSyntax syn = CreateValueSyntaxFromTomlObject(keyValuePair2.Value);
                    if (syn == null)
                        continue;
                    tbl.Items.Add(new KeyValueSyntax(keyValuePair2.Key, syn));
                }
                doc.Tables.Add(tbl);
            }
            IsSaving = true;
            System.IO.File.WriteAllText(FilePath, doc.ToString());
            if (System.IO.File.Exists(LegacyFilePath))
                System.IO.File.Delete(LegacyFilePath);
        }

        internal static ValueSyntax CreateValueSyntaxFromTomlObject(TomlObject obj)
        {
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    return new BooleanValueSyntax(((TomlBoolean)obj).Value);
                case ObjectKind.String:
                    return new StringValueSyntax(((TomlString)obj).Value);
                case ObjectKind.Float:
                    return new FloatValueSyntax(((TomlFloat)obj).Value);
                case ObjectKind.Integer:
                    return new IntegerValueSyntax(((TomlInteger)obj).Value);
                case ObjectKind.Array:
                    return CreateArraySyntaxFromTomlArray((TomlArray)obj);
                default:
                    break;
            }
            return null;
        }

        private static ArraySyntax CreateArraySyntaxFromTomlArray(TomlArray arr)
        {
            if (arr.Count <= 0)
                return null;
            TomlObject obj = arr.GetTomlObject(0);
            switch (obj.Kind)
            {
                case ObjectKind.Boolean:
                    return new ArraySyntax(arr.ToArray<bool>());
                case ObjectKind.String:
                    return new ArraySyntax(arr.ToArray<string>());
                case ObjectKind.Float:
                    return new ArraySyntax(arr.ToArray<double>());
                case ObjectKind.Integer:
                    return new ArraySyntax(arr.ToArray<long>());
                default:
                    break;
            }
            return null;
        }

        internal static void SetupRawValue(string category_identifier, string entry_identifier, TomlObject obj)
        {
            lock (RawValue)
            {
                if (!RawValue.TryGetValue(category_identifier, out Dictionary<string, TomlObject> prefdict))
                {
                    RawValue[category_identifier] = new Dictionary<string, TomlObject>();
                    prefdict = RawValue[category_identifier];
                }
                lock (prefdict)
                    prefdict[entry_identifier] = obj;
            }
        }

        internal static void SetupEntryFromRawValue(MelonPreferences_Entry entry)
        {
            lock (RawValue)
            {
                if (!RawValue.TryGetValue(entry.Category.Identifier, out Dictionary<string, TomlObject> prefdict))
                    return;
                lock (prefdict)
                {
                    if (!prefdict.TryGetValue(entry.Identifier, out TomlObject obj))
                        return;
                    entry.Load(obj);
                }
            }
        }
    }
}
