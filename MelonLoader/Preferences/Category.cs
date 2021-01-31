using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public readonly List<MelonPreferences_Entry> Entries = new List<MelonPreferences_Entry>();

        internal MelonPreferences_Category(string identifier, string displayname)
        {
            Identifier = identifier;
            DisplayName = displayname;
            MelonPreferences.Categories.Add(this);
        }

        public MelonPreferences_Entry CreateEntry<T>(string identifier, T value, string displayname = null, bool ishidden = false)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("Name is null or empty when calling CreateEntry");
            MelonPreferences_Entry entry = GetEntry(identifier);
            if (entry != null)
                throw new Exception($"Calling CreateEntry for { identifier } when it Already Exists");
            MelonPreferences_Entry.ResolveEventArgs args = new MelonPreferences_Entry.ResolveEventArgs { ReflectedType = typeof(T) };
            MelonPreferences.InvokeEntryTypeResolveEvents(args);
            if (args.Entry == null)
                throw new Exception($"Calling CreateEntry for { identifier } with Unknown Type");
            entry = args.Entry;
            entry.Identifier = identifier;
            entry.DisplayName = displayname;
            entry.IsHidden = ishidden;
            entry.Category = this;
            entry.SetDefaultValue(value);
            entry.ResetToDefault();
            Preferences.IO.File.SetupEntryFromRawValue(entry);
            Entries.Add(entry);
            return entry;
        }

        public MelonPreferences_Entry GetEntry(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier cannot be null or empty when calling GetEntry");
            if (Entries.Count <= 0)
                return null;
            return Entries.Find(x => x.Identifier.Equals(identifier));
        }
        public bool HasEntry(string identifier) => (GetEntry(identifier) != null);
    }
}
