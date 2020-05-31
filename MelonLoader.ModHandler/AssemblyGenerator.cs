using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal static string AssemblyFolder = null;
        private static Package UnityDependencies = new Package();
        private static Executable_Package Il2CppDumper = new Executable_Package();
        private static Executable_Package Il2CppAssemblyUnhollower = new Executable_Package();
        private static string localConfigPath = null;
        private static LocalConfig localConfig = new LocalConfig();
        private static Il2CppConfig il2cppConfig = new Il2CppConfig();

        internal static bool Initialize()
        {
            Setup();
            if (AssemblyGenerateCheck())
            {
                MelonModLogger.Log("Assembly Generation Needed!");
                if (!AssemblyGenerate())
                    return false;
                Cleanup();
                MelonModLogger.Log("Assembly Generation was Successful!");
            }
            WriteLocalConfig();
            return true;
        }

        private static void Setup()
        {
            GameAssembly_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameAssembly.dll");

            MSCORLIB_Path = Path.Combine(Imports.GetAssemblyDirectory(), "mscorlib.dll");

            AssemblyFolder = Imports.GetAssemblyDirectory();

            BaseFolder = SetupDirectory(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader"), "AssemblyGenerator"));

            Il2CppDumper.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppDumper"));
            Il2CppDumper.OutputDirectory = SetupDirectory(Path.Combine(Il2CppDumper.BaseFolder, "DummyDll"));
            Il2CppDumper.FileName = "Il2CppDumper.exe";
            Il2CppDumper.TypeName = "Il2CppDumper.Program";

            Il2CppAssemblyUnhollower.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppAssemblyUnhollower"));
            Il2CppAssemblyUnhollower.OutputDirectory = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "Output"));
            Il2CppAssemblyUnhollower.FileName = "AssemblyUnhollower.exe";
            Il2CppAssemblyUnhollower.TypeName = "AssemblyUnhollower.Program";
            Il2CppAssemblyUnhollower.isIl2CppAssemblyUnhollower = true;

            UnityDependencies.BaseFolder = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "UnityDependencies"));

            localConfigPath = Path.Combine(BaseFolder, "config.json");
            if (File.Exists(localConfigPath))
                localConfig = Decoder.Decode(File.ReadAllText(localConfigPath)).Make<LocalConfig>();
        }

        private static bool AssemblyGenerateCheck()
        {
            if (string.IsNullOrEmpty(localConfig.UnityVersion) || (localConfig.UnityVersion != MelonLoader.Main.UnityVersion))
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
            ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = (Il2CppDumper.BaseFolder + "\\");
            if (!Il2CppDumper.Execute(new string[] {
                GameAssembly_Path,
                Path.Combine(Path.Combine(Path.Combine(Imports.GetGameDataDirectory(), "il2cpp_data"), "Metadata"), "global-metadata.dat")
            }))
            {
                MelonModLogger.LogError("Failed to Execute Il2CppDumper!");
                return false;
            }
            ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = Imports.GetGameDirectory();

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

        private static void Cleanup()
        {
            if (localConfig.OldFiles.Count() > 0)
            {
                for (int i = 0; i < localConfig.OldFiles.Count(); i++)
                {
                    string oldFile = localConfig.OldFiles[i];
                    if (!string.IsNullOrEmpty(oldFile))
                    {
                        string oldFilePath = Path.Combine(AssemblyFolder, oldFile);
                        if (File.Exists(oldFilePath))
                            File.Delete(oldFilePath);
                    }
                }
                localConfig.OldFiles.Clear();
            }
            string[] files = Directory.GetFiles(Il2CppAssemblyUnhollower.OutputDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    string file = files[i];
                    if (!string.IsNullOrEmpty(file))
                    {
                        string filename = Path.GetFileName(file);
                        localConfig.OldFiles.Add(filename);
                        File.Copy(file, Path.Combine(AssemblyFolder, filename), true);
                    }
                }
            }
            Directory.Delete(Il2CppAssemblyUnhollower.OutputDirectory, true);
            Il2CppAssemblyUnhollower.OutputDirectory = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "Output"));
        }

        private static string SetupDirectory(string path) { if (!Directory.Exists(path)) Directory.CreateDirectory(path); return path; }
        private static void FixIl2CppDumperConfig() => File.WriteAllText(Path.Combine(Il2CppDumper.BaseFolder, "config.json"), Encoder.Encode(il2cppConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private static void WriteLocalConfig()
        {
            localConfig.UnityVersion = MelonLoader.Main.UnityVersion;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(GameAssembly_Path))
                {
                    var hash = md5.ComputeHash(stream);
                    localConfig.GameAssemblyHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            File.WriteAllText(localConfigPath, Encoder.Encode(localConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        }
    }

    internal class Package
    {
        internal string Version = null;
        internal string BaseFolder = null;
        internal string FileName = null;
    }

    internal class Executable_Package : Package
    {
        internal string OutputDirectory = null;
        internal string TypeName = null;
        internal bool isIl2CppAssemblyUnhollower = false;

        internal bool Execute(string[] argv)
        {
            string assembly_path = Path.Combine(BaseFolder, FileName);
            if (File.Exists(assembly_path))
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(assembly_path);
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
            }
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
        public List<string> OldFiles = new List<string>();
    }
}
