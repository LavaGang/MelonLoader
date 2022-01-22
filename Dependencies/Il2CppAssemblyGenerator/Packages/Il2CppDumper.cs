using System.IO;
using MelonLoader.TinyJSON;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppDumper : Models.ExecutablePackage
    {
        internal Il2CppDumper()
        {
            Name = nameof(Il2CppDumper);
            Version = "6.6.5";
            URL = $"https://github.com/Perfare/{Name}/releases/download/v{Version}/{Name}-v{Version}.zip";
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "DummyDll");
            ExeFilePath = Path.Combine(Destination, $"{Name}.exe");
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup()
            => string.IsNullOrEmpty(Config.Values.DumperVersion)
            || !Config.Values.DumperVersion.Equals(Version);
        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

        internal override void Cleanup()
        {
            string dumpcspath = Path.Combine(Destination, "dump.cs");
            if (File.Exists(dumpcspath))
                File.Delete(dumpcspath);
            base.Cleanup();
        }

        internal override bool Execute()
        {
            FixConfig();
            if (Execute(new string[] {
                Core.GameAssemblyPath,
                Path.Combine(MelonUtils.GetGameDataDirectory(), "il2cpp_data", "Metadata", "global-metadata.dat")
            }))
                return true;
            return false;
        }

        private void FixConfig()
            => File.WriteAllText(Path.Combine(Destination, "config.json"), Encoder.Encode(new Il2CppDumperConfig(), EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private class Il2CppDumperConfig
        {
            public bool DumpMethod = true;
            public bool DumpField = true;
            public bool DumpProperty = true;
            public bool DumpAttribute = true;
            public bool DumpFieldOffset = false;
            public bool DumpMethodOffset = false;
            public bool DumpTypeDefIndex = false;
            public bool GenerateDummyDll = true;
            public bool GenerateScript = false;
            public bool RequireAnyKey = false;
            public bool ForceIl2CppVersion = false;
            public float ForceVersion = 24.3f;
        }
    }
}
