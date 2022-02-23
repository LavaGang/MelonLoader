using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppAssemblyUnhollower : Models.ExecutablePackage
    {
        internal Il2CppAssemblyUnhollower()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceUnhollowerVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "0.4.18.0";

            Name = nameof(Il2CppAssemblyUnhollower);
            URL = $"https://github.com/knah/{Name}/releases/download/v{Version}/{Name}.{Version}.zip";
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Managed");
            ExeFilePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup()
            => string.IsNullOrEmpty(Config.Values.UnhollowerVersion) 
            || !Config.Values.UnhollowerVersion.Equals(Version);

        internal override void Save()
            => Save(ref Config.Values.UnhollowerVersion);

        internal override bool Execute()
        {
            if (Execute(new string[] {
                $"--input={ Core.dumper.OutputFolder }",
                $"--output={ OutputFolder }",
                $"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }",
                $"--unity={ Core.unitydependencies.Destination }",
                $"--gameassembly={ Core.GameAssemblyPath }",
                string.IsNullOrEmpty(Core.deobfuscationMap.Version) ? string.Empty : $"--rename-map={ Core.deobfuscationMap.Destination }",
                string.IsNullOrEmpty(Core.deobfuscationRegex.Regex) ? string.Empty : $"--obf-regex={ Core.deobfuscationRegex.Regex }",
                "--add-prefix-to=ICSharpCode",
                "--add-prefix-to=Newtonsoft",
                "--add-prefix-to=TinyJson",
                "--add-prefix-to=Valve.Newtonsoft"
            }))
                return true;
            return false;
        }
    }
}
