using System.Collections.Generic;
using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class Il2CppAssemblyUnhollower : ExecutablePackageBase
    {
        internal Il2CppAssemblyUnhollower()
        {
            Version = Utils.ForceVersion_Il2CppAssemblyUnhollower();
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = SamboyAPI.Response_ForceUnhollowerVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "0.4.13.0";
            URL = "https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v" + Version + "/Il2CppAssemblyUnhollower." + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
            Output = Path.Combine(Destination, "Managed");
            ExePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
        }

        private void Save()
        {
            Config.Il2CppAssemblyUnhollowerVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Il2CppAssemblyUnhollowerVersion) || !Config.Il2CppAssemblyUnhollowerVersion.Equals(Version));

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                Logger.Msg("Il2CppAssemblyUnhollower is up to date. No Download Needed.");
                return true;
            }
            Logger.Msg("Downloading Il2CppAssemblyUnhollower...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal bool Execute()
        {
            Logger.Msg("Executing Il2CppAssemblyUnhollower...");
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
            if (!string.IsNullOrEmpty(SamboyAPI.Response_ObfuscationRegex))
                parameters.Add("--obf-regex=" + SamboyAPI.Response_ObfuscationRegex);
            return Execute(parameters.ToArray());
        }
    }
}