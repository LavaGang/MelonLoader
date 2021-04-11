using System;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    public static class MelonLaunchOptions
    {
        private static string FilePath = null;
        private static Dictionary<string, MelonPreferences_Entry> CommandLineToEntryDict = new Dictionary<string, MelonPreferences_Entry>();

        static MelonLaunchOptions()
        {
            FilePath = Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "LaunchOptions.cfg");
            SetupDictionary();
            if (File.Exists(FilePath))
                LoadCategories();
            else
                SaveCategories();
            DisableFileSaving();
        }

        private static void SetupDictionary()
        {
            //AnalyticsBlocker.Setup();
            //Console.Setup();
            Core.Setup();
            if (MelonUtils.IsGameIl2Cpp())
                Il2CppAssemblyGenerator.Setup();
            //Logger.Setup();
        }

        private static void LoadCategories()
        {
            //AnalyticsBlocker.Category.LoadFromFile(false);
            //Console.Category.LoadFromFile(false);
            Core.Category.LoadFromFile(false);
            if (MelonUtils.IsGameIl2Cpp())
                Il2CppAssemblyGenerator.Category.LoadFromFile(false);
            //Logger.Category.LoadFromFile(false);
        }

        private static void SaveCategories()
        {
            //AnalyticsBlocker.Category.SaveToFile(false);
            //Console.Category.SaveToFile(false);
            Core.Category.SaveToFile(false);
            if (MelonUtils.IsGameIl2Cpp())
                Il2CppAssemblyGenerator.Category.SaveToFile(false);
            //Logger.Category.SaveToFile(false);
        }

        private static void DisableFileSaving()
        {
            //AnalyticsBlocker.Category.File.ShouldSave = false;
            //Console.Category.File.ShouldSave = false;
            Core.Category.File.ShouldSave = false;
            if (MelonUtils.IsGameIl2Cpp())
                Il2CppAssemblyGenerator.Category.File.ShouldSave = false;
            //Logger.Category.File.ShouldSave = false;
        }

        private static void SetupCategory(string name, ref MelonPreferences_Category Category)
        {
            Category = MelonPreferences.CreateCategory(name, is_hidden: true);
            Category.SetFilePath(FilePath, autoload: false);
            Category.DestroyFileWatcher();
        }

        private static void AddOption<T>(MelonPreferences_Category cat, string commandline, string prefid, T defaultvalue = default, Action<T, T> onchangecallback = null)
        {
            MelonPreferences_Entry<T> entry = cat.CreateEntry(prefid, defaultvalue, is_hidden: true);
            if (onchangecallback != null)
                entry.OnValueChanged += onchangecallback;
            CommandLineToEntryDict[commandline] = entry;
        }

        internal static void Load()
        {
            string[] args = Environment.GetCommandLineArgs();
            if ((args == null)
                || (args.Length <= 0))
                return;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg))
                    continue;
                arg = arg.ToLowerInvariant();
                MelonPreferences_Entry entry = null;
                if (!CommandLineToEntryDict.TryGetValue(arg, out entry))
                    continue;
                Type entry_type = entry.GetReflectedType();
                if (entry_type == typeof(bool))
                {
                    MelonPreferences_Entry<bool> entry_bool = (MelonPreferences_Entry<bool>)entry;
                    entry_bool.Value = !entry_bool.DefaultValue;
                }
                else if ((entry_type == typeof(int))
                    || (entry_type == typeof(string)))
                {
                    string valuestr = null;
                    if ((i + 1) < args.Length)
                        valuestr = args[i + 1];
                    if (string.IsNullOrEmpty(valuestr))
                        continue;
                    if (entry_type == typeof(int))
                    {
                        MelonPreferences_Entry<int> entry_int = (MelonPreferences_Entry<int>)entry;
                        int value = 0;
                        if (!int.TryParse(valuestr, out value))
                            continue;
                        entry_int.Value = value;
                    }
                    else if (entry_type == typeof(string))
                    {
                        MelonPreferences_Entry<string> entry_string = (MelonPreferences_Entry<string>)entry;
                        entry_string.Value = valuestr;
                    }
                }
            }
        }

        #region Args

        /*
        public static class AnalyticsBlocker
        {
            internal static MelonPreferences_Category Category;
            internal static void Setup()
            {
                SetupCategory("AnalyticsBlocker", ref Category);

                AddOption(Category, "--melonloader.dab", nameof(ShouldDAB), onchangecallback: new Action<bool, bool> ((oldval, newval) => { ShouldDAB = newval; }));
            }

            public static bool ShouldDAB { get; internal set; }
        }

        public static class Console
        {
            internal static MelonPreferences_Category Category;
            internal static void Setup()
            {
                SetupCategory("Console", ref Category);

#if !DEBUG
                AddOption(Category, "--melonloader.hideconsole", nameof(Enabled), true, new Action<bool, bool>((oldval, newval) => { Enabled = newval; }));
                AddOption(Category, "--melonloader.hidewarnings", nameof(HideWarnings), onchangecallback: new Action<bool, bool>((oldval, newval) => { HideWarnings = newval; }));
#endif

                AddOption(Category, "--melonloader.consoleontop", nameof(AlwaysOnTop), onchangecallback: new Action<bool, bool>((oldval, newval) => { AlwaysOnTop = newval; }));
                AddOption(Category, "--melonloader.consoledst", nameof(SetTitleOnInit), true, new Action<bool, bool>((oldval, newval) => { SetTitleOnInit = newval; }));
                AddOption(Category, "--melonloader.consolemonitor", nameof(Monitor), 1, new Action<int, int>((oldval, newval) => { Monitor = newval; }));
                AddOption(Category, "--melonloader.consolemode", nameof(DisplayMode), onchangecallback: new Action<int, int>((oldval, newval) =>
                {
                    if ((newval >= ((int)DisplayModeEnum.NORMAL))
                        || (newval <= ((int)DisplayModeEnum.LEMON)))
                        DisplayMode = (DisplayModeEnum)newval;
                }));
            }

            public enum DisplayModeEnum
            {
                NORMAL,
                MAGENTA,
                RAINBOW,
                RANDOMRAINBOW,
                LEMON
            }
            public static DisplayModeEnum DisplayMode { get; internal set; }
            public static bool Enabled { get; internal set; }
            public static bool AlwaysOnTop { get; internal set; }
            public static bool HideWarnings { get; internal set; }
            public static bool SetTitleOnInit { get; internal set; }
            public static int Monitor { get; internal set; }
        }
        */

        public static class Core
        {
            internal static MelonPreferences_Category Category;
            internal static void Setup()
            {
                SetupCategory("Core", ref Category);

                AddOption(Category, "--melonloader.debug", nameof(DebugMode),
#if DEBUG
                true,
#else
                false,
#endif
                new Action<bool, bool>((oldval, newval) => { DebugMode = newval; }));

                AddOption(Category, "--quitfix", nameof(QuitFix), onchangecallback: new Action<bool, bool>((oldval, newval) => { QuitFix = newval; }));
                AddOption(Category, "--melonloader.loadmodeplugins", nameof(LoadMode_Plugins), onchangecallback: new Action<int, int>((oldval, newval) =>
                {
                    if ((newval >= ((int)LoadModeEnum.NORMAL))
                        || (newval <= ((int)LoadModeEnum.BOTH)))
                        LoadMode_Plugins = (LoadModeEnum)newval;
                }));
                AddOption(Category, "--melonloader.loadmodemods", nameof(LoadMode_Mods), onchangecallback: new Action<int, int>((oldval, newval) =>
                {
                    if ((newval >= ((int)LoadModeEnum.NORMAL))
                        || (newval <= ((int)LoadModeEnum.BOTH)))
                        LoadMode_Mods = (LoadModeEnum)newval;
                }));
            }

            public enum LoadModeEnum
            {
                NORMAL,
                DEV,
                BOTH
            }
            public static LoadModeEnum LoadMode_Plugins { get; internal set; }
            public static LoadModeEnum LoadMode_Mods { get; internal set; }
            public static bool DebugMode { get; internal set; }
            public static bool QuitFix { get; internal set; }
        }

        public static class Il2CppAssemblyGenerator
        {
            internal static MelonPreferences_Category Category;
            internal static void Setup()
            {
                SetupCategory("Il2CppAssemblyGenerator", ref Category);

                AddOption(Category, "--melonloader.agfregenerate", nameof(ForceRegeneration), false, new Action<bool, bool>((oldval, newval) => { ForceRegeneration = newval; }));
                AddOption(Category, "--melonloader.agfvdumper", nameof(ForceVersion_Dumper), "0.0.0.0", new Action<string, string>((oldval, newval) => { ForceVersion_Dumper = newval; }));
                AddOption(Category, "--melonloader.agfvunhollower", nameof(ForceVersion_Il2CppAssemblyUnhollower), "0.0.0.0", new Action<string, string>((oldval, newval) => { ForceVersion_Il2CppAssemblyUnhollower = newval; }));
                AddOption(Category, "--melonloader.agfvunity", nameof(ForceVersion_UnityDependencies), "0.0.0.0", new Action<string, string>((oldval, newval) => { ForceVersion_UnityDependencies = newval; }));
            }

            public static bool ForceRegeneration { get; internal set; }
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceVersion_Il2CppAssemblyUnhollower { get; internal set; }
            public static string ForceVersion_UnityDependencies { get; internal set; }
        }

        /*
        public static class Logger
        {
            internal static MelonPreferences_Category Category;
            internal static void Setup()
            {
                SetupCategory("Logger", ref Category);

                AddOption(Category, "--melonloader.maxlogs", nameof(Max_Logs), 10, new Action<int, int>((oldval, newval) => { Max_Logs = newval; }));
                AddOption(Category, "--melonloader.maxwarnings", nameof(Max_Warnings), 100, new Action<int, int>((oldval, newval) => { Max_Warnings = newval; }));
                AddOption(Category, "--melonloader.maxerrors", nameof(Max_Errors), 100, new Action<int, int>((oldval, newval) => { Max_Errors = newval; }));
            }
            static Logger()
            {
                Max_Logs = 10;
                Max_Warnings = 100;
                Max_Errors = 100;
            }
            public static int Max_Logs { get; internal set; }
            public static int Max_Warnings { get; internal set; }
            public static int Max_Errors { get; internal set; }
        }
        */

        #endregion
    }
}