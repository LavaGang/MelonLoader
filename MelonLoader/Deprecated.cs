using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#pragma warning disable 0108

namespace MelonLoader
{
    [Obsolete("Imports is obsolete.")]
    public static class Imports
    {
        [Obsolete("Imports.GetCompanyName is obsolete. Please use MelonUtils.GetGameDeveloper instead.")]
        public static string GetCompanyName() => MelonUtils.GameDeveloper;
        [Obsolete("Imports.GetProductName is obsolete. Please use MelonUtils.GetGameName instead.")]
        public static string GetProductName() => MelonUtils.GameName;
        [Obsolete("Imports.GetGameDirectory is obsolete. Please use MelonUtils.GetGameDirectory instead.")]
        public static string GetGameDirectory() => MelonUtils.GameDirectory;
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
        public static bool IsVRChat { get => MelonUtils.IsVRChat; }
        [Obsolete("MelonLoaderBase.IsBoneworks is obsolete. Please use MelonUtils.IsBONEWORKS instead.")]
        public static bool IsBoneworks { get => MelonUtils.IsBONEWORKS; }
        [Obsolete("MelonLoaderBase.UserDataPath is obsolete. Please use MelonUtils.GetUserDataDirectory instead.")]
        public static string UserDataPath { get => MelonUtils.UserDataDirectory; }
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
        [Obsolete("Main.IsBoneworks is obsolete. Please use MelonUtils.IsBONEWORKS instead.")]
        public static bool IsBoneworks = false;
        [Obsolete("Main.GetUnityVersion is obsolete. Please use MelonUtils.GetUnityVersion instead.")]
        public static string GetUnityVersion() => MelonUtils.GetUnityVersion();
        [Obsolete("Main.GetUserDataPath is obsolete. Please use MelonUtils.GetUserDataDirectory instead.")]
        public static string GetUserDataPath() => MelonUtils.UserDataDirectory;
        internal static void LegacySupport()
        {
            Mods = MelonHandler.Mods;
            Plugins = MelonHandler.Plugins;
            IsVRChat = MelonUtils.IsVRChat;
            IsBoneworks = MelonUtils.IsBONEWORKS;
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
        internal MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
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
        internal MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
    [Obsolete("MelonModLogger is obsolete. Please use MelonLogger instead.")]
    public class MelonModLogger : MelonLogger {}
    [Obsolete("MelonPrefs is obsolete. Please use MelonPreferences instead.")]
    public class MelonPrefs
    {
        [Obsolete("MelonPrefs.RegisterCategory is obsolete. Please use MelonPreferences.CreateCategory instead.")]
        public static void RegisterCategory(string name, string displayText) => MelonPreferences.CreateCategory(name, displayText);
        [Obsolete("MelonPrefs.RegisterString is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonPrefs.RegisterBool is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonPrefs.RegisterInt is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonPrefs.RegisterFloat is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => MelonPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonPrefs.HasKey is obsolete. Please use MelonPreferences.HasEntry instead.")]
        public static bool HasKey(string section, string name) => MelonPreferences.HasEntry(section, name);
        [Obsolete("MelonPrefs.GetPreferences is obsolete. Please use MelonPreferences.Categories instead.")]
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
        [Obsolete("MelonPrefs.GetCategoryDisplayName is obsolete. Please use MelonPreferences.GetCategoryDisplayName instead.")]
        public static string GetCategoryDisplayName(string key) => MelonPreferences.GetCategory(key)?.DisplayName;
        [Obsolete("MelonPrefs.SaveConfig is obsolete. Please use MelonPreferences.Save instead.")]
        public static void SaveConfig() => MelonPreferences.Save();
        [Obsolete("MelonPrefs.GetString is obsolete. Please use MelonPreferences.GetEntryString instead.")]
        public static string GetString(string section, string name)
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory(section);
            if (category == null)
                return null;
            MelonPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return null;
            if (entry.GetReflectedType() == typeof(bool))
                return entry.GetValue<bool>().ToString();
            else if (entry.GetReflectedType() == typeof(int))
                return entry.GetValue<int>().ToString();
            else if (entry.GetReflectedType() == typeof(long))
                return entry.GetValue<long>().ToString();
            else if (entry.GetReflectedType() == typeof(float))
                return entry.GetValue<float>().ToString();
            else if (entry.GetReflectedType() == typeof(double))
                return entry.GetValue<double>().ToString();
            else if (entry.GetReflectedType() == typeof(byte))
                return entry.GetValue<byte>().ToString();
            return null;
        }
        [Obsolete("MelonPrefs.SetString is obsolete. Please use MelonPreferences.SetEntryString instead.")]
        public static void SetString(string section, string name, string value)
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory(section);
            if (category == null)
                return;
            MelonPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return;
            int val_int = 0;
            float val_float = 0f;
            if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                entry.SetValue(value.ToLower().StartsWith("true"));
            else if (Int32.TryParse(value, out val_int))
                entry.SetValue(val_int);
            else if (float.TryParse(value, out val_float))
                entry.SetValue(val_float);
            else
                entry.SetValue(value);
        }
        [Obsolete("MelonPrefs.GetBool is obsolete. Please use MelonPreferences.GetEntryBool instead.")]
        public static bool GetBool(string section, string name) => MelonPreferences.GetEntryValue<bool>(section, name);
        [Obsolete("MelonPrefs.SetBool is obsolete. Please use MelonPreferences.SetEntryBool instead.")]
        public static void SetBool(string section, string name, bool value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonPrefs.GetInt is obsolete. Please use MelonPreferences.GetEntryInt instead.")]
        public static int GetInt(string section, string name) => MelonPreferences.GetEntryValue<int>(section, name);
        [Obsolete("MelonPrefs.SetInt is obsolete. Please use MelonPreferences.SetEntryInt instead.")]
        public static void SetInt(string section, string name, int value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonPrefs.GetFloat is obsolete. Please use MelonPreferences.GetEntryFloat instead.")]
        public static float GetFloat(string section, string name) => MelonPreferences.GetEntryValue<float>(section, name);
        [Obsolete("MelonPrefs.GetEntryFloat is obsolete. Please use MelonPreferences.SetEntryFloat instead.")]
        public static void SetFloat(string section, string name, float value) => MelonPreferences.SetEntryValue(section, name, value);
        [Obsolete("MelonPrefs.MelonPreferenceType is obsolete. Please use MelonPreferences_Entry.TypeEnum instead.")]
        public enum MelonPreferenceType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("MelonPrefs.MelonPreference is obsolete. Please use MelonPreferences_Entry instead.")]
        public class MelonPreference
        {
            [Obsolete("MelonPrefs.MelonPreference.Value is obsolete. Please use MelonPreferences_Entry.GetValue instead.")]
            public string Value { get => GetString(Entry.Category.Identifier, Entry.Identifier); set => SetString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MelonPrefs.MelonPreference.ValueEdited is obsolete. Please use MelonPreferences_Entry.GetValueEdited instead.")]
            public string ValueEdited { get => GetEditedString(Entry.Category.Identifier, Entry.Identifier); set => SetEditedString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MelonPrefs.MelonPreference.Type is obsolete. Please use MelonPreferences_Entry.GetReflectedType instead.")]
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
            [Obsolete("MelonPrefs.MelonPreference.Hidden is obsolete. Please use MelonPreferences_Entry.IsHidden instead.")]
            public bool Hidden { get => Entry.IsHidden; }
            [Obsolete("MelonPrefs.MelonPreference.DisplayText is obsolete. Please use MelonPreferences_Entry.DisplayName instead.")]
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
                if (entry.GetReflectedType() == typeof(bool))
                    return entry.GetEditedValue<bool>().ToString();
                else if (entry.GetReflectedType() == typeof(int))
                    return entry.GetEditedValue<int>().ToString();
                else if (entry.GetReflectedType() == typeof(long))
                    return entry.GetEditedValue<long>().ToString();
                else if (entry.GetReflectedType() == typeof(float))
                    return entry.GetEditedValue<float>().ToString();
                else if (entry.GetReflectedType() == typeof(double))
                    return entry.GetEditedValue<double>().ToString();
                else if (entry.GetReflectedType() == typeof(byte))
                    return entry.GetEditedValue<byte>().ToString();
                return null;
            }
            private static void SetEditedString(string section, string name, string value)
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(section);
                if (category == null)
                    return;
                MelonPreferences_Entry entry = category.GetEntry(name);
                if (entry == null)
                    return;
                int val_int = 0;
                float val_float = 0f;
                if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                    entry.SetEditedValue(value.ToLower().StartsWith("true"));
                else if (Int32.TryParse(value, out val_int))
                    entry.SetEditedValue(val_int);
                else if (float.TryParse(value, out val_float))
                    entry.SetEditedValue(val_float);
                else
                    entry.SetEditedValue(value);
            }
        }
    }
    [Obsolete("ModPrefs is obsolete. Please use MelonPreferences instead.")]
    public class ModPrefs : MelonPrefs
    {
        [Obsolete("ModPrefs.GetPrefs is obsolete. Please use MelonPreferences instead.")]
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
                    PrefDesc newpref = new PrefDesc(pref);
                    newpref.ValueEdited = pref.ValueEdited;
                    newprefsdict.Add(prefsdict.Keys.ElementAt(j), newpref);
                }
                output.Add(prefs.Keys.ElementAt(i), newprefsdict);
            }
            return output;
        }
        [Obsolete("ModPrefs.RegisterPrefString is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => RegisterString(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("ModPrefs.RegisterPrefBool is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => RegisterBool(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("ModPrefs.RegisterPrefInt is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => RegisterInt(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("ModPrefs.RegisterPrefFloat is obsolete. Please use MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => RegisterFloat(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("ModPrefs.PrefType is obsolete. Please use MelonPreferences_Entry.TypeEnum instead.")]
        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("ModPrefs.PrefDesc is obsolete. Please use MelonPreferences_Entry instead.")]
        public class PrefDesc : MelonPreference
        {
            [Obsolete("ModPrefs.PrefDesc.Type is obsolete. Please use MelonPreferences_Entry.Type instead.")]
            public PrefType Type { get => (PrefType)base.Type; }
            [Obsolete("ModPrefs.PrefDesc is obsolete. Please use MelonPreferences_Entry instead.")]
            public PrefDesc(MelonPreferences_Entry entry) : base(entry) { }
            public PrefDesc(MelonPreference pref) : base(pref) { }
        }
    }
}