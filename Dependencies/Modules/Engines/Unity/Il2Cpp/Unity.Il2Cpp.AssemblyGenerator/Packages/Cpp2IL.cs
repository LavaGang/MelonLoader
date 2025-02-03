﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MelonLoader.Utils;
using Semver;

namespace MelonLoader.Engine.Unity.Packages
{
    internal class Cpp2IL : Models.ExecutablePackage
    {
        internal static SemVersion NetCoreMinVersion = SemVersion.Parse("2022.1.0-pre-release.18");
        internal SemVersion VersionSem;
        private string BaseFolder;

        private static string ReleaseName =>
            OsUtils.IsWindows ? "Windows" : OsUtils.IsUnix ? "Linux" : "OSX";
		
        internal Cpp2IL() { }
        internal Cpp2IL(string BasePath)
        {
            //Version = LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceDumperVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = $"2022.1.0-pre-release.19";
            VersionSem = SemVersion.Parse(Version);

            Name = nameof(Cpp2IL);
            
            var filename = Name;
#if WINDOWS
            filename += ".exe";
#endif

            BaseFolder = Path.Combine(BasePath, Name);
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);

            FilePath =
                ExeFilePath =
                Destination =
                Path.Combine(BaseFolder, filename);

            OutputFolder = Path.Combine(BaseFolder, "cpp2il_out");

            URL = $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-{ReleaseName}";
#if WINDOWS
            URL += ".exe";
#endif
        }

        internal override bool ShouldSetup() 
            => string.IsNullOrEmpty(Config.Values.DumperVersion) 
            || !Config.Values.DumperVersion.Equals(Version);

        internal override void Cleanup() { }

        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

        internal virtual bool Execute(string GameAssemblyPath)
            => Execute([
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,

                "--game-path",
                "\"" + Path.GetDirectoryName(GameAssemblyPath) + "\"",

                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--output-as",
                "dummydll",

                "--use-processor",
                "attributeanalyzer",
                "attributeinjector",
                //LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer ? "callanalyzer" : string.Empty,
                //LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector ? "nativemethoddetector" : string.Empty,
                //"deobfmap",
                //"stablenamer",

            ], false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"},
            });
    }
}
