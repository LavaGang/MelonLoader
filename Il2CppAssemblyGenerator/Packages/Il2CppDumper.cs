using System.IO;
using MelonLoader.TinyJSON;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Il2CppDumper : ExecutablePackageBase
    {
#if PORT_DISABLE
        internal Il2CppDumper()
        {
            Version = MelonCommandLine.AssemblyGenerator.ForceVersion_Dumper;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.LAST_RESPONSE.ForceDumperVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "6.5.0";
            URL = "https://github.com/Perfare/Il2CppDumper/releases/download/v" + Version + "/Il2CppDumper-v" + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "Il2CppDumper");
            Output = Path.Combine(Destination, "DummyDll");
            ExePath = Path.Combine(Destination, "Il2CppDumper.exe");
        }

        private void Save()
        {
            Config.DumperVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.DumperVersion) || !Config.DumperVersion.Equals(Version));

        internal override void Cleanup()
        {
            string dumpcspath = Path.Combine(Destination, "dump.cs");
            if (File.Exists(dumpcspath))
                File.Delete(dumpcspath);
        }

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Il2CppDumper is up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Il2CppDumper...");
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
            MelonLogger.Msg("Executing Il2CppDumper...");
            string metadata_path = Path.Combine(Path.Combine(Path.Combine(string.Copy(MelonUtils.GetGameDataDirectory()), "il2cpp_data"), "Metadata"), "global-metadata.dat");
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
#else
        internal bool Execute()
        {
            MelonLogger.Msg("Executing Il2CppDumper...");
            string metadata_path = Path.Combine(Path.Combine(Path.Combine(string.Copy(MelonUtils.GetGameDataDirectory()), "il2cpp"), "Metadata"), "global-metadata.dat");
            return Main(string.Copy(MelonUtils.GetMainAssemblyLoc()), metadata_path, Destination + Path.DirectorySeparatorChar);
        }

        private bool Main(string gameAssembly, string metadata, string output)
        {
            global::Il2CppDumper.Console.LogHandler += (msg) => { MelonLogger.Msg(System.ConsoleColor.Magenta, $"[Il2CppDumper] {msg}"); };
            var config = new global::Il2CppDumper.Config();

            config.DumpMethod = true;
            config.DumpField = true;
            config.DumpProperty = true;
            config.DumpAttribute = true;
            config.DumpFieldOffset = false;
            config.DumpMethodOffset = false;
            config.DumpTypeDefIndex = false;
            config.GenerateDummyDll = true;
            config.GenerateStruct = false;
            config.RequireAnyKey = false;
            config.ForceIl2CppVersion = false;
            config.ForceVersion = 24.3f;

            string restore = Directory.GetCurrentDirectory();
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);
            Core.OverrideAppDomainBase(output);

            global::Il2CppDumper.Program.config = config;
            MelonLogger.Msg(global::Il2CppDumper.Program.config.DumpFieldOffset.ToString());

            if (!global::Il2CppDumper.Program.Init(gameAssembly, metadata, out var metadataOut, out var il2CppOut))
            {
                Directory.SetCurrentDirectory(restore);
                return false;
            }
            MelonLogger.Msg(global::Il2CppDumper.Program.config.DumpFieldOffset.ToString());


            global::Il2CppDumper.Program.Dump(metadataOut, il2CppOut, output);
            Core.OverrideAppDomainBase(restore);

            return true;
        }

        internal override void Cleanup()
        {
            string dumpcspath = Path.Combine(Destination, "dump.cs");
            if (File.Exists(dumpcspath))
                File.Delete(dumpcspath);
        }
        internal Il2CppDumper()
        {
            Destination = Path.Combine(Core.BasePath, "Il2CppDumper");
            Output = Path.Combine(Destination, "DummyDll");
        }
#endif
    }
}