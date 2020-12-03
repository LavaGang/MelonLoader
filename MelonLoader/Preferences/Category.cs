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

        public MelonPreferences_Entry CreateEntry<T>(string name, T value, string displayname = null, bool hidden = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(name);
            if (entry == null)
                return Preferences.TypeManager.ConstructEntry(this, name, value, displayname, hidden);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            Preferences.TypeManager.ConvertCurrentValueType(entry, value);
            return entry;
        }
    }
}