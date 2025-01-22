using Semver;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages;

internal class Cpp2IL_NetFramework : Models.ExecutablePackage
{
    private static string ReleaseName => "Windows-Netframework472";
    internal Cpp2IL_NetFramework()
    {
        Version = LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion;
#if !DEBUG
        if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
            Version = RemoteAPI.Info.ForceDumperVersion;
#endif
        if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
            Version = $"2022.1.0-pre-release.15";

        Name = nameof(Cpp2IL);
        Destination = Path.Combine(Core.BasePath, Name);
        OutputFolder = Path.Combine(Destination, "cpp2il_out");

        URL = $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-{ReleaseName}.zip";
        ExeFilePath = Path.Combine(Destination, $"{Name}.exe");
        FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
    }

    internal override bool ShouldSetup()
        => string.IsNullOrEmpty(Config.Values.DumperVersion)
        || !Config.Values.DumperVersion.Equals(Version);

    internal override void Cleanup() { }

    internal override void Save()
        => Save(ref Config.Values.DumperVersion);

    internal override bool Execute()
    {
        return SemVersion.Parse(Version) <= SemVersion.Parse("2022.0.999") ? ExecuteOld() : ExecuteNew();
    }

    private bool ExecuteNew()
    {
        return Execute([
            MelonDebug.IsEnabled() ? "--verbose" : string.Empty,

            "--game-path",
            "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",

            "--exe-name",
            "\"" + Process.GetCurrentProcess().ProcessName + "\"",

            "--output-as",
            "dummydll",

            "--use-processor",
            "attributeanalyzer",
            "attributeinjector",
            LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer ? "callanalyzer" : string.Empty,
            LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector ? "nativemethoddetector" : string.Empty,
            //"deobfmap",
            //"stablenamer",

        ], false, new Dictionary<string, string>() {
            {"NO_COLOR", "1"}
        });
    }

    private bool ExecuteOld()
    {
        return Execute([
            MelonDebug.IsEnabled() ? "--verbose" : string.Empty,

            "--game-path",
            "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",

            "--exe-name",
            "\"" + Process.GetCurrentProcess().ProcessName + "\"",

            "--skip-analysis",
            "--skip-metadata-txts",
            "--disable-registration-prompts"

        ], false, new Dictionary<string, string>() {
            {"NO_COLOR", "1"}
        });
    }
}