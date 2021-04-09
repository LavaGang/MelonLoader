using System.Collections.Generic;
using System.Linq;
using MelonLoader.Tomlyn;
using MelonLoader.Tomlyn.Model;
using MelonLoader.Tomlyn.Syntax;

namespace MelonLoader.Preferences.IO
{
    internal class File
    {
        private bool _waserror = false;
        internal bool WasError { get => _waserror; set { if (value == true) { MelonLogger.Warning($"Defaulting {FilePath} to Fallback Functionality to further avoid File Corruption..."); IsSaving = false; FileWatcher.Destroy(); } _waserror = value; } }
        internal string FilePath = null;
        internal string LegacyFilePath = null;
        internal bool IsSaving = false;
        internal Dictionary<string, Dictionary<string, TomlObject>> RawValue = new Dictionary<string, Dictionary<string, TomlObject>>();
        internal Watcher FileWatcher = null;

        internal File(string filepath)
        {
            FilePath = filepath;
            FileWatcher = new Watcher(this);
        }

        internal File(string filepath, string legacyfilepath)
        {
            FilePath = filepath;
            LegacyFilePath = legacyfilepath;
            FileWatcher = new Watcher(this);
        }

        internal void LegacyLoad()
        {
            if (string.IsNullOrEmpty(LegacyFilePath) || !System.IO.File.Exists(LegacyFilePath))
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
                if (parts[1].ToLower().StartsWith("true") || parts[1].ToLower().StartsWith("false"))
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(parts[1].ToLower().StartsWith("true")));
                else if (int.TryParse(parts[1], out int val_int))
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject((long)val_int));
                else if (float.TryParse(parts[1], out float val_float))
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject((double)val_float));
                else
                    SetupRawValue(category, parts[0], TomlObject.ToTomlObject(parts[1].Replace("\r", "")));
            }
        }

        internal void Load()
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
                        obj = TomlObject.ToTomlObject(tblkeypair.Value);

                    if (obj == null)
                        continue;
                    SetupRawValue(category, identifier, obj);
                }
            }
        }

        internal void Save()
        {
            if (_waserror)
                return;
            DocumentSyntax doc = new DocumentSyntax();
            foreach (KeyValuePair<string, Dictionary<string, TomlObject>> keyValuePair in RawValue.ToArray())
            {
                TableSyntax tbl = new TableSyntax(keyValuePair.Key);
                foreach (KeyValuePair<string, TomlObject> keyValuePair2 in keyValuePair.Value.ToArray())
                {
                    if (keyValuePair2.Value == null)
                        continue;
                    ValueSyntax syn = CreateValueSyntaxFromTomlObject(keyValuePair2.Value);
                    if (syn == null)
                        continue;
                    tbl.Items.Add(new KeyValueSyntax(keyValuePair2.Key, syn));
                }
                doc.Tables.Add(tbl);
            }
            IsSaving = true;
            System.IO.File.WriteAllText(FilePath, doc.ToString());
            if ((LegacyFilePath != null) && System.IO.File.Exists(LegacyFilePath))
                System.IO.File.Delete(LegacyFilePath);
        }

        private static ValueSyntax CreateValueSyntaxFromTomlObject(TomlObject obj)
        {
            return obj.Kind switch
            {
                ObjectKind.Boolean => new BooleanValueSyntax(((TomlBoolean)obj).Value),
                ObjectKind.String =>  new StringValueSyntax(string.IsNullOrEmpty(((TomlString)obj).Value) ? "" : ((TomlString)obj).Value),
                ObjectKind.Float => new FloatValueSyntax(((TomlFloat)obj).Value),
                ObjectKind.Integer => new IntegerValueSyntax(((TomlInteger)obj).Value),
                ObjectKind.Array => CreateArraySyntaxFromTomlArray((TomlArray)obj),
                _ => null
            };
        }

        private static ArraySyntax CreateArraySyntaxFromTomlArray(TomlArray arr)
        {
            var newSyntax = new ArraySyntax
            {
                OpenBracket = SyntaxFactory.Token(TokenKind.OpenBracket),
                CloseBracket = SyntaxFactory.Token(TokenKind.CloseBracket)
            };
            for(var i = 0; i < arr.Count; i++)
            {
                var item = new ArrayItemSyntax {Value = CreateValueSyntaxFromTomlObject(arr.GetTomlObject(i))};
                if (i + 1 < arr.Count)
                {
                    item.Comma = SyntaxFactory.Token(TokenKind.Comma);
                    item.Comma.AddTrailingWhitespace();
                }
                newSyntax.Items.Add(item);
            }

            return newSyntax;
        }

        internal void SetupRawValue(string category_identifier, string entry_identifier, TomlObject obj)
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

        internal void SetupEntryFromRawValue(MelonPreferences_Entry entry)
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
