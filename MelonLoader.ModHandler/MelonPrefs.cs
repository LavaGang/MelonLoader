using System;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    internal static class MelonPrefsHandler
    {
        private static string filename = "modprefs.ini";
        private static IniFile _instance;
        private static IniFile Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IniFile(Path.Combine(Main.GetUserDataPath(), filename));
                return _instance;
            }
        }

        internal static bool HasKey(string section, string name) { return Instance.IniReadValue(section, name) != null; }

        internal static string GetString(string section, string name, string defaultValue = "", bool autoSave = false)
        {
            string value = Instance.IniReadValue(section, name);
            if (!string.IsNullOrEmpty(value))
                return value;
            else if (autoSave)
                SetString(section, name, defaultValue);
            return defaultValue;
        }
        internal static void SetString(string section, string name, string value) { Instance.IniWriteValue(section, name, value.Trim()); }

        internal static int GetInt(string section, string name, int defaultValue = 0, bool autoSave = false)
        {
            int value;
            if (int.TryParse(Instance.IniReadValue(section, name), out value))
                return value;
            else if (autoSave)
                SetInt(section, name, defaultValue);
            return defaultValue;
        }
        internal static void SetInt(string section, string name, int value) { Instance.IniWriteValue(section, name, value.ToString()); }

        internal static float GetFloat(string section, string name, float defaultValue = 0f, bool autoSave = false)
        {
            float value;
            if (float.TryParse(Instance.IniReadValue(section, name), out value))
                return value;
            else if (autoSave)
                SetFloat(section, name, defaultValue);
            return defaultValue;
        }
        internal static void SetFloat(string section, string name, float value) { Instance.IniWriteValue(section, name, value.ToString()); }

        internal static bool GetBool(string section, string name, bool defaultValue = false, bool autoSave = false)
        {
            string sVal = GetString(section, name, null);
            if ("true".Equals(sVal) || "1".Equals(sVal) || "0".Equals(sVal) || "false".Equals(sVal))
                return ("true".Equals(sVal) || "1".Equals(sVal));
            else if (autoSave)
                SetBool(section, name, defaultValue);
            return defaultValue;
        }
        internal static void SetBool(string section, string name, bool value) { Instance.IniWriteValue(section, name, value ? "true" : "false"); }
    }

    public static class MelonPrefs
    {
        private static Dictionary<string, Dictionary<string, MelonPreference>> prefs = new Dictionary<string, Dictionary<string, MelonPreference>>();
        private static Dictionary<string, string> categoryDisplayNames = new Dictionary<string, string>();

        public static void RegisterCategory(string name, string displayText) { categoryDisplayNames[name] = displayText; }
        public static void RegisterString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) { Register(section, name, defaultValue, displayText, MelonPreferenceType.STRING, hideFromList); }
        public static void RegisterBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) { Register(section, name, defaultValue ? "true" : "false", displayText, MelonPreferenceType.BOOL, hideFromList); }
        public static void RegisterInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) { Register(section, name, "" + defaultValue, displayText, MelonPreferenceType.INT, hideFromList); }
        public static void RegisterFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) { Register(section, name, "" + defaultValue, displayText, MelonPreferenceType.FLOAT, hideFromList); }
        private static void Register(string section, string name, string defaultValue, string displayText, MelonPreferenceType type, bool hideFromList)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection))
            {
                if (prefsInSection.TryGetValue(name, out MelonPreference pref))
                    MelonLogger.LogError("Trying to registered Pref " + section + ":" + name + " more than one time");
                else
                {
                    string toStoreValue = defaultValue;
                    if (MelonPrefsHandler.HasKey(section, name))
                        toStoreValue = MelonPrefsHandler.GetString(section, name, defaultValue);
                    else MelonPrefsHandler.SetString(section, name, defaultValue);
                    prefsInSection.Add(name, new MelonPreference(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                }
            }
            else
            {
                Dictionary<string, MelonPreference> dic = new Dictionary<string, MelonPreference>();
                string toStoreValue = defaultValue;
                if (MelonPrefsHandler.HasKey(section, name))
                    toStoreValue = MelonPrefsHandler.GetString(section, name, defaultValue);
                else MelonPrefsHandler.SetString(section, name, defaultValue);
                dic.Add(name, new MelonPreference(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                prefs.Add(section, dic);
            }
        }

        public static bool HasKey(string section, string name) { return prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.ContainsKey(name); }
        public static Dictionary<string, Dictionary<string, MelonPreference>> GetPreferences() { return prefs; }
        public static string GetCategoryDisplayName(string key) { if (categoryDisplayNames.TryGetValue(key, out string name)) return name; return key; }

        public static void SaveConfig()
        {
            foreach (KeyValuePair<string, Dictionary<string, MelonPreference>> prefsInSection in prefs)
            {
                foreach (KeyValuePair<string, MelonPreference> pref in prefsInSection.Value)
                {
                    pref.Value.Value = pref.Value.ValueEdited;
                    MelonPrefsHandler.SetString(prefsInSection.Key, pref.Key, pref.Value.Value);
                }
            }
            Main.OnModSettingsApplied();
            MelonLogger.Log("Config Saved!");
        }

        public static string GetString(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.TryGetValue(name, out MelonPreference pref))
                return pref.Value;
            MelonLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return "";
        }

        public static void SetString(string section, string name, string value)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.TryGetValue(name, out MelonPreference pref))
            {
                pref.Value = pref.ValueEdited = value;
                MelonPrefsHandler.SetString(section, name, value);
            }
            else
                MelonLogger.LogError("Trying to save unknown pref " + section + ":" + name);
        }

        public static bool GetBool(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.TryGetValue(name, out MelonPreference pref))
                return (pref.Value.Equals("true") || pref.Value.Equals("1"));
            MelonLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return false;
        }
        public static void SetBool(string section, string name, bool value) { SetString(section, name, value ? "true" : "false"); }

        public static int GetInt(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.TryGetValue(name, out MelonPreference pref))
                if (int.TryParse(pref.Value, out int valueI))
                    return valueI;
            MelonLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return 0;
        }
        public static void SetInt(string section, string name, int value) { SetString(section, name, value.ToString()); }

        public static float GetFloat(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.TryGetValue(name, out MelonPreference pref))
                if (float.TryParse(pref.Value, out float valueF))
                    return valueF;
            MelonLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return 0.0f;
        }
        public static void SetFloat(string section, string name, float value) { SetString(section, name, value.ToString()); }

        public enum MelonPreferenceType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }

        public class MelonPreference
        {
            public string Value { get; set; }
            public string ValueEdited { get; set; }
            public MelonPreferenceType Type { get; internal set; }
            public bool Hidden { get; internal set; }
            public String DisplayText { get; internal set; }

            public MelonPreference(string value, MelonPreferenceType type, bool hidden, string displayText)
            {
                Value = value;
                ValueEdited = value;
                Type = type;
                Hidden = hidden;
                DisplayText = displayText;
            }

            [Obsolete()]
            public MelonPreference(string value, ModPrefs.PrefType type, bool hidden, string displayText)
            {
                Value = value;
                ValueEdited = value;
                Type = (MelonPreferenceType)type;
                Hidden = hidden;
                DisplayText = displayText;
            }
        }
    }
}