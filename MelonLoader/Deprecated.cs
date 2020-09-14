using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#pragma warning disable 0108

namespace MelonLoader
{
    [Obsolete("Imports is obsolete.")]
    public static class Imports
    {
        [Obsolete("Imports.GetCompanyName is obsolete. Please use MelonUtils.GetGameDeveloper instead.")]
        public static string GetCompanyName() => MelonUtils.GetGameDeveloper();
        [Obsolete("Imports.GetProductName is obsolete. Please use MelonUtils.GetGameName instead.")]
        public static string GetProductName() => MelonUtils.GetGameName();
        [Obsolete("Imports.GetGameDirectory is obsolete. Please use MelonUtils.GetGameDirectory instead.")]
        public static string GetGameDirectory() => MelonUtils.GetGameDirectory();
        [Obsolete("Imports.GetGameDataDirectory is obsolete. Please use MelonUtils.GetGameDataDirectory instead.")]
        public static string GetGameDataDirectory() => MelonUtils.GetGameDataDirectory();
        [Obsolete("Imports.GetAssemblyDirectory is obsolete. Please use MelonUtils.GetManagedDirectory instead.")]
        public static string GetAssemblyDirectory() => MelonUtils.GetManagedDirectory();
        [Obsolete("Imports.IsIl2CppGame is obsolete. Please use MelonUtils.IsGameIl2Cpp instead.")]
        public static bool IsIl2CppGame() => MelonUtils.IsGameIl2Cpp();
        [Obsolete("Imports.IsDebugMode is obsolete. Please use MelonDebug.IsEnabled instead.")]
        public static bool IsDebugMode() => MelonDebug.IsEnabled();
        [Obsolete("Imports.Hook is obsolete. Please use MelonUtils.NativeHookAttach instead.")]
        public static void Hook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookAttach(target, detour);
        [Obsolete("Imports.Unhook is obsolete. Please use MelonUtils.NativeHookDetach instead.")]
        public static void Unhook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookDetach(target, detour);
    }
    [Obsolete("MelonLoaderBase is obsolete.")]
    public static class MelonLoaderBase
    {
        [Obsolete("MelonLoaderBase.IsVRChat is obsolete. Please use MelonUtils.IsVRChat instead.")]
        public static bool IsVRChat { get => MelonUtils.IsVRChat(); }
        [Obsolete("MelonLoaderBase.IsBoneworks is obsolete. Please use MelonUtils.IsBoneworks instead.")]
        public static bool IsBoneworks { get => MelonUtils.IsBoneworks(); }
        [Obsolete("MelonLoaderBase.UserDataPath is obsolete. Please use MelonUtils.GetUserDataDirectory instead.")]
        public static string UserDataPath { get => MelonUtils.GetUserDataDirectory(); }
        [Obsolete("MelonLoaderBase.UnityVersion is obsolete. Please use MelonUtils.GetUnityVersion instead.")]
        public static string UnityVersion { get => MelonUtils.GetUnityVersion(); }
    }
    [Obsolete("Main is obsolete.")]
    public static class Main
    {
        [Obsolete("Main.Mods is obsolete. Please use MelonHandler.Mods instead.")]
        public static List<MelonMod> Mods = null;
        [Obsolete("Main.Plugins is obsolete. Please use MelonHandler.Plugins instead.")]
        public static List<MelonPlugin> Plugins = null;
        [Obsolete("Main.IsVRChat is obsolete. Please use MelonUtils.IsVRChat instead.")]
        public static bool IsVRChat = false;
        [Obsolete("Main.IsBoneworks is obsolete. Please use MelonUtils.IsBoneworks instead.")]
        public static bool IsBoneworks = false;
        [Obsolete("Main.GetUnityVersion is obsolete. Please use MelonUtils.GetUnityVersion instead.")]
        public static string GetUnityVersion() => MelonUtils.GetUnityVersion();
        [Obsolete("Main.GetUserDataPath is obsolete. Please use MelonUtils.GetUserDataDirectory instead.")]
        public static string GetUserDataPath() => MelonUtils.GetUserDataDirectory();
        internal static void LegacySupport()
        {
            Mods = MelonHandler.Mods;
            Plugins = MelonHandler.Plugins;
            IsVRChat = MelonUtils.IsVRChat();
            IsBoneworks = MelonUtils.IsBoneworks();
        }
    }
    [Obsolete("MelonConsole is obsolete.")]
    public class MelonConsole
    {
        [Obsolete("MelonConsole.SetTitle is obsolete. Please use MelonUtils.SetConsoleTitle instead.")]
        public static void SetTitle(string title) => MelonUtils.SetConsoleTitle(title);
    }
    [Obsolete("MelonModGame is obsolete. Please use MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonModGameAttribute : Attribute
    {
        public string Developer { get; }
        public string GameName { get; }
        public MelonModGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        internal MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
    [Obsolete("MelonModInfo is obsolete. Please use MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonModInfoAttribute : Attribute
    {
        public Type SystemType { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string DownloadLink { get; }

        public MelonModInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
        internal MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
    [Obsolete("MelonPluginGame is obsolete. Please use MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonPluginGameAttribute : Attribute
    {
        public string Developer { get; }
        public string GameName { get; }
        public MelonPluginGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        public MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
    [Obsolete("MelonPluginInfo is obsolete. Please use MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPluginInfoAttribute : Attribute
    {
        public Type SystemType { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string DownloadLink { get; }

        public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
        public MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
    [Obsolete("MelonModLogger is obsolete. Please use MelonLogger instead.")]
    public class MelonModLogger : MelonLogger {}
    [Obsolete("MelonPrefs is obsolete. Please use MelonPreferences instead.")]
    public class MelonPrefs
    {
        private static string ConfigFileName = "modprefs.ini";
        private static IniFile ConfigFile = null;
        private static Dictionary<string, Dictionary<string, MelonPreference>> prefs = new Dictionary<string, Dictionary<string, MelonPreference>>();
        private static Dictionary<string, string> categoryDisplayNames = new Dictionary<string, string>();
        internal static void Setup() { if (ConfigFile == null) ConfigFile = new IniFile(Path.Combine(Core.UserDataPath, ConfigFileName)); }
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
                    if (ConfigFile.HasKey(section, name))
                        toStoreValue = ConfigFile.GetString(section, name, defaultValue);
                    else ConfigFile.SetString(section, name, defaultValue);
                    prefsInSection.Add(name, new MelonPreference(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                }
            }
            else
            {
                Dictionary<string, MelonPreference> dic = new Dictionary<string, MelonPreference>();
                string toStoreValue = defaultValue;
                if (ConfigFile.HasKey(section, name))
                    toStoreValue = ConfigFile.GetString(section, name, defaultValue);
                else ConfigFile.SetString(section, name, defaultValue);
                dic.Add(name, new MelonPreference(toStoreValue, type, hideFromList, (displayText ?? "") == "" ? name : displayText));
                prefs.Add(section, dic);
            }
        }

        public static bool HasKey(string section, string name) { return prefs.TryGetValue(section, out Dictionary<string, MelonPreference> prefsInSection) && prefsInSection.ContainsKey(name); }
        public static Dictionary<string, Dictionary<string, MelonPreference>> GetPreferences() { return prefs; }
        public static string GetCategoryDisplayName(string key) { if (categoryDisplayNames.TryGetValue(key, out string name)) return name; return key; }

        public static void SaveConfig()
        {
            SaveConfigToTable();
            MelonHandler.OnPreferencesApplied();
        }

        internal static void SaveConfigToTable()
        {
            foreach (KeyValuePair<string, Dictionary<string, MelonPreference>> prefsInSection in prefs)
            {
                foreach (KeyValuePair<string, MelonPreference> pref in prefsInSection.Value)
                {
                    pref.Value.Value = pref.Value.ValueEdited;
                    ConfigFile.SetString(prefsInSection.Key, pref.Key, pref.Value.Value);
                }
            }
            MelonLogger.Log("Legacy Config Saved!");
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
                ConfigFile.SetString(section, name, value);
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
        }
    }
    [Obsolete("ModPrefs is obsolete. Please use MelonPreferences instead.")]
    public class ModPrefs : MelonPrefs
    {
        public static Dictionary<string, Dictionary<string, PrefDesc>> GetPrefs()
        {
            Dictionary<string, Dictionary<string, PrefDesc>> output = new Dictionary<string, Dictionary<string, PrefDesc>>();
            Dictionary<string, Dictionary<string, MelonPreference>> prefs = GetPreferences();
            for (int i = 0; i < prefs.Values.Count; i++)
            {
                Dictionary<string, MelonPreference> prefsdict = prefs.Values.ElementAt(i);
                Dictionary<string, PrefDesc> newprefsdict = new Dictionary<string, PrefDesc>();
                for (int j = 0; j < prefsdict.Values.Count; j++)
                {
                    MelonPreference pref = prefsdict.Values.ElementAt(j);
                    PrefDesc newpref = new PrefDesc(pref.Value, (PrefType)pref.Type, pref.Hidden, pref.DisplayText);
                    newpref.ValueEdited = pref.ValueEdited;
                    newprefsdict.Add(prefsdict.Keys.ElementAt(j), newpref);
                }
                output.Add(prefs.Keys.ElementAt(i), newprefsdict);
            }
            return output;
        }
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => RegisterString(section, name, defaultValue, displayText, hideFromList);
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => RegisterBool(section, name, defaultValue, displayText, hideFromList);
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => RegisterInt(section, name, defaultValue, displayText, hideFromList);
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => RegisterFloat(section, name, defaultValue, displayText, hideFromList);
        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        public class PrefDesc : MelonPreference
        {
            public PrefType Type { get => (PrefType)base.Type; }
            public PrefDesc(string value, PrefType type, bool hidden, string displayText) : base(value, (MelonPreferenceType)type, hidden, displayText)
            {
                Value = value;
                ValueEdited = value;
                base.Type = (MelonPreferenceType)type;
                Hidden = hidden;
                DisplayText = displayText;
            }
        }
    }
}