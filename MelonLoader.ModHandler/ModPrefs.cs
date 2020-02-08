using System;
using System.Collections.Generic;
using System.IO;
#pragma warning disable CS0618

namespace MelonLoader
{
    internal static class ModPrefsController
    {
        private static string filename = "modprefs.ini";
        private static IniFile _instance;
        private static IniFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
                    if (!Directory.Exists(userDataDir)) Directory.CreateDirectory(userDataDir);
                    _instance = new IniFile(Path.Combine(userDataDir, filename));
                }
                return _instance;
            }
        }

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
            if ("true".Equals(sVal) || "false".Equals(sVal))
                return "true".Equals(sVal);
            else if (autoSave)
                SetBool(section, name, defaultValue);
            return defaultValue;
        }
        internal static void SetBool(string section, string name, bool value) { Instance.IniWriteValue(section, name, value ? "true" : "false"); }

        internal static bool HasKey(string section, string name) { return Instance.IniReadValue(section, name) != null; }
    }

    public static class ModPrefs
    {
        private static Dictionary<string, Dictionary<string, PrefDesc>> prefs = new Dictionary<string, Dictionary<string, PrefDesc>>();
        private static Dictionary<string, string> categoryDisplayNames = new Dictionary<string, string>();
        public static void RegisterCategory(string name, string displayText) { categoryDisplayNames[name] = displayText; }
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(section, name, defaultValue, displayText, PrefType.STRING, hideFromList); }
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(section, name, defaultValue ? "true" : "false", displayText, PrefType.BOOL, hideFromList); }
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(section, name, "" + defaultValue, displayText, PrefType.INT, hideFromList); }
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(section, name, "" + defaultValue, displayText, PrefType.FLOAT, hideFromList); }

        private static void RegisterPref(string section, string name, string defaultValue, string displayText, PrefType type, bool hideFromList)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection))
            {
                if (prefsInSection.TryGetValue(name, out PrefDesc pref))
                    MelonModLogger.LogError("Trying to registered Pref " + section + ":" + name + " more than one time");
                else
                {
                    string toStoreValue = defaultValue;
                    if (ModPrefsController.HasKey(section, name))
                        toStoreValue = ModPrefsController.GetString(section, name, defaultValue);
                    else ModPrefsController.SetString(section, name, defaultValue);
                    prefsInSection.Add(name, new PrefDesc(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                }
            }
            else
            {
                Dictionary<string, PrefDesc> dic = new Dictionary<string, PrefDesc>();
                string toStoreValue = defaultValue;
                if (ModPrefsController.HasKey(section, name))
                    toStoreValue = ModPrefsController.GetString(section, name, defaultValue);
                else ModPrefsController.SetString(section, name, defaultValue);
                dic.Add(name, new PrefDesc(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                prefs.Add(section, dic);
            }
        }

        public static bool HasKey(string section, string name) { return prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.ContainsKey(name); }
        public static Dictionary<string, Dictionary<string, PrefDesc>> GetPrefs() { return prefs; }
        public static string GetCategoryDisplayName(string key) { if (categoryDisplayNames.TryGetValue(key, out string name)) return name; return key; }

        public static void SaveConfig()
        {
            foreach (KeyValuePair<string, Dictionary<string, PrefDesc>> prefsInSection in prefs)
            {
                foreach (KeyValuePair<string, PrefDesc> pref in prefsInSection.Value)
                {
                    pref.Value.Value = pref.Value.ValueEdited;
                    ModPrefsController.SetString(prefsInSection.Key, pref.Key, pref.Value.Value);
                }
            }
            Main.OnModSettingsApplied();
            MelonModLogger.Log("Config Saved!");
        }

        public static string GetString(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                return pref.Value;
            MelonModLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return "";
        }

        public static void SetString(string section, string name, string value)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
            {
                pref.Value = value;
                ModPrefsController.SetString(section, name, value);
            }
            else
                MelonModLogger.LogError("Trying to save unknown pref " + section + ":" + name);
        }

        public static bool GetBool(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                return pref.Value.Equals("true");
            MelonModLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return false;
        }
        public static void SetBool(string section, string name, bool value) { SetString(section, name, value ? "true" : "false"); }

        public static int GetInt(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                if (int.TryParse(pref.Value, out int valueI))
                    return valueI;
            MelonModLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return 0;
        }
        public static void SetInt(string section, string name, int value) { SetString(section, name, value.ToString()); }

        public static float GetFloat(string section, string name)
        {
            if (prefs.TryGetValue(section, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                if (float.TryParse(pref.Value, out float valueF))
                    return valueF;
            MelonModLogger.LogError("Trying to get unregistered Pref " + section + ":" + name);
            return 0.0f;
        }
        public static void SetFloat(string section, string name, float value) { SetString(section, name, value.ToString()); }

        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }

        public class PrefDesc
        {
            public string Value { get; set; }
            public string ValueEdited { get; set; }
            public PrefType Type { get; private set; }
            public bool Hidden { get; private set; }
            public String DisplayText { get; private set; }

            public PrefDesc(string value, PrefType type, bool hidden, string displayText)
            {
                Value = value;
                ValueEdited = value;
                Type = type;
                Hidden = hidden;
                DisplayText = displayText;
            }
        }
    }
}