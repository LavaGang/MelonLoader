using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader.TinyJSON;
#pragma warning disable CS0618

namespace MelonLoader
{
    internal static class LegacyModPrefsConverter
    {
        private static IniFile _instance;
        private static IniFile Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IniFile(Path.Combine(Main.GetUserDataPath(), "modprefs.ini"));
                return _instance;
            }
        }
    }

    public static class ModPrefs
    {
        private static string FileName = "ModPrefs.json";
        private static Dictionary<string, Dictionary<string, PrefDesc>> prefs = new Dictionary<string, Dictionary<string, PrefDesc>>();
        private static Dictionary<string, string> categoryDisplayNames = new Dictionary<string, string>();

        public static void LoadConfig()
        {
            string filepath = Path.Combine(Main.GetUserDataPath(), FileName);
            if (File.Exists(filepath))
                prefs = Decoder.Decode(File.ReadAllText(filepath)).Make<Dictionary<string, Dictionary<string, PrefDesc>>>();
        }

        public static void SaveConfig()
        {
            File.WriteAllText(Path.Combine(Main.GetUserDataPath(), FileName), Encoder.Encode(prefs, EncodeOptions.NoTypeHints | EncodeOptions.IncludePublicProperties | EncodeOptions.PrettyPrint));
            Main.OnModSettingsApplied();
            MelonModLogger.Log("Config Saved!");
        }

        public static void RegisterCategory(string name, string displayText) { categoryDisplayNames[name] = displayText; }
        public static void RegisterPrefString(string category_name, string name, string defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(category_name, name, defaultValue, displayText, PrefType.STRING, hideFromList); }
        public static void RegisterPrefBool(string category_name, string name, bool defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(category_name, name, defaultValue ? "true" : "false", displayText, PrefType.BOOL, hideFromList); }
        public static void RegisterPrefInt(string category_name, string name, int defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(category_name, name, "" + defaultValue, displayText, PrefType.INT, hideFromList); }
        public static void RegisterPrefFloat(string category_name, string name, float defaultValue, string displayText = null, bool hideFromList = false) { RegisterPref(category_name, name, "" + defaultValue, displayText, PrefType.FLOAT, hideFromList); }

        private static void RegisterPref(string category_name, string name, string defaultValue, string displayText, PrefType type, bool hideFromList)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection))
            {
                if (!prefsInSection.TryGetValue(name, out PrefDesc pref))
                    prefsInSection.Add(name, new PrefDesc(defaultValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
            }
            else
            {
                Dictionary<string, PrefDesc> dic = new Dictionary<string, PrefDesc>();
                dic.Add(name, new PrefDesc(defaultValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                prefs.Add(category_name, dic);
            }
        }

        public static bool HasKey(string category_name, string name) { return prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.ContainsKey(name); }
        public static Dictionary<string, Dictionary<string, PrefDesc>> GetPrefs() { return prefs; }
        public static string GetCategoryDisplayName(string key) { if (categoryDisplayNames.TryGetValue(key, out string name)) return name; return key; }

        public static string GetString(string category_name, string name)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                return pref.Value;
            MelonModLogger.LogError("Trying to get unregistered Pref " + category_name + ":" + name);
            return "";
        }

        public static void SetString(string category_name, string name, string value)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                pref.Value = value;
            else
                MelonModLogger.LogError("Trying to save unknown pref " + category_name + ":" + name);
        }

        public static bool GetBool(string category_name, string name)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                return (pref.Value.Equals("true") || pref.Value.Equals("1"));
            MelonModLogger.LogError("Trying to get unregistered Pref " + category_name + ":" + name);
            return false;
        }
        public static void SetBool(string category_name, string name, bool value) { SetString(category_name, name, value ? "true" : "false"); }

        public static int GetInt(string category_name, string name)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                if (int.TryParse(pref.Value, out int valueI))
                    return valueI;
            MelonModLogger.LogError("Trying to get unregistered Pref " + category_name + ":" + name);
            return 0;
        }
        public static void SetInt(string category_name, string name, int value) { SetString(category_name, name, value.ToString()); }

        public static float GetFloat(string category_name, string name)
        {
            if (prefs.TryGetValue(category_name, out Dictionary<string, PrefDesc> prefsInSection) && prefsInSection.TryGetValue(name, out PrefDesc pref))
                if (float.TryParse(pref.Value, out float valueF))
                    return valueF;
            MelonModLogger.LogError("Trying to get unregistered Pref " + category_name + ":" + name);
            return 0.0f;
        }
        public static void SetFloat(string category_name, string name, float value) { SetString(category_name, name, value.ToString()); }

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