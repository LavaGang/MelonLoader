using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public class MelonPreferences_Category
    {
        public string Identifier { get; internal set; }
        public string DisplayName { get; internal set; }
        public readonly List<MelonPreferences_Entry> Entries = new List<MelonPreferences_Entry>();

        internal MelonPreferences_Category(string identifier, string display_name)
        {
            Identifier = identifier;
            DisplayName = display_name;
            MelonPreferences.Categories.Add(this);
        }

        public MelonPreferences_Entry CreateEntry<T>(string identifier, T default_value, string display_name = null, bool is_hidden = false)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new Exception("identifier is null or empty when calling CreateEntry");
            if (display_name == null)
                display_name = identifier;
            var entry = GetEntry<T>(identifier);
            if (entry != null)
                throw new Exception($"Calling CreateEntry for { display_name } when it Already Exists");
            entry = new MelonPreferences_Entry<T>
            {
                Identifier = identifier,
                DisplayName = display_name,
                IsHidden = is_hidden,
                Category = this,
                DefaultValue = default_value,
                Value = default_value
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
