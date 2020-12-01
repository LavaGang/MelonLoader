using System;
using System.Collections.Generic;
using System.Linq;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        internal List<MelonPreferences_Entry> prefstbl = new List<MelonPreferences_Entry>();
        public List<MelonPreferences_Entry> Prefs { get => prefstbl.AsReadOnly().ToList(); }
        internal MelonPreferences_Category(string name, string displayname)
        {
            Name = name;
            DisplayName = displayname;
            MelonPreferences.categorytbl.Add(this);
        }

        public MelonPreferences_Entry GetEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling GetEntry");
            if (prefstbl.Count <= 0)
                return null;
            return prefstbl.Find(x => x.Name.Equals(name));
        }
        public bool HasEntry(string name) => (GetEntry(name) != null);

        public MelonPreferences_Entry CreateEntry(string name, string value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_string = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.STRING)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.STRING;
                entry.ValueEdited_string = entry.Value_string = value;
            }
            return entry;
        }
        public MelonPreferences_Entry CreateEntry(string name, bool value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_bool = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.BOOL)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.BOOL;
                entry.ValueEdited_bool = entry.Value_bool = value;
            }
            return entry;
        }
        public MelonPreferences_Entry CreateEntry(string name, int value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_int = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.INT)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.INT;
                entry.ValueEdited_int = entry.Value_int = value;
            }
            return entry;
        }
        public MelonPreferences_Entry CreateEntry(string name, float value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_float = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.FLOAT)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.FLOAT;
                entry.ValueEdited_float = entry.Value_float = value;
            }
            return entry;
        }
        public MelonPreferences_Entry CreateEntry(string name, long value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_long = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.LONG)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.LONG;
                entry.ValueEdited_long = entry.Value_long = value;
            }
            return entry;
        }
        public MelonPreferences_Entry CreateEntry(string name, double value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            entry.DefaultValue_double = value;
            if (entry.Type != MelonPreferences_Entry.TypeEnum.DOUBLE)
            {
                entry.Type = MelonPreferences_Entry.TypeEnum.DOUBLE;
                entry.ValueEdited_double = entry.Value_double = value;
            }
            return entry;
        }

        internal MelonPreferences_Entry LoadEntry(string name, string value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_string = entry.Value_string = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, bool value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_bool = entry.Value_bool = value;
            return entry;
            
        }
        internal MelonPreferences_Entry LoadEntry(string name, int value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_int = entry.Value_int = value;
            return entry;
            
        }
        internal MelonPreferences_Entry LoadEntry(string name, float value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_float = entry.Value_float = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, long value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_long = entry.Value_long = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, double value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling LoadEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return new MelonPreferences_Entry(this, name, value, displayname, hidden);
            entry.ValueEdited_double = entry.Value_double = value;
            return entry;
        }
    }
}