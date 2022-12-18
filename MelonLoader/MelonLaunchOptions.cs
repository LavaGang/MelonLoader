using System;
using System.Collections.Generic;
using System.Drawing;

namespace MelonLoader
{
    public static class MelonLaunchOptions
    {
        private static Dictionary<string, Action> WithoutArg = new Dictionary<string, Action>();
        private static Dictionary<string, Action<string>> WithArg = new Dictionary<string, Action<string>>();

        static MelonLaunchOptions()
        {
            AnalyticsBlocker.Setup();
            Core.Setup();
            Console.Setup();
            Il2CppAssemblyGenerator.Setup();
            Logger.Setup();
        }

        internal static void Load()
        {
            List<string> foundOptions = new List<string>();

            LemonEnumerator<string> argEnumerator = new LemonEnumerator<string>(Environment.GetCommandLineArgs());
            while (argEnumerator.MoveNext())
            {
                string fullcmd = argEnumerator.Current;
                if (string.IsNullOrEmpty(fullcmd))
                    continue;

                if (!fullcmd.StartsWith("--"))
                    continue;

                string cmd = fullcmd.Remove(0, 2);

                if (WithoutArg.TryGetValue(cmd, out Action withoutArgFunc))
                {
                    foundOptions.Add(fullcmd);
                    withoutArgFunc();
                }
                else if (WithArg.TryGetValue(cmd, out Action<string> withArgFunc))
                {
                    if (!argEnumerator.MoveNext())
                        continue;

                    string cmdArg = argEnumerator.Current;
                    if (string.IsNullOrEmpty(cmdArg))
                        continue;

                    if (cmdArg.StartsWith("--"))
                        continue;

                    foundOptions.Add($"{fullcmd} = {cmdArg}");
                    withArgFunc(cmdArg);
                }
            }

            if (foundOptions.Count <= 0)
                return;
        }

#region Args
        public static class AnalyticsBlocker
        {
            public static bool ShouldDAB { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["melonloader.dab"] = () => ShouldDAB = true;

            }
        }

        public static class Core
        {
            public enum LoadModeEnum
            {
                NORMAL,
                DEV,
                BOTH
            }
            public static LoadModeEnum LoadMode_Plugins { get; internal set; }
            public static LoadModeEnum LoadMode_Mods { get; internal set; }
            public static bool QuitFix { get; internal set; }
            public static bool StartScreen { get; internal set; } = true;
            public static string UnityVersion { get; internal set; }
            public static bool IsDebug { get; internal set; }
            public static bool UserWantsDebugger { get; internal set; }
            public static bool ShouldDisplayAnalyticsBlocker { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["quitfix"] = () => QuitFix = true;
                WithoutArg["melonloader.disablestartscreen"] = () => StartScreen = false;
                WithArg["melonloader.loadmodeplugins"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        LoadMode_Plugins = (LoadModeEnum)MelonUtils.Clamp(valueint, (int)LoadModeEnum.NORMAL, (int)LoadModeEnum.BOTH);
                };
                WithArg["melonloader.loadmodemods"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        LoadMode_Mods = (LoadModeEnum)MelonUtils.Clamp(valueint, (int)LoadModeEnum.NORMAL, (int)LoadModeEnum.BOTH);
                };
                WithArg["melonloader.unityversion"] = (string arg) => UnityVersion = arg;
                WithoutArg["melonloader.debug"] = () => IsDebug = true;
                WithoutArg["melonloader.launchdebugger"] = () => UserWantsDebugger = true;
                WithoutArg["melonloader.dab"] = () => ShouldDisplayAnalyticsBlocker = true;
            }
        }

        public static class Console
        {
            public enum DisplayMode
            {
                NORMAL,
                MAGENTA,
                RAINBOW,
                RANDOMRAINBOW,
                LEMON
            };
            public static DisplayMode Mode { get; internal set; }
            public static bool CleanUnityLogs { get; internal set; } = true;
            public static bool ShouldSetTitle { get; internal set; } = true;
            public static bool AlwaysOnTop { get; internal set; }
            public static bool ShouldHide { get; internal set; }
            public static bool HideWarnings { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["melonloader.disableunityclc"] = () => CleanUnityLogs = false;
                WithoutArg["melonloader.consoledst"] = () => ShouldSetTitle = false;
                WithoutArg["melonloader.consoleontop"] = () => AlwaysOnTop = true;
                WithoutArg["melonloader.hideconsole"] = () => ShouldHide = true;
                WithoutArg["melonloader.hidewarnings"] = () => HideWarnings = true;

                WithArg["melonloader.consolemode"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        Mode = (DisplayMode)MelonUtils.Clamp(valueint, (int)DisplayMode.NORMAL, (int)DisplayMode.LEMON);
                };
            }
        }

        public static class Il2CppAssemblyGenerator
        {
            public static bool ForceRegeneration { get; internal set; }
            public static bool OfflineMode { get; internal set; }
            public static bool DisableDeobfMapIntegrityCheck { get; internal set; }
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceRegex { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["melonloader.disabledmic"] = () => DisableDeobfMapIntegrityCheck = true;
                WithoutArg["melonloader.agfoffline"] = () => OfflineMode = true;
                WithoutArg["melonloader.agfregenerate"] = () => ForceRegeneration = true;
                WithArg["melonloader.agfvdumper"] = (string arg) => ForceVersion_Dumper = arg;
                WithArg["melonloader.agfregex"] = (string arg) => ForceRegex = arg;
            }
        }

        public static class Logger
        {
            public static int MaxLogs { get; internal set; } = 10;
            public static int MaxWarnings { get; internal set; } = 10;
            public static int MaxErrors { get; internal set; } = 10;

            internal static void Setup()
            {
                WithArg["melonloader.maxlogs"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxLogs = valueint;
                };
                WithArg["melonloader.maxwarnings"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxWarnings = valueint;
                };
                WithArg["melonloader.maxerrors"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxErrors = valueint;
                };
            }
        }
        #endregion
    }
}