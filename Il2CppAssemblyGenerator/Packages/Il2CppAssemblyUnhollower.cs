using System.Collections.Generic;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppAssemblyUnhollower : ExecutablePackageBase
    {
        internal Il2CppAssemblyUnhollower()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceUnhollowerVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                //Version = "0.4.16.1";
                Version = "0.4.15.4";
            URL = $"https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v{Version}/Il2CppAssemblyUnhollower.{Version}.zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
            Output = Path.Combine(Destination, "Managed");
            ExePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
        }

        private void Save()
        {
            Config.Values.UnhollowerVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => string.IsNullOrEmpty(Config.Values.UnhollowerVersion) || !Config.Values.UnhollowerVersion.Equals(Version);

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Il2CppAssemblyUnhollower is up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Il2CppAssemblyUnhollower...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal bool Execute()
        {
            MelonLogger.Msg("Executing Il2CppAssemblyUnhollower...");
            return Execute(new string[] {
                $"--input={ Core.dumper.Output }",
                $"--output={ Output }",
                $"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }",
                $"--unity={ Core.unitydependencies.Destination }",
                $"--gameassembly={ Core.GameAssemblyPath }",
                string.IsNullOrEmpty(Core.deobfuscationMap.Version) ? string.Empty : $"--rename-map={ Path.Combine(Core.deobfuscationMap.Destination, Core.deobfuscationMap.NewFileName) }",
                "--blacklist-assembly=Mono.Security",
                "--blacklist-assembly=Newtonsoft.Json",
                "--blacklist-assembly=Valve.Newtonsoft.Json",
                string.IsNullOrEmpty(Core.deobfuscationMap.ObfuscationRegex) ? string.Empty : $"--obf-regex={ Core.deobfuscationMap.ObfuscationRegex }"
            });
        }
    }
}