using System.IO;
using System.Diagnostics;
using System;
using MelonLoader.Properties;

namespace MelonLoader.Utils
{
    public static class MelonEnvironment
    {
        private static bool Is_ALPHA_PreRelease = true;

        public const string OurRuntimeName =
#if !NET6_0
            "net35";
#else
            "net6";
#endif

        public static string HashCode { get; private set; }
        public static bool IsDotnetRuntime { get; } = OurRuntimeName == "net6";
        public static bool IsMonoRuntime { get; } = !IsDotnetRuntime;

        public static string MelonBaseDirectory => LoaderConfig.Current.Loader.BaseDirectory;

        public static string MelonLoaderDirectory { get; } = Path.Combine(MelonBaseDirectory, "MelonLoader");

        public static string ApplicationExecutablePath { get; } = Process.GetCurrentProcess().MainModule.FileName;
        public static string ApplicationExecutableName { get; } = Path.GetFileNameWithoutExtension(ApplicationExecutablePath);
        public static string ApplicationRootDirectory { get; } = Path.GetDirectoryName(ApplicationExecutablePath);

        public static string ModsDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Mods");
        public static string PluginsDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Plugins");
        public static string UserLibsDirectory { get; } = Path.Combine(MelonLoaderDirectory, "UserLibs");
        public static string UserDataDirectory { get; } = Path.Combine(MelonLoaderDirectory, "UserData");
        public static string LoadersDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Loaders");

        public static string MelonLoaderLogsDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Logs");
        public static string DependenciesDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Dependencies");

        public static string EngineModulesDirectory { get; } = Path.Combine(DependenciesDirectory, "Engines");
        public static string RuntimeModulesDirectory { get; } = Path.Combine(DependenciesDirectory, "Runtimes");

        public static string OurRuntimeDirectory { get; } = Path.Combine(MelonLoaderDirectory, OurRuntimeName);

        public static MelonPlatformAttribute.CompatiblePlatforms CurrentPlatform { get; private set; }
        public static MelonPlatformDomainAttribute.CompatibleDomains CurrentDomain { get; private set; }
        public static MelonApplicationAttribute CurrentApplicationAttribute { get; private set; } = new();

        public class EngineInfo
        {
            public string Name;
            public string Version;
            public string Variant;
        }
        public static EngineInfo CurrentEngineInfo { get; private set; } = new();

        public class ApplicationInfo
        {
            public string Name;
            public string Developer;
            public string Version;
        }
        public static ApplicationInfo CurrentApplicationInfo { get; private set; } = new();

        public static string GetVersionString()
        {
            var lemon = LoaderConfig.Current.Loader.Theme == LoaderConfig.CoreConfig.LoaderTheme.Lemon;
            var versionStr = $"{(lemon ? "Lemon" : "Melon")}Loader " +
                             $"v{BuildInfo.Version} " +
                             $"{(Is_ALPHA_PreRelease ? "ALPHA Pre-Release" : "Open-Beta")}";
            return versionStr;
        }

        internal static void SetEngineInfo(string name, string version, string variant = null)
        {
            CurrentEngineInfo.Name = name;
            CurrentEngineInfo.Version = version;
            CurrentEngineInfo.Variant = variant;
        }

        internal static void SetApplicationInfo(string developer, string name, string version)
        {
            CurrentApplicationInfo.Name = name;
            CurrentApplicationInfo.Developer = developer;
            CurrentApplicationInfo.Version = version;

            CurrentApplicationAttribute.Name = name;
            CurrentApplicationAttribute.Developer = developer;
        }

        internal static void Initialize(AppDomain domain)
        {
            HashCode = MelonUtils.ComputeSimpleSHA256Hash(typeof(MelonEnvironment).Assembly.Location);

            if (IsMonoRuntime)
                MelonUtils.SetCurrentDomainBaseDirectory(ApplicationRootDirectory, domain);

            if (!Directory.Exists(MelonLoaderDirectory))
                Directory.CreateDirectory(MelonLoaderDirectory);

            if (!Directory.Exists(MelonLoaderLogsDirectory))
                Directory.CreateDirectory(MelonLoaderLogsDirectory);

            if (!Directory.Exists(LoadersDirectory))
                Directory.CreateDirectory(LoadersDirectory);

            if (!Directory.Exists(UserDataDirectory))
                Directory.CreateDirectory(UserDataDirectory);

            if (!Directory.Exists(UserLibsDirectory))
                Directory.CreateDirectory(UserLibsDirectory);
            OsUtils.AddNativeDLLDirectory(UserLibsDirectory);

            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);

            if (!Directory.Exists(ModsDirectory))
                Directory.CreateDirectory(ModsDirectory);

            if (!Directory.Exists(DependenciesDirectory))
                Directory.CreateDirectory(DependenciesDirectory);

            if (!Directory.Exists(EngineModulesDirectory))
                Directory.CreateDirectory(EngineModulesDirectory);

            if (!Directory.Exists(RuntimeModulesDirectory))
                Directory.CreateDirectory(RuntimeModulesDirectory);

            CurrentPlatform = OsUtils.Is32Bit ? MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X86 : MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64;
            CurrentDomain = IsDotnetRuntime ? MelonPlatformDomainAttribute.CompatibleDomains.DOTNET : MelonPlatformDomainAttribute.CompatibleDomains.MONO;

            SetEngineInfo("UNKNOWN", "0.0.0");
            SetApplicationInfo("UNKNOWN", "UNKNOWN", "0.0.0");
        }

        internal static void WelcomeMessage()
        {
            MelonUtils.SetConsoleTitle($"{GetVersionString()} - {CurrentApplicationInfo.Name} {CurrentApplicationInfo.Version ?? ""}");

            MelonLogger.WriteSpacer();
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect(GetVersionString());
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"OS: {OsUtils.GetOSVersion()}");
            MelonLogger.MsgDirect($"Arch: {(OsUtils.Is32Bit ? "x86" : "x64")}");
            MelonLogger.MsgDirect($"Runtime: {OurRuntimeName}");
            MelonLogger.MsgDirect($"Hash Code: {HashCode}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"Engine: {CurrentEngineInfo.Name} {CurrentEngineInfo.Version}");
            MelonLogger.MsgDirect($"Engine Version: {CurrentEngineInfo.Version}");
            if (!string.IsNullOrEmpty(CurrentEngineInfo.Variant))
                MelonLogger.MsgDirect($"Engine Variant: {CurrentEngineInfo.Variant}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"Application Name: {CurrentApplicationInfo.Name}");
            MelonLogger.MsgDirect($"Application Developer: {CurrentApplicationInfo.Name}");
            MelonLogger.MsgDirect($"Application Version: {CurrentApplicationInfo.Version}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect("Command-Line: ");
            foreach (var pair in MelonLaunchOptions.InternalArguments)
                if (string.IsNullOrEmpty(pair.Value))
                    MelonLogger.MsgDirect($"   {pair.Key}");
                else
                    MelonLogger.MsgDirect($"   {pair.Key} = {pair.Value}");
            MelonLogger.MsgDirect("------------------------------");
        }
    }
}