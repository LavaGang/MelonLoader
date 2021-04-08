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
#if PORT_DISABLE
            MelonPreferences.categorytbl.Add(this);
#endif
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
                entry = Preferences.TypeManager.ConstructEntry(this, name, value, displayname, hidden);
            if (entry == null)
                throw new Exception("Failed to Create Entry of Unsupported Type: " + typeof(T).FullName);
            entry.DisplayName = displayname;
            entry.Hidden = hidden;
            Preferences.TypeManager.ConvertCurrentValueType(entry, value);
#if PORT_DISABLE
            if (MelonPreferences.SaveAfterEntryCreation)
                MelonPreferences.Save_Internal();
#endif
            return entry;
        }
    }
}