using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public static class MelonLaunchOptions
    {
        private static Dictionary<string, Action> WithoutArg = new Dictionary<string, Action>();
        private static Dictionary<string, Action<string>> WithArg = new Dictionary<string, Action<string>>();
        private static string[] _cmd;

         /// <summary>
         /// Dictionary of all Arguments with value (if found) that were not used by MelonLoader
         /// <para>
         /// <b>Key</b> is the argument, <b>Value</b> is the value for the argument, <c>null</c> if not found
         /// </para>
         /// </summary>
        public static Dictionary<string, string> ExternalArguments { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> InternalArguments { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Array of All Command Line Arguments
        /// </summary>
        public static string[] CommandLineArgs
        {
            get
            {
                if (_cmd == null)
                    _cmd = Environment.GetCommandLineArgs();
                return _cmd;
            }
        }

        internal static void Load()
        {
            string[] args = CommandLineArgs;
            int maxLen = args.Length;
            for (int i = 1; i < maxLen; i++)
            {
                string fullcmd = args[i];
                if (string.IsNullOrEmpty(fullcmd))
                    continue;

                // Parse Prefix
                string noPrefixCmd = fullcmd;
                if (noPrefixCmd.StartsWith("--"))
                    noPrefixCmd = noPrefixCmd.Remove(0, 2);
                else if (noPrefixCmd.StartsWith("-"))
                    noPrefixCmd = noPrefixCmd.Remove(0, 1);
                else
                {
                    // Unknown Command, Add it to Dictionary
                    ExternalArguments.Add(noPrefixCmd, null);
                    continue;
                }

                // Parse Argumentless Commands
                if (WithoutArg.TryGetValue(noPrefixCmd, out Action withoutArgFunc))
                {
                    InternalArguments.Add(noPrefixCmd, null);
                    withoutArgFunc();
                    continue;
                }

                // Parse Argument
                string cmdArg = null;
                if (noPrefixCmd.Contains("="))
                {
                    string[] split = noPrefixCmd.Split('=');
                    noPrefixCmd = split[0];
                    cmdArg = split[1];
                }

                if ((string.IsNullOrEmpty(cmdArg)
                        && ((i + 1) >= maxLen))
                    || string.IsNullOrEmpty(cmdArg)
                    || cmdArg.StartsWith("--")
                    || cmdArg.StartsWith("-"))
                {
                    // Unknown Command, Add it to Dictionary
                    ExternalArguments.Add(noPrefixCmd, null);
                    continue;
                }

                // Parse Argument Commands
                if (WithArg.TryGetValue(noPrefixCmd, out Action<string> withArgFunc))
                {
                    InternalArguments.Add(noPrefixCmd, cmdArg);
                    withArgFunc(cmdArg);
                    continue;
                }

                // Unknown Command with Argument, Add it to Dictionary
                ExternalArguments.Add(noPrefixCmd, cmdArg);
            }
        }

        #region Obsolete

        [Obsolete("Use LoaderConfig.Current.Loader instead.")]
        public static class Core
        {
            [Obsolete("This option isn't used anymore.")]
            public enum LoadModeEnum
            {
                NORMAL,
                DEV,
                BOTH
            }

            [Obsolete("This option isn't used anymore. It will always return NORMAL.")]
            public static LoadModeEnum LoadMode_Plugins => LoadModeEnum.NORMAL;

            [Obsolete("This option isn't used anymore. It will always return NORMAL.")]
            public static LoadModeEnum LoadMode_Mods => LoadModeEnum.NORMAL;

            [Obsolete("Use LoaderConfig.Current.Loader.ForceQuit instead.")]
            public static bool QuitFix => LoaderConfig.Current.Loader.ForceQuit;

            [Obsolete("Use LoaderConfig.Current.Loader.DisableStartScreen instead.")]
            public static bool StartScreen => !LoaderConfig.Current.Loader.DisableStartScreen;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.VersionOverride instead.")]
            public static string UnityVersion => LoaderConfig.Current.UnityEngine.VersionOverride;

            [Obsolete("Use LoaderConfig.Current.Loader.DebugMode instead.")]
            public static bool IsDebug => LoaderConfig.Current.Loader.DebugMode;

            [Obsolete("Use LoaderConfig.Current.Loader.LaunchDebugger instead.")]
            public static bool UserWantsDebugger => LoaderConfig.Current.Loader.LaunchDebugger;
        }

        [Obsolete("Use LoaderConfig.Current.Console instead.")]
        public static class Console
        {
            [Obsolete("Use LoaderConfig.CoreConfig.LoaderTheme instead.")]
            public enum DisplayMode
            {
                NORMAL,
                MAGENTA,
                RAINBOW,
                RANDOMRAINBOW,
                LEMON
            };

            [Obsolete("Use LoaderConfig.Current.Loader.Theme instead.")]
            public static DisplayMode Mode => (DisplayMode)LoaderConfig.Current.Loader.Theme;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner instead.")]
            public static bool CleanUnityLogs => !LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner;

            [Obsolete("Use LoaderConfig.Current.Console.DontSetTitle instead.")]
            public static bool ShouldSetTitle => !LoaderConfig.Current.Console.DontSetTitle;

            [Obsolete("Use LoaderConfig.Current.Console.AlwaysOnTop instead.")]
            public static bool AlwaysOnTop => LoaderConfig.Current.Console.AlwaysOnTop;

            [Obsolete("Use LoaderConfig.Current.Console.Hide instead.")]
            public static bool ShouldHide => LoaderConfig.Current.Console.Hide;

            [Obsolete("Use LoaderConfig.Current.Console.HideWarnings instead.")]
            public static bool HideWarnings => LoaderConfig.Current.Console.HideWarnings;
        }

        [Obsolete("Use LoaderConfig.Current.UnityEngine instead.")]
        public static class Cpp2IL
        {
            [Obsolete("Use LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer instead.")]
            public static bool CallAnalyzer => LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector instead.")]
            public static bool NativeMethodDetector => LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector;
        }

        [Obsolete("Use LoaderConfig.Current.UnityEngine instead.")]
        public static class Il2CppAssemblyGenerator
        {
            [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceRegeneration instead.")]
            public static bool ForceRegeneration => LoaderConfig.Current.UnityEngine.ForceRegeneration;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceOfflineGeneration instead.")]
            public static bool OfflineMode => LoaderConfig.Current.UnityEngine.ForceOfflineGeneration;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion instead.")]
            public static string ForceVersion_Dumper => LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion;

            [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceGeneratorRegex instead.")]
            public static string ForceRegex => LoaderConfig.Current.UnityEngine.ForceGeneratorRegex;
        }

        [Obsolete("Use LoaderConfig.Logs instead.")]
        public static class Logger
        {
            [Obsolete("Use LoaderConfig.Current.Logs.MaxLogs instead.")]
            public static int MaxLogs => (int)LoaderConfig.Current.Logs.MaxLogs;

            [Obsolete("This option isn't used anymore. It will always return 10.")]
            public static int MaxWarnings => 10;

            [Obsolete("This option isn't used anymore. It will always return 10.")]
            public static int MaxErrors => 10;
        }

        #endregion Obsolete
    }
}
