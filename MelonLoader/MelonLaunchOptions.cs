using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public static class MelonLaunchOptions
    {
        private static Dictionary<string, Action> WithoutArg = new Dictionary<string, Action>();
        private static Dictionary<string, Action<string>> WithArg = new Dictionary<string, Action<string>>();

        static MelonLaunchOptions()
        {
            Core.Setup();
            Console.Setup();
            Il2CppAssemblyGenerator.Setup();
        }

        internal static void Load()
        {
            LemonEnumerator<string> argEnumerator = new LemonEnumerator<string>(Environment.GetCommandLineArgs());
            while (argEnumerator.MoveNext())
            {
                if (string.IsNullOrEmpty(argEnumerator.Current))
                    continue;

                if (!argEnumerator.Current.StartsWith("--"))
                    continue;

                string cmd = argEnumerator.Current.Remove(0, 2);

                if (WithoutArg.TryGetValue(cmd, out Action withoutArgFunc))
                    withoutArgFunc();
                else if (WithArg.TryGetValue(cmd, out Action<string> withArgFunc))
                {
                    if (!argEnumerator.MoveNext())
                        continue;

                    if (string.IsNullOrEmpty(argEnumerator.Current))
                        continue;

                    if (argEnumerator.Current.StartsWith("--"))
                        continue;

                    withArgFunc(argEnumerator.Current);
                }
            }
        }

#region Args
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
            public static bool IsDebug { get; internal set; }
            public static bool UserWantsDebugger { get; internal set; }

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
                WithoutArg["melonloader.debug"] = () => IsDebug = true;
                WithoutArg["melonloader.launchdebugger"] = () => UserWantsDebugger = true;
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

            internal static void Setup()
            {
                WithoutArg["melonloader.disableunityclc"] = () => CleanUnityLogs = false;
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
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceVersion_Il2CppAssemblyUnhollower { get; internal set; }
            public static string ForceVersion_UnityDependencies { get; internal set; }
            public static string ForceRegex { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["melonloader.agfoffline"] = () => OfflineMode = true;
                WithoutArg["melonloader.agfregenerate"] = () => ForceRegeneration = true;
                WithArg["melonloader.agfvdumper"] = (string arg) => ForceVersion_Dumper = arg;
                WithArg["melonloader.agfvunhollower"] = (string arg) => ForceVersion_Il2CppAssemblyUnhollower = arg;
                WithArg["melonloader.agfvunity"] = (string arg) => ForceVersion_UnityDependencies = arg;
                WithArg["melonloader.agfregex"] = (string arg) => ForceRegex = arg;
            }
        }
#endregion
    }
}