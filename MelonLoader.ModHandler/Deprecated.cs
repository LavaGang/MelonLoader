using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable 0108

namespace MelonLoader
{
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
    public class MelonModLogger
    {
        public static void Log(string s) => MelonLogger.Native_Log((MelonLogger.GetNameSection() + s));
        public static void Log(ConsoleColor color, string s) => MelonLogger.Native_LogColor((MelonLogger.GetNameSection() + s), color);
        public static void Log(string s, params object[] args) => MelonLogger.Native_Log((MelonLogger.GetNameSection() + string.Format(s, args)));
        public static void Log(ConsoleColor color, string s, params object[] args) => MelonLogger.Native_LogColor((MelonLogger.GetNameSection() + string.Format(s, args)), color);
        public static void LogWarning(string s) => MelonLogger.Native_LogWarning(MelonLogger.GetNameSection(), s);
        public static void LogWarning(string s, params object[] args) => MelonLogger.Native_LogWarning(MelonLogger.GetNameSection(), string.Format(s, args));
        public static void LogError(string s) => MelonLogger.Native_LogError(MelonLogger.GetNameSection(), s);
        public static void LogError(string s, params object[] args) => MelonLogger.Native_LogError(MelonLogger.GetNameSection(), string.Format(s, args));
    }
    [Obsolete("ModPrefs is obsolete. Please use MelonPrefs instead.")]
    public class ModPrefs
    {
        public static Dictionary<string, Dictionary<string, PrefDesc>> GetPrefs()
        {
            Dictionary<string, Dictionary<string, PrefDesc>> output = new Dictionary<string, Dictionary<string, PrefDesc>>();
            Dictionary<string, Dictionary<string, MelonPrefs.MelonPreference>> prefs = MelonPrefs.GetPreferences();
            for (int i = 0; i < prefs.Values.Count; i++)
            {
                Dictionary<string, MelonPrefs.MelonPreference> prefsdict = prefs.Values.ElementAt(i);
                Dictionary<string, PrefDesc> newprefsdict = new Dictionary<string, PrefDesc>();
                for (int j = 0; j < prefsdict.Values.Count; j++)
                {
                    MelonPrefs.MelonPreference pref = prefsdict.Values.ElementAt(j);
                    PrefDesc newpref = new PrefDesc(pref.Value, (PrefType)pref.Type, pref.Hidden, pref.DisplayText);
                    newpref.ValueEdited = pref.ValueEdited;
                    newprefsdict.Add(prefsdict.Keys.ElementAt(j), newpref);
                }
                output.Add(prefs.Keys.ElementAt(i), newprefsdict);
            }
            return output;
        }
        public static void RegisterCategory(string name, string displayText) => MelonPrefs.RegisterCategory(name, displayText);
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => MelonPrefs.RegisterString(section, name, defaultValue, displayText, hideFromList);
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => MelonPrefs.RegisterBool(section, name, defaultValue, displayText);
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => MelonPrefs.RegisterInt(section, name, defaultValue, displayText, hideFromList);
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => MelonPrefs.RegisterFloat(section, name, defaultValue, displayText, hideFromList);
        public static bool HasKey(string section, string name) => MelonPrefs.HasKey(section, name);
        public static string GetCategoryDisplayName(string key) => MelonPrefs.GetCategoryDisplayName(key);
        public static void SaveConfig() => MelonPrefs.SaveConfig();
        public static string GetString(string section, string name) => MelonPrefs.GetString(section, name);
        public static void SetString(string section, string name, string value) => MelonPrefs.SetString(section, name, value);
        public static bool GetBool(string section, string name) => MelonPrefs.GetBool(section, name);
        public static void SetBool(string section, string name, bool value) => MelonPrefs.SetBool(section, name, value);
        public static int GetInt(string section, string name) => MelonPrefs.GetInt(section, name);
        public static void SetInt(string section, string name, int value) => MelonPrefs.SetInt(section, name, value);
        public static float GetFloat(string section, string name) => MelonPrefs.GetFloat(section, name);
        public static void SetFloat(string section, string name, float value) => MelonPrefs.SetFloat(section, name, value);
        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        public class PrefDesc : MelonPrefs.MelonPreference
        {
            public PrefType Type { get => (PrefType)base.Type; }
            public PrefDesc(string value, PrefType type, bool hidden, string displayText) : base(value, type, hidden, displayText)
            {
                Value = value;
                ValueEdited = value;
                base.Type = (MelonPrefs.MelonPreferenceType)type;
                Hidden = hidden;
                DisplayText = displayText;
            }
        }
    }
 }