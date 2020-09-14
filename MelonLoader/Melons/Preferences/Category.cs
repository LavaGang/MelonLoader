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

        public MelonPreferences_Entry CreateEntry(string name, string value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry != null)
            {
                entry.DisplayName = displayname;
                entry.DefaultValue_string = value;
                return entry;
            }
            return new MelonPreferences_Entry(this, name, value, displayname, hidden);
        }
        public MelonPreferences_Entry CreateEntry(string name, bool value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry != null)
            {
                entry.DisplayName = displayname;
                entry.DefaultValue_bool = value;
                return entry;
            }
            return new MelonPreferences_Entry(this, name, value, displayname, hidden);
        }
        public MelonPreferences_Entry CreateEntry(string name, int value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry != null)
            {
                entry.DisplayName = displayname;
                entry.DefaultValue_int = value;
                return entry;
            }
            return new MelonPreferences_Entry(this, name, value, displayname, hidden);
        }
        public MelonPreferences_Entry CreateEntry(string name, float value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry != null)
            {
                entry.DisplayName = displayname;
                entry.DefaultValue_float = value;
                return entry;
            }
            return new MelonPreferences_Entry(this, name, value, displayname, hidden);
        }

        internal MelonPreferences_Entry LoadEntry(string name, string value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = CreateEntry(name, value, displayname, hidden);
            entry.Value_string = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, bool value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = CreateEntry(name, value, displayname, hidden);
            entry.Value_bool = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, int value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = CreateEntry(name, value, displayname, hidden);
            entry.Value_int = value;
            return entry;
        }
        internal MelonPreferences_Entry LoadEntry(string name, float value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = CreateEntry(name, value, displayname, hidden);
            entry.Value_float = value;
            return entry;
        }
    }
}
