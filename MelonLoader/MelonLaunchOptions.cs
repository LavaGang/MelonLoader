using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MelonLoader;

public static class MelonLaunchOptions
{
    private static readonly Dictionary<string, Action> WithoutArg = [];
    private static readonly Dictionary<string, Action<string>> WithArg = [];
    private static string[] _cmd;

    /// <summary>
    /// Dictionary of all Arguments with value (if found) that were not used by MelonLoader
    /// <para>
    /// <b>Key</b> is the argument, <b>Value</b> is the value for the argument, <c>null</c> if not found
    /// </para>
    /// </summary>
    public static Dictionary<string, string> ExternalArguments { get; private set; } = [];
    public static Dictionary<string, string> InternalArguments { get; private set; } = [];

    /// <summary>
    /// Array of All Command Line Arguments
    /// </summary>
    public static string[] CommandLineArgs
    {
        get
        {
            _cmd ??= Environment.GetCommandLineArgs();
            return _cmd;
        }
    }

    internal static void Load()
    {
        var args = CommandLineArgs;
        var maxLen = args.Length;
        for (var i = 1; i < maxLen; i++)
        {
            var fullcmd = args[i];
            if (string.IsNullOrEmpty(fullcmd))
                continue;

            // Parse Prefix
            var noPrefixCmd = fullcmd;
            if (noPrefixCmd.StartsWith("--"))
                noPrefixCmd = noPrefixCmd[2..];
            else if (noPrefixCmd.StartsWith("-"))
                noPrefixCmd = noPrefixCmd[1..];
            else
            {
                // Unknown Command, Add it to Dictionary
                ExternalArguments.Add(noPrefixCmd, null);
                continue;
            }

            // Parse Argumentless Commands
            if (WithoutArg.TryGetValue(noPrefixCmd, out var withoutArgFunc))
            {
                InternalArguments.Add(noPrefixCmd, null);
                withoutArgFunc();
                continue;
            }

            // Parse Argument
            string cmdArg = null;
            if (noPrefixCmd.Contains('='))
            {
                var split = noPrefixCmd.Split('=');
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
            if (WithArg.TryGetValue(noPrefixCmd, out var withArgFunc))
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

    [Obsolete("Use LoaderConfig.Current.Loader instead. This will be removed in a future version.", true)]
    [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's deprecated")]
    public static class Core
    {
        [Obsolete("This option isn't used anymore. This will be removed in a future version.", true)]
        public enum LoadModeEnum
        {
            NORMAL,
            DEV,
            BOTH
        }

        [Obsolete("This option isn't used anymore. It will always return NORMAL. This will be removed in a future version.", true)]
        public static LoadModeEnum LoadMode_Plugins => LoadModeEnum.NORMAL;

        [Obsolete("This option isn't used anymore. It will always return NORMAL. This will be removed in a future version.", true)]
        public static LoadModeEnum LoadMode_Mods => LoadModeEnum.NORMAL;

        [Obsolete("Use LoaderConfig.Current.Loader.ForceQuit instead. This will be removed in a future version.", true)]
        public static bool QuitFix => LoaderConfig.Current.Loader.ForceQuit;

        [Obsolete("Use LoaderConfig.Current.Loader.DisableStartScreen instead. This will be removed in a future version.", true)]
        public static bool StartScreen => !LoaderConfig.Current.Loader.DisableStartScreen;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.VersionOverride instead. This will be removed in a future version.", true)]
        public static string UnityVersion => LoaderConfig.Current.UnityEngine.VersionOverride;

        [Obsolete("Use LoaderConfig.Current.Loader.DebugMode instead. This will be removed in a future version.", true)]
        public static bool IsDebug => LoaderConfig.Current.Loader.DebugMode;

        [Obsolete("Use LoaderConfig.Current.Loader.LaunchDebugger instead. This will be removed in a future version.", true)]
        public static bool UserWantsDebugger => LoaderConfig.Current.Loader.LaunchDebugger;
    }

    [Obsolete("Use LoaderConfig.Current.Console instead. This will be removed in a future version.", true)]
    public static class Console
    {
        [Obsolete("Use LoaderConfig.CoreConfig.LoaderTheme instead. This will be removed in a future version.", true)]
        public enum DisplayMode
        {
            NORMAL,
            MAGENTA,
            RAINBOW,
            RANDOMRAINBOW,
            LEMON
        };

        [Obsolete("Use LoaderConfig.Current.Loader.Theme instead. This will be removed in a future version.", true)]
        public static DisplayMode Mode => (DisplayMode)LoaderConfig.Current.Loader.Theme;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner instead. This will be removed in a future version.", true)]
        public static bool CleanUnityLogs => !LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner;

        [Obsolete("Use LoaderConfig.Current.Console.DontSetTitle instead. This will be removed in a future version.", true)]
        public static bool ShouldSetTitle => !LoaderConfig.Current.Console.DontSetTitle;

        [Obsolete("Use LoaderConfig.Current.Console.AlwaysOnTop instead. This will be removed in a future version.", true)]
        public static bool AlwaysOnTop => LoaderConfig.Current.Console.AlwaysOnTop;

        [Obsolete("Use LoaderConfig.Current.Console.Hide instead. This will be removed in a future version.", true)]
        public static bool ShouldHide => LoaderConfig.Current.Console.Hide;

        [Obsolete("Use LoaderConfig.Current.Console.HideWarnings instead. This will be removed in a future version.", true)]
        public static bool HideWarnings => LoaderConfig.Current.Console.HideWarnings;
    }

    [Obsolete("Use LoaderConfig.Current.UnityEngine instead. This will be removed in a future version.", true)]
    public static class Cpp2IL
    {
        [Obsolete("Use LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer instead. This will be removed in a future version.", true)]
        public static bool CallAnalyzer => LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector instead. This will be removed in a future version.", true)]
        public static bool NativeMethodDetector => LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector;
    }

    [Obsolete("Use LoaderConfig.Current.UnityEngine instead. This will be removed in a future version.", true)]
    public static class Il2CppAssemblyGenerator
    {
        [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceRegeneration instead. This will be removed in a future version.", true)]
        public static bool ForceRegeneration => LoaderConfig.Current.UnityEngine.ForceRegeneration;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceOfflineGeneration instead. This will be removed in a future version.", true)]
        public static bool OfflineMode => LoaderConfig.Current.UnityEngine.ForceOfflineGeneration;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion instead. This will be removed in a future version.", true)]
        [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's deprecated")]
        public static string ForceVersion_Dumper => LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion;

        [Obsolete("Use LoaderConfig.Current.UnityEngine.ForceGeneratorRegex instead. This will be removed in a future version.", true)]
        public static string ForceRegex => LoaderConfig.Current.UnityEngine.ForceGeneratorRegex;
    }

    [Obsolete("Use LoaderConfig.Logs instead. This will be removed in a future version.", true)]
    public static class Logger
    {
        [Obsolete("Use LoaderConfig.Current.Logs.MaxLogs instead. This will be removed in a future version.", true)]
        public static int MaxLogs => (int)LoaderConfig.Current.Logs.MaxLogs;

        [Obsolete("This option isn't used anymore. It will always return 10. This will be removed in a future version.", true)]
        public static int MaxWarnings => 10;

        [Obsolete("This option isn't used anymore. It will always return 10. This will be removed in a future version.", true)]
        public static int MaxErrors => 10;
    }

    #endregion Obsolete
}
