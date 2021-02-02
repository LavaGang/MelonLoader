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
            var entry = GetEntry<T>(identifier);
            if (entry != null)
                throw new Exception($"Calling CreateEntry for { identifier } when it Already Exists");

            entry = new MelonPreferences_Entry<T>
            {
                Identifier = identifier,
                DisplayName = displayname,
                IsHidden = ishidden,
                Category = this,
                DefaultValue = value,
                Value = value
            };
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

        public MelonPreferences_Entry<T> GetEntry<T>(string identifier)
        {
            return (MelonPreferences_Entry<T>) GetEntry(identifier);
        }
        public bool HasEntry(string identifier) => GetEntry(identifier) != null;
    }
}
