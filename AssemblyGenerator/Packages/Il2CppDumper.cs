﻿using System.IO;
using MelonLoader.TinyJSON;

namespace MelonLoader.AssemblyGenerator
{
    internal class Il2CppDumper : ExecutablePackageBase
    {
        internal Il2CppDumper()
        {
            Version = Utils.ForceVersion_Il2CppDumper();
            if (string.IsNullOrEmpty(Version))
                Version = "6.4.12";
            URL = "https://github.com/Perfare/Il2CppDumper/releases/download/v" + Version + "/Il2CppDumper-v" + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppDumper");
            Output = Path.Combine(Destination, "DummyDll");
            ExePath = Path.Combine(Destination, "Il2CppDumper.exe");
        }

        private void Save()
        {
            Config.Il2CppDumperVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Il2CppDumperVersion) || !Config.Il2CppDumperVersion.Equals(Version));

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                Logger.Msg("Il2CppDumper is up to date. No Download Needed.");
                return true;
            }
            Logger.Msg("Downloading Il2CppDumper...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal bool Execute()
        {
            FixConfig();
            Logger.Msg("Executing Il2CppDumper...");
            string metadata_path = Path.Combine(Path.Combine(Path.GetDirectoryName(Utils.GetConfigDirectory()), "Metadata"), "global-metadata.dat");
            return Execute(new string[] { Core.GameAssemblyPath, metadata_path });
        }

        private void FixConfig() => File.WriteAllText(Path.Combine(Destination, "config.json"), Encoder.Encode(new Il2CppDumperConfig(), EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
    }

    internal class Il2CppDumperConfig
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