using System.Collections.Generic;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Il2CppAssemblyUnhollower : ExecutablePackageBase
    {
        internal Il2CppAssemblyUnhollower()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.ReturnedInfo.ForceUnhollowerVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "0.4.14.0";
            URL = "https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v" + Version + "/Il2CppAssemblyUnhollower." + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
            Output = Path.Combine(Destination, "Managed");
            ExePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
        }

        private void Save()
        {
            Config.Values.UnhollowerVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Values.UnhollowerVersion) || !Config.Values.UnhollowerVersion.Equals(Version));

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
            List<string> parameters = new List<string>();
            parameters.Add($"--input={ Core.dumper.Output }");
            parameters.Add($"--output={ Output }");
            parameters.Add($"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }");
            parameters.Add($"--unity={ Core.unitydependencies.Destination }");
            parameters.Add($"--gameassembly={ Core.GameAssemblyPath }");
            if (!string.IsNullOrEmpty(Core.deobfuscationMap.Version))
                parameters.Add($"--rename-map={ Path.Combine(Core.deobfuscationMap.Destination, Core.deobfuscationMap.NewFileName) }");
            parameters.Add("--blacklist-assembly=Mono.Security");
            parameters.Add("--blacklist-assembly=Newtonsoft.Json");
            parameters.Add("--blacklist-assembly=Valve.Newtonsoft.Json");
            if (!string.IsNullOrEmpty(Core.deobfuscationMap.ObfuscationRegex))
                parameters.Add($"--obf-regex={ Core.deobfuscationMap.ObfuscationRegex }");
            return Execute(parameters.ToArray());
        }
    }
}