﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    public static class MelonCommandLine
    {
        private class ArgInfo
        {
            internal string CommandLine;
            internal delegate void ParseCommandLineArgDelegate(string value);
            internal ParseCommandLineArgDelegate ParseCommandLineArg;

            internal string Config_Name;
            internal string Config_Category;
            internal delegate void ParseConfigArgDelegate(ArgInfo argInfo);
            internal ParseConfigArgDelegate ParseConfigArg;
        }
        private static readonly List<ArgInfo> ArgInfoTbl = new List<ArgInfo>();
        private static readonly IniFile iniFile = null;

        static MelonCommandLine()
        {
            iniFile = new IniFile(Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "LaunchOptions.ini"));

            string Config_Category_AnalyticsBlocker = "AnalyticsBlocker";
            string Config_Category_AssemblyGenerator = "AssemblyGenerator";
            string Config_Category_Console = "Console";
            string Config_Category_Core = "Core";
            string Config_Category_Logger = "Logger";

            #region AnalyticsBlocker
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.dab",
                ParseCommandLineArg = (string value) => AnalyticsBlocker.ShouldDAB = true,

                Config_Name = "ShouldDAB",
                Config_Category = Config_Category_AnalyticsBlocker,
                ParseConfigArg = (ArgInfo argInfo) => AnalyticsBlocker.ShouldDAB = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, AnalyticsBlocker.ShouldDAB, true)
            });
            #endregion

            #region AssemblyGenerator
            /*
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.agfregenerate",
                ParseCommandLineArg = (string value) => AssemblyGenerator.ForceRegeneration = true,

                //Config_Name = "ForceRegenerate",
                //Config_Category = Config_Category_AssemblyGenerator,
                //ParseConfigArg = (ArgInfo argInfo) => AssemblyGenerator.ForceRegeneration = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, AssemblyGenerator.ForceRegeneration, true)
            });
            */

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.agfvdumper",
                ParseCommandLineArg = (string value) => AssemblyGenerator.ForceVersion_Dumper = value,

                Config_Name = "ForceVersion_Dumper",
                Config_Category = Config_Category_AssemblyGenerator,
                ParseConfigArg = (ArgInfo argInfo) => AssemblyGenerator.ForceVersion_Dumper = iniFile.GetString(argInfo.Config_Category, argInfo.Config_Name, AssemblyGenerator.ForceVersion_Dumper, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.agfvunhollower",
                ParseCommandLineArg = (string value) => AssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower = value,

                Config_Name = "ForceVersion_Il2CppAssemblyUnhollower",
                Config_Category = Config_Category_AssemblyGenerator,
                ParseConfigArg = (ArgInfo argInfo) => AssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower = iniFile.GetString(argInfo.Config_Category, argInfo.Config_Name, AssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.agfvunity",
                ParseCommandLineArg = (string value) => AssemblyGenerator.ForceVersion_UnityDependencies = value,

                Config_Name = "ForceVersion_UnityDependencies",
                Config_Category = Config_Category_AssemblyGenerator,
                ParseConfigArg = (ArgInfo argInfo) => AssemblyGenerator.ForceVersion_UnityDependencies = iniFile.GetString(argInfo.Config_Category, argInfo.Config_Name, AssemblyGenerator.ForceVersion_UnityDependencies, true)
            });
            #endregion

            #region Console
#if !DEBUG
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.hideconsole",
                ParseCommandLineArg = (string value) => Console.Enabled = false,

                Config_Name = "Enabled",
                Config_Category = Config_Category_Console,
                ParseConfigArg = (ArgInfo argInfo) => Console.Enabled = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Console.Enabled, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.hidewarnings",
                ParseCommandLineArg = (string value) => Console.HideWarnings = true,

                Config_Name = "HideWarnings",
                Config_Category = Config_Category_Console,
                ParseConfigArg = (ArgInfo argInfo) => Console.HideWarnings = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Console.HideWarnings, true)
            });
#endif

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.consoleontop",
                ParseCommandLineArg = (string value) => Console.AlwaysOnTop = true,

                Config_Name = "AlwaysOnTop",
                Config_Category = Config_Category_Console,
                ParseConfigArg = (ArgInfo argInfo) => Console.AlwaysOnTop = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Console.AlwaysOnTop, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.consoledst",
                ParseCommandLineArg = (string value) => Console.SetTitleOnInit = false,

                Config_Name = "SetTitleOnInit",
                Config_Category = Config_Category_Console,
                ParseConfigArg = (ArgInfo argInfo) => Console.SetTitleOnInit = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Console.SetTitleOnInit, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.magenta",
                ParseCommandLineArg = (string value) => Console.DisplayMode = Console.DisplayModeEnum.MAGENTA
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.rainbow",
                ParseCommandLineArg = (string value) => Console.DisplayMode = Console.DisplayModeEnum.RAINBOW
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.randomrainbow",
                ParseCommandLineArg = (string value) => Console.DisplayMode = Console.DisplayModeEnum.RANDOMRAINBOW
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--lemonloader",
                ParseCommandLineArg = (string value) => Console.DisplayMode = Console.DisplayModeEnum.LEMON
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                Config_Name = "DisplayMode",
                Config_Category = Config_Category_Console,
                ParseConfigArg = (ArgInfo argInfo) =>
                {
                    int mode = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, (int)Console.DisplayMode, true);
                    if ((mode >= (int)Console.DisplayModeEnum.NORMAL)
                        && (mode <= (int)Console.DisplayModeEnum.LEMON))
                        Console.DisplayMode = (Console.DisplayModeEnum)mode;
                }
            });
            #endregion

            #region Core
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.loadmodeplugins",
                ParseCommandLineArg = (string value) =>
                {
                    int mode = 0;
                    if (int.TryParse(value, out mode)
                        && (mode >= (int)Core.LoadModeEnum.NORMAL)
                        && (mode <= (int)Core.LoadModeEnum.BOTH))
                        Core.LoadMode_Plugins = (Core.LoadModeEnum)mode;
                },

                Config_Name = "LoadMode_Plugins",
                Config_Category = Config_Category_Core,
                ParseConfigArg = (ArgInfo argInfo) =>
                {
                    int mode = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, (int)Core.LoadMode_Plugins, true);
                    if ((mode >= (int)Core.LoadModeEnum.NORMAL)
                        && (mode <= (int)Core.LoadModeEnum.BOTH))
                        Core.LoadMode_Plugins = (Core.LoadModeEnum)mode;
                }
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.loadmodemods",
                ParseCommandLineArg = (string value) =>
                {
                    int mode = 0;
                    if (int.TryParse(value, out mode)
                        && (mode >= (int)Core.LoadModeEnum.NORMAL)
                        && (mode <= (int)Core.LoadModeEnum.BOTH))
                        Core.LoadMode_Mods = (Core.LoadModeEnum)mode;
                },

                Config_Name = "LoadMode_Mods",
                Config_Category = Config_Category_Core,
                ParseConfigArg = (ArgInfo argInfo) =>
                {
                    int mode = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, (int)Core.LoadMode_Mods, true);
                    if ((mode >= (int)Core.LoadModeEnum.NORMAL)
                        && (mode <= (int)Core.LoadModeEnum.BOTH))
                        Core.LoadMode_Mods = (Core.LoadModeEnum)mode;
                }
            });

#if !DEBUG
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.debug",
                ParseCommandLineArg = (string value) => Core.DebugMode = true,

                Config_Name = "DebugMode",
                Config_Category = Config_Category_Core,
                ParseConfigArg = (ArgInfo argInfo) => Core.DebugMode = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Core.DebugMode, true)
            });
#endif

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--quitfix",
                ParseCommandLineArg = (string value) => Core.QuitFix = true,

                Config_Name = "QuitFix",
                Config_Category = Config_Category_Core,
                ParseConfigArg = (ArgInfo argInfo) => Core.QuitFix = iniFile.GetBool(argInfo.Config_Category, argInfo.Config_Name, Core.QuitFix, true)
            });
            #endregion

            #region Logger
            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.maxlogs",
                ParseCommandLineArg = (string value) => { int val = Logger.Max_Logs; if (int.TryParse(value, out val)) Logger.Max_Logs = val; },

                Config_Name = "Max_Logs",
                Config_Category = Config_Category_Logger,
                ParseConfigArg = (ArgInfo argInfo) => Logger.Max_Logs = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, Logger.Max_Logs, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.maxwarnings",
                ParseCommandLineArg = (string value) => { int val = Logger.Max_Warnings; if (int.TryParse(value, out val)) Logger.Max_Warnings = val; },

                Config_Name = "Max_Warnings",
                Config_Category = Config_Category_Logger,
                ParseConfigArg = (ArgInfo argInfo) => Logger.Max_Warnings = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, Logger.Max_Warnings, true)
            });

            ArgInfoTbl.Add(new ArgInfo
            {
                CommandLine = "--melonloader.maxerrors",
                ParseCommandLineArg = (string value) => { int val = Logger.Max_Errors; if (int.TryParse(value, out val)) Logger.Max_Errors = val; },
                
                Config_Name = "Max_Errors",
                Config_Category = Config_Category_Logger,
                ParseConfigArg = (ArgInfo argInfo) => Logger.Max_Errors = iniFile.GetInt(argInfo.Config_Category, argInfo.Config_Name, Logger.Max_Errors, true)
            });
            #endregion
        }

        internal static void Load()
        {
            ParseConfig();
            ParseCommandLine();
        }

        private static void ParseConfig()
        {
            if (ArgInfoTbl.Count <= 0)
                return;
            foreach (ArgInfo argInfo in ArgInfoTbl)
            {
                if ((argInfo.ParseConfigArg == null)
                    || string.IsNullOrEmpty(argInfo.Config_Category)
                    || string.IsNullOrEmpty(argInfo.Config_Name))
                    continue;
                argInfo.ParseConfigArg(argInfo);
            }
        }

        private static void ParseCommandLine()
        {
            if (ArgInfoTbl.Count <= 0)
                return;
            string[] args = Environment.GetCommandLineArgs();
            if ((args == null)
                || (args.Length <= 0))
                return;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg))
                    continue;
                ArgInfo argInfo = GetArgInfoFromCommandLine(arg);
                if ((argInfo == null)
                    || (argInfo.ParseCommandLineArg == null))
                    continue;
                string value = null;
                if ((i + 1) < args.Length)
                    value = args[i + 1];
                argInfo.ParseCommandLineArg(value);
            }
        }

        private static ArgInfo GetArgInfoFromCommandLine(string cmd)
        {
            foreach (ArgInfo argInfo in ArgInfoTbl)
            {
                if (!string.IsNullOrEmpty(argInfo.CommandLine)
                    && argInfo.CommandLine.Equals(cmd))
                    return argInfo;
            }
            return null;
        }

        #region Args
        public static class AnalyticsBlocker
        {
            static AnalyticsBlocker() => ShouldDAB = false;
            public static bool ShouldDAB { get; internal set; }
        }

        public static class AssemblyGenerator
        {
            static AssemblyGenerator()
            {
                ForceRegeneration = false;
                ForceVersion_Dumper = "0.0.0.0";
                ForceVersion_Il2CppAssemblyUnhollower = "0.0.0.0";
                ForceVersion_UnityDependencies = "0.0.0.0";
            }
            public static bool ForceRegeneration { get; internal set; }
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceVersion_Il2CppAssemblyUnhollower { get; internal set; }
            public static string ForceVersion_UnityDependencies { get; internal set; }
        }

        public static class Console
        {
            static Console()
            {
                DisplayMode = DisplayModeEnum.NORMAL;
                Enabled = true;
                AlwaysOnTop = false;
                HideWarnings = false;
                SetTitleOnInit = true;
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
        }

        public static class Core
        {
            static Core()
            {
                LoadMode_Plugins = LoadModeEnum.NORMAL;
                LoadMode_Mods = LoadModeEnum.NORMAL;
                QuitFix = false;
#if DEBUG
                DebugMode = true;
#else
                DebugMode = false;
#endif
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

        public static class Logger
        {
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
        #endregion
    }
}