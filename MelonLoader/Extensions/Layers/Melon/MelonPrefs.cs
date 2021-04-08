using System;
using System.Collections.Generic;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonPrefs is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences instead.")]
    public class MelonPrefs
    {
        [Obsolete("MelonLoader.MelonPrefs.RegisterCategory is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateCategory instead.")]
        public static void RegisterCategory(string name, string displayText) => MelonPreferences.CreateCategory(name, displayText);
        [Obsolete("MelonLoader.MelonPrefs.RegisterString is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.MelonPrefs.RegisterBool is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.MelonPrefs.RegisterInt is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.MelonPrefs.RegisterFloat is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.MelonPrefs.HasKey is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.HasEntry instead.")]
        public static bool HasKey(string section, string name) => MelonPreferences.HasEntry(section, name);
        [Obsolete("MelonLoader.MelonPrefs.GetPreferences is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.Categories instead.")]
        public static Dictionary<string, Dictionary<string, MelonPreference>> GetPreferences()
        {
            Dictionary<string, Dictionary<string, MelonPreference>> output = new Dictionary<string, Dictionary<string, MelonPreference>>();
            if (MelonPreferences.Categories.Count <= 0)
                return output;
            foreach (MelonPreferences_Category category in MelonPreferences.Categories)
            {
                Dictionary<string, MelonPreference> newprefsdict = new Dictionary<string, MelonPreference>();
                foreach (MelonPreferences_Entry entry in category.Entries)
                {
                    Type reflectedType = entry.GetReflectedType();
                    if ((reflectedType != typeof(string))
                        && (reflectedType != typeof(bool))
                        && (reflectedType != typeof(int))
                        && (reflectedType != typeof(float))
                        && (reflectedType != typeof(double))
                        && (reflectedType != typeof(long)))
                        continue;
                    MelonPreference newpref = new MelonPreference(entry);
                    newprefsdict.Add(entry.Identifier, newpref);
                }
                output.Add(category.Identifier, newprefsdict);
            }
            return output;
        }
        [Obsolete("MelonLoader.MelonPrefs.GetCategoryDisplayName is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.GetCategoryDisplayName instead.")]
        public static string GetCategoryDisplayName(string key) => MelonPreferences.GetCategory(key)?.DisplayName;
        [Obsolete("MelonLoader.MelonPrefs.SaveConfig is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.Save instead.")]
        public static void SaveConfig() => MelonPreferences.Save();
        [Obsolete("MelonLoader.MelonPrefs.GetString is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.GetEntryString instead.")]
        public static string GetString(string section, string name)
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory(section);
            if (category == null)
                return null;
            MelonPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return null;
            return entry.GetValueAsString();
        }
        [Obsolete("MelonLoader.MelonPrefs.SetString is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.SetEntryString instead.")]
        public static void SetString(string section, string name, string value)
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory(section);
            if (category == null)
                return;
            MelonPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return;
            switch (entry)
            {
                case MelonPreferences_Entry<string> stringEntry:
                    stringEntry.Value = value;
                    break;
                case MelonPreferences_Entry<int> intEntry:
                    if (int.TryParse(value, out var parsedInt))
                        intEntry.Value = parsedInt;
                    break;
                case MelonPreferences_Entry<float> floatEntry:
                    if (float.TryParse(value, out var parsedFloat))
                        floatEntry.Value = parsedFloat;
                    break;
                case MelonPreferences_Entry<bool> boolEntry:
                    if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                        boolEntry.Value = value.ToLower().StartsWith("true");
                    break;
            }
        }
        [Obsolete("MelonLoader.MelonPrefs.GetBool is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.GetEntryBool instead.")]
        public static bool GetBool(string section, string name) => MelonPreferences.GetEntryValue<bool>(section, name);
        [Obsolete("MelonLoader.MelonPrefs.SetBool is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.SetEntryBool instead.")]
        public static void SetBool(string section, string name, bool value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonLoader.MelonPrefs.GetInt is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.GetEntryInt instead.")]
        public static int GetInt(string section, string name) => MelonPreferences.GetEntryValue<int>(section, name);
        [Obsolete("MelonLoader.MelonPrefs.SetInt is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.SetEntryInt instead.")]
        public static void SetInt(string section, string name, int value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonLoader.MelonPrefs.GetFloat is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.GetEntryFloat instead.")]
        public static float GetFloat(string section, string name) => MelonPreferences.GetEntryValue<float>(section, name);
        [Obsolete("MelonLoader.MelonPrefs.GetEntryFloat is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.SetEntryFloat instead.")]
        public static void SetFloat(string section, string name, float value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonLoader.MelonPrefs.MelonPreferenceType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.TypeEnum instead.")]
        public enum MelonPreferenceType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("MelonLoader.MelonPrefs.MelonPreference is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry instead.")]
        public class MelonPreference
        {
            [Obsolete("MelonLoader.MelonPrefs.MelonPreference.Value is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.GetValue instead.")]
            public string Value { get => GetString(Entry.Category.Identifier, Entry.Identifier); set => SetString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MelonLoader.MelonPrefs.MelonPreference.ValueEdited is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.GetValueEdited instead.")]
            public string ValueEdited { get => GetEditedString(Entry.Category.Identifier, Entry.Identifier); set => SetEditedString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MelonLoader.MelonPrefs.MelonPreference.Type is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.GetReflectedType instead.")]
            public MelonPreferenceType Type
            {
                get
                {
                    if (Entry.GetReflectedType() == typeof(string))
                        return MelonPreferenceType.STRING;
                    else if (Entry.GetReflectedType() == typeof(bool))
                        return MelonPreferenceType.BOOL;
                    else if ((Entry.GetReflectedType() == typeof(float))
                        || (Entry.GetReflectedType() == typeof(double)))
                        return MelonPreferenceType.FLOAT;
                    else if ((Entry.GetReflectedType() == typeof(int))
                        || (Entry.GetReflectedType() == typeof(long))
                        || (Entry.GetReflectedType() == typeof(byte)))
                        return MelonPreferenceType.INT;
                    return (MelonPreferenceType)4;
                }
            }
            [Obsolete("MelonLoader.MelonPrefs.MelonPreference.Hidden is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.IsHidden instead.")]
            public bool Hidden { get => Entry.IsHidden; }
            [Obsolete("MelonLoader.MelonPrefs.MelonPreference.DisplayText is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.DisplayName instead.")]
            public string DisplayText { get => Entry.DisplayName; }

            internal MelonPreference(MelonPreferences_Entry entry) => Entry = entry;
            internal MelonPreference(MelonPreference pref) => Entry = pref.Entry;
            private MelonPreferences_Entry Entry = null;
            private static string GetEditedString(string section, string name)
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(section);
                if (category == null)
                    return null;
                MelonPreferences_Entry entry = category.GetEntry(name);
                if (entry == null)
                    return null;

                return entry.GetEditedValueAsString();
            }
            private static void SetEditedString(string section, string name, string value)
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(section);
                if (category == null)
                    return;
                MelonPreferences_Entry entry = category.GetEntry(name);
                if (entry == null)
                    return;
                switch (entry)
                {
                    case MelonPreferences_Entry<string> stringEntry:
                        stringEntry.EditedValue = value;
                        break;
                    case MelonPreferences_Entry<int> intEntry:
                        if (int.TryParse(value, out var parsedInt))
                            intEntry.EditedValue = parsedInt;
                        break;
                    case MelonPreferences_Entry<float> floatEntry:
                        if (float.TryParse(value, out var parsedFloat))
                            floatEntry.EditedValue = parsedFloat;
                        break;
                    case MelonPreferences_Entry<bool> boolEntry:
                        if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                            boolEntry.EditedValue = value.ToLower().StartsWith("true");
                        break;
                }
            }
        }
    }
}