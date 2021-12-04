using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable 0108

namespace MelonLoader
{
    [Obsolete("MelonLoader.ModPrefs is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences instead.")]
    public class ModPrefs : MelonPrefs
    {
        [Obsolete("MelonLoader.ModPrefs.GetPrefs is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences instead.")]
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
        [Obsolete("MelonLoader.ModPrefs.RegisterPrefString is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => RegisterString(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.ModPrefs.RegisterPrefBool is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => RegisterBool(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.ModPrefs.RegisterPrefInt is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => RegisterInt(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.ModPrefs.RegisterPrefFloat is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences.CreateEntry instead.")]
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => RegisterFloat(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MelonLoader.ModPrefs.PrefType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.TypeEnum instead.")]
        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("MelonLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry instead.")]
        public class PrefDesc : MelonPreference
        {
            [Obsolete("MelonLoader.ModPrefs.PrefDesc.Type is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry.Type instead.")]
            public PrefType Type { get => (PrefType)base.Type; }
            [Obsolete("MelonLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry instead.")]
            public PrefDesc(MelonPreferences_Entry entry) : base(entry) { }
            [Obsolete("MelonLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MelonLoader.MelonPreferences_Entry instead.")]
            public PrefDesc(MelonPreference pref) : base(pref) { }
        }
    }
}