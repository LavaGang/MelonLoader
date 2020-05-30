using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using TinyJSON;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        private static string GameAssembly_Path = null;
        private static string MSCORLIB_Path = null;
        internal static string BaseFolder = null;
        internal static string TempFolder = null;
        internal static string TempFolder_UnityDependencies = null;
        private static Package UnityDependencies = new Package();
        private static Executable_Package Il2CppDumper = new Executable_Package();
        private static Executable_Package Il2CppAssemblyUnhollower = new Executable_Package();
        private static string localConfigPath = null;
        private static LocalConfig localConfig = new LocalConfig();
        private static string tempConfigPath = null;
        private static TempConfig tempConfig = new TempConfig();
        private static Il2CppConfig il2cppConfig = new Il2CppConfig();

        internal static bool Initialize()
        {
            Setup();
            if (DownloadCheck() && !Download())
                return false;
            WriteTempConfig();
            if (ExtractCheck() && !Extract())
                return false;
            if (AssemblyGenerateCheck())
            {
                Cleanup_OldFiles();
                if (!AssemblyGenerate())
                    return false;
                Cleanup();
            }
            WriteLocalConfig();
            return true;
        }

        private static void Setup()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            // Setup GameAssembly_Path
            GameAssembly_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameAssembly.dll");

            // Setup MSCORLIB_Path
            MSCORLIB_Path = Path.Combine(Imports.GetAssemblyDirectory(), "mscorlib.dll");

            // Setup TempFolder
            TempFolder = SetupDirectory(Path.Combine(Path.GetTempPath(), "MelonLoader"));
            TempFolder_UnityDependencies = SetupDirectory(Path.Combine(TempFolder, "UnityDependencies"));

            // Setup BaseFolder
            BaseFolder = SetupDirectory(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader"), "AssemblyGenerator"));

            // Setup Il2CppDumper
            Il2CppDumper.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppDumper"));
            Il2CppDumper.OutputDirectory = SetupDirectory(Path.Combine(Il2CppDumper.BaseFolder, "DummyDll"));
            Il2CppDumper.AssemblyName = "Il2CppDumper.exe";
            Il2CppDumper.TypeName = "Il2CppDumper.Program";

            // Setup Il2CppAssemblyUnhollower
            Il2CppAssemblyUnhollower.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppAssemblyUnhollower"));
            Il2CppAssemblyUnhollower.OutputDirectory = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "Output"));
            Il2CppAssemblyUnhollower.AssemblyName = "Il2CppAssemblyUnhollower.exe";
            Il2CppAssemblyUnhollower.TypeName = "AssemblyUnhollower.Program";
            Il2CppAssemblyUnhollower.isIl2CppAssemblyUnhollower = true;

            // Setup UnityDependencies
            UnityDependencies.BaseFolder = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "UnityDependencies"));

            // Setup Local Config
            localConfigPath = Path.Combine(BaseFolder, "config.json");
            if (File.Exists(localConfigPath))
                localConfig = Decoder.Decode(File.ReadAllText(localConfigPath)).Make<LocalConfig>();

            // Setup Temp Config
            tempConfigPath = Path.Combine(TempFolder, "config.json");
            if (File.Exists(tempConfigPath))
                tempConfig = Decoder.Decode(File.ReadAllText(tempConfigPath)).Make<TempConfig>();
        }

        private static bool DownloadCheck() => (Il2CppDumper.DownloadCheck() || Il2CppAssemblyUnhollower.DownloadCheck() || UnityDependencies.DownloadCheck());
        private static bool Download()
        {
            if (Il2CppDumper.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppDumper...");
                if (!Il2CppDumper.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppDumper!");
                    return false;
                }
            }
            if (Il2CppAssemblyUnhollower.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppAssemblyUnhollower...");
                if (!Il2CppAssemblyUnhollower.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppAssemblyUnhollower!");
                    return false;
                }
            }
            if (UnityDependencies.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Unity Dependencies for " + Imports.GetUnityFileVersion() + "...");
                if (!UnityDependencies.Download())
                {
                    MelonModLogger.LogError("Failed to Download Unity Dependencies!");
                    return false;
                }
            }
            return true;
        }

        private static bool ExtractCheck() => (Il2CppDumper.ExtractCheck() || Il2CppAssemblyUnhollower.ExtractCheck() || UnityDependencies.ExtractCheck());
        private static bool Extract()
        {
            if (Il2CppDumper.ShouldExtract)
            {
                MelonModLogger.Log("Extracting Il2CppDumper...");
                if (!Il2CppDumper.Download())
                {
                    MelonModLogger.LogError("Failed to Extract Il2CppDumper!");
                    return false;
                }
            }
            if (Il2CppAssemblyUnhollower.ShouldExtract)
            {
                MelonModLogger.Log("Extracting Il2CppAssemblyUnhollower...");
                if (!Il2CppAssemblyUnhollower.Download())
                {
                    MelonModLogger.LogError("Failed to Extract Il2CppAssemblyUnhollower!");
                    return false;
                }
            }
            if (UnityDependencies.ShouldExtract)
            {
                MelonModLogger.Log("Extracting Unity Dependencies for " + Imports.GetUnityFileVersion() + "...");
                if (!UnityDependencies.Download())
                {
                    MelonModLogger.LogError("Failed to Extract Unity Dependencies!");
                    return false;
                }
            }
            return true;
        }

        private static bool AssemblyGenerateCheck()
        {
            if (string.IsNullOrEmpty(localConfig.UnityVersion) || (localConfig.UnityVersion != Imports.GetUnityFileVersion()))
                return true;
            string game_assembly_hash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(GameAssembly_Path))
                {
                    var hash = md5.ComputeHash(stream);
                    game_assembly_hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            if (string.IsNullOrEmpty(localConfig.GameAssemblyHash) || (game_assembly_hash != localConfig.GameAssemblyHash))
                return true;
            return false;
        }

        private static bool AssemblyGenerate()
        {
            FixIl2CppDumperConfig();
            MelonModLogger.Log("Executing Il2CppDumper...");
            if (!Il2CppDumper.Execute(new string[] {
                GameAssembly_Path,
                Path.Combine(Path.Combine(Path.Combine(Imports.GetGameDataDirectory(), "il2cpp_data"), "Metadata"), "global-metadata.dat")
            }))
            {
                MelonModLogger.LogError("Failed to Execute Il2CppDumper!");
                return false;
            }
            MelonModLogger.Log("Executing Il2CppAssemblyUnhollower...");
            if (!Il2CppAssemblyUnhollower.Execute(new string[] {
                ("--input=" + Il2CppDumper.OutputDirectory),
                ("--output=" + Il2CppAssemblyUnhollower.OutputDirectory),
                ("--mscorlib=" + MSCORLIB_Path),
                ("--unity=" + UnityDependencies.BaseFolder),
                "--blacklist-assembly=Mono.Security",
                "--blacklist-assembly=Newtonsoft.Json",
                "--blacklist-assembly=Valve.Newtonsoft.Json"
            }))
            {
                MelonModLogger.LogError("Failed to Execute Il2CppAssemblyUnhollower!");
                return false;
            }
            return true;
        }

        private static void Cleanup_OldFiles()
        {
            if (localConfig.OldFiles.Count() > 0)
            {

            }
        }

        private static void Cleanup()
        {

        }

        private static string SetupDirectory(string path) { if (!Directory.Exists(path)) Directory.CreateDirectory(path); return path; }
        private static void FixIl2CppDumperConfig() => File.WriteAllText(Path.Combine(Il2CppDumper.BaseFolder, "config.json"), Encoder.Encode(il2cppConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private static void WriteLocalConfig() => File.WriteAllText(localConfigPath, Encoder.Encode(localConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private static void WriteTempConfig() => File.WriteAllText(tempConfigPath, Encoder.Encode(tempConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
    }

    internal class Package
    {
        internal string BaseFolder = null;
        internal bool ShouldDownload = false;
        internal bool ShouldExtract = false;

        internal bool DownloadCheck()
        {

            return false;
        }

        internal bool Download()
        {

            return true;
        }

        internal bool ExtractCheck()
        {

            return false;
        }

        internal bool Extract()
        {

            return true;
        }
    }

    internal class Executable_Package : Package
    {
        internal string OutputDirectory = null;
        internal string AssemblyName = null;
        internal string TypeName = null;
        internal bool isIl2CppAssemblyUnhollower = false;

        internal bool Execute(string[] argv)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(Path.Combine(BaseFolder, AssemblyName));
                if (a != null)
                {
                    Type b = a.GetType(TypeName);
                    if (b != null)
                    {
                        MethodInfo c = null;
                        if (isIl2CppAssemblyUnhollower)
                            c = b.GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => ((x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string[]))));
                        else
                            c = b.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (c != null)
                        {
                            Directory.SetCurrentDirectory(BaseFolder);
                            c.Invoke(null, new object[] { argv });
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception e) { MelonModLogger.LogError(e.ToString()); return false; }
            return true;
        }
    }

    internal class Il2CppConfig
    {
        public bool DumpMethod = true;
        public bool DumpField = true;
        public bool DumpProperty = true;
        public bool DumpAttribute = true;
        public bool DumpFieldOffset = false;
        public bool DumpMethodOffset = false;
        public bool DumpTypeDefIndex = false;
        public bool DummyDll = true;
        public bool MakeFunction = true;
        public bool RequireAnyKey = false;
        public bool ForceIl2CppVersion = false;
        public float ForceVersion = 24.3f;
    }

    internal class LocalConfig
    {
        public string UnityVersion = null;
        public string GameAssemblyHash = null;
        public string Version_Il2CppDumper = null;
        public string Version_Il2CppAssemblyUnhollower = null;
        public List<string> OldFiles = new List<string>();
    }

    internal class TempConfig
    {
        public string Version_Il2CppDumper = null;
        public string Version_Il2CppAssemblyUnhollower = null;
        public List<string> UnityDependencies = new List<string>();
    }
}
