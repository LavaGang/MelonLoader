using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class Il2CppAssemblyUnhollower : ExecutablePackageBase
    {
        internal Il2CppAssemblyUnhollower()
        {
            Version = Utils.ForceVersion_Il2CppAssemblyUnhollower();
            if (string.IsNullOrEmpty(Version))
                Version = "0.4.10.0";
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
            string mscorlib_path = Path.Combine(Core.ManagedPath, "mscorlib.dll");
            return Execute(new string[] {
                ("--input=" + Core.il2cppdumper.Output),
                ("--output=" + Output),
                ("--mscorlib=" + mscorlib_path),
                ("--unity=" + Core.unitydependencies.Destination),
                "--gameassembly=" + Core.GameAssemblyPath,
                "--blacklist-assembly=Mono.Security",
                "--blacklist-assembly=Newtonsoft.Json",
                "--blacklist-assembly=Valve.Newtonsoft.Json"});
        }
    }
}