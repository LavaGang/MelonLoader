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
        internal static string AssemblyFolder = null;
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
            WriteLocalConfigVersions();
            if (AssemblyGenerateCheck())
            {
                MelonModLogger.Log("Assembly Generation Needed!");

                /*
                if (UnityDependencies.ShouldExtract)
                {
                    Directory.Delete(UnityDependencies.BaseFolder, true);
                    UnityDependencies.BaseFolder = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "UnityDependencies"));
                    MelonModLogger.Log("Extracting Unity Dependencies for " + Imports.GetUnityFileVersion() + "...");
                    if (!UnityDependencies.Download())
                    {
                        MelonModLogger.LogError("Failed to Extract Unity Dependencies!");
                        return false;
                    }
                }
                */

                Cleanup();
                if (!AssemblyGenerate())
                    return false;
                Cleanup2();
                MelonModLogger.Log("Assembly Generation was Successful!");
            }
            WriteLocalConfig();
            return true;
        }

        private static void Setup()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            GameAssembly_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameAssembly.dll");

            MSCORLIB_Path = Path.Combine(Imports.GetAssemblyDirectory(), "mscorlib.dll");

            TempFolder = SetupDirectory(Path.Combine(Path.GetTempPath(), "MelonLoader"));
            TempFolder_UnityDependencies = SetupDirectory(Path.Combine(TempFolder, "UnityDependencies"));

            AssemblyFolder = Imports.GetAssemblyDirectory();

            BaseFolder = SetupDirectory(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader"), "AssemblyGenerator"));

            Il2CppDumper.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppDumper"));
            Il2CppDumper.TempFolder = TempFolder;
            Il2CppDumper.OutputDirectory = SetupDirectory(Path.Combine(Il2CppDumper.BaseFolder, "DummyDll"));
            Il2CppDumper.ZipName = "Il2CppDumper.zip";
            Il2CppDumper.FileName = "Il2CppDumper.exe";
            Il2CppDumper.TypeName = "Il2CppDumper.Program";
            Il2CppDumper.UseGitHubRepo = true;
            Il2CppDumper.GitHubRepo_User = "Prefare";
            Il2CppDumper.GithubRepo_Project = "Il2CppDumper";

            Il2CppAssemblyUnhollower.BaseFolder = SetupDirectory(Path.Combine(BaseFolder, "Il2CppAssemblyUnhollower"));
            Il2CppAssemblyUnhollower.TempFolder = TempFolder;
            Il2CppAssemblyUnhollower.OutputDirectory = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "Output"));
            Il2CppAssemblyUnhollower.ZipName = "Il2CppAssemblyUnhollower.zip";
            Il2CppAssemblyUnhollower.FileName = "Il2CppAssemblyUnhollower.exe";
            Il2CppAssemblyUnhollower.TypeName = "AssemblyUnhollower.Program";
            Il2CppAssemblyUnhollower.isIl2CppAssemblyUnhollower = true;
            Il2CppAssemblyUnhollower.UseGitHubRepo = true;
            Il2CppAssemblyUnhollower.GitHubRepo_User = "knah";
            Il2CppAssemblyUnhollower.GithubRepo_Project = "Il2CppAssemblyUnhollower";

            UnityDependencies.BaseFolder = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "UnityDependencies"));
            UnityDependencies.TempFolder = TempFolder_UnityDependencies;
            UnityDependencies.Version = Imports.GetUnityFileVersion();
            UnityDependencies.ZipName = (UnityDependencies.Version + ".zip");
            UnityDependencies.URL = "";

            localConfigPath = Path.Combine(BaseFolder, "config.json");
            if (File.Exists(localConfigPath))
                localConfig = Decoder.Decode(File.ReadAllText(localConfigPath)).Make<LocalConfig>();

            tempConfigPath = Path.Combine(TempFolder, "config.json");
            if (File.Exists(tempConfigPath))
                tempConfig = Decoder.Decode(File.ReadAllText(tempConfigPath)).Make<TempConfig>();
        }

        private static bool DownloadCheck()
        {
            if (string.IsNullOrEmpty(tempConfig.Version_Il2CppDumper) || Il2CppDumper.DownloadCheck())
                Il2CppDumper.ShouldDownload = true;
            if (string.IsNullOrEmpty(tempConfig.Version_Il2CppAssemblyUnhollower) || Il2CppAssemblyUnhollower.DownloadCheck())
                Il2CppAssemblyUnhollower.ShouldDownload = true;
            if (!tempConfig.UnityDependencies.Contains(UnityDependencies.Version))
                UnityDependencies.ShouldDownload = true;
            return (Il2CppDumper.ShouldDownload || Il2CppAssemblyUnhollower.ShouldDownload || UnityDependencies.ShouldDownload);
        }
        private static bool Download()
        {
            /*
            if (Il2CppDumper.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppDumper...");
                if (!Il2CppDumper.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppDumper!");
                    return false;
                }
                tempConfig.Version_Il2CppDumper = Il2CppDumper.Version;
            }
            */

            if (Il2CppAssemblyUnhollower.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppAssemblyUnhollower...");
                if (!Il2CppAssemblyUnhollower.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppAssemblyUnhollower!");
                    return false;
                }
                tempConfig.Version_Il2CppAssemblyUnhollower = Il2CppAssemblyUnhollower.Version;
            }

            /*
            if (UnityDependencies.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Unity Dependencies for " + Imports.GetUnityFileVersion() + "...");
                if (!UnityDependencies.Download())
                {
                    MelonModLogger.LogError("Failed to Download Unity Dependencies!");
                    return false;
                }
                tempConfig.UnityDependencies.Add(UnityDependencies.Version);
            }
            */

            return true;
        }

        private static bool ExtractCheck()
        {
            if (string.IsNullOrEmpty(localConfig.Version_Il2CppDumper) || string.IsNullOrEmpty(tempConfig.Version_Il2CppDumper) || (localConfig.Version_Il2CppDumper != tempConfig.Version_Il2CppDumper) || Il2CppDumper.ExtractCheck())
                Il2CppDumper.ShouldExtract = true;
            if (string.IsNullOrEmpty(localConfig.Version_Il2CppAssemblyUnhollower) || string.IsNullOrEmpty(tempConfig.Version_Il2CppAssemblyUnhollower) || (localConfig.Version_Il2CppAssemblyUnhollower != tempConfig.Version_Il2CppAssemblyUnhollower) || Il2CppAssemblyUnhollower.ExtractCheck())
            {
                UnityDependencies.ShouldExtract = true;
                Il2CppAssemblyUnhollower.ShouldExtract = true;
            }
            return (Il2CppDumper.ShouldExtract || Il2CppAssemblyUnhollower.ShouldExtract || UnityDependencies.ShouldExtract);
        }

        private static bool Extract()
        {
            /*
            if (Il2CppDumper.ShouldExtract)
            {
                MelonModLogger.Log("Extracting Il2CppDumper...");
                if (!Il2CppDumper.Extract())
                {
                    MelonModLogger.LogError("Failed to Extract Il2CppDumper!");
                    return false;
                }
            }
            */

            if (Il2CppAssemblyUnhollower.ShouldExtract)
            {
                MelonModLogger.Log("Extracting Il2CppAssemblyUnhollower...");
                if (!Il2CppAssemblyUnhollower.Extract())
                {
                    MelonModLogger.LogError("Failed to Extract Il2CppAssemblyUnhollower!");
                    return false;
                }
            }

            return true;
        }

        private static bool AssemblyGenerateCheck()
        {
            //if (string.IsNullOrEmpty(localConfig.UnityVersion) || (localConfig.UnityVersion != Imports.GetUnityFileVersion()))
            //{
            //    UnityDependencies.ShouldExtract = true;
            //    return true;
            //}
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
            {
                UnityDependencies.ShouldExtract = true;
                return true;
            }
            return false;
        }

        private static bool AssemblyGenerate()
        {
            FixIl2CppDumperConfig();

            /*
            MelonModLogger.Log("Executing Il2CppDumper...");
            if (!Il2CppDumper.Execute(new string[] {
                GameAssembly_Path,
                Path.Combine(Path.Combine(Path.Combine(Imports.GetGameDataDirectory(), "il2cpp_data"), "Metadata"), "global-metadata.dat")
            }))
            {
                MelonModLogger.LogError("Failed to Execute Il2CppDumper!");
                return false;
            }
            */

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
            }
            CleanupOutputs();
        }

        private static void CleanupOutputs()
        {
            Directory.Delete(Il2CppDumper.OutputDirectory, true);
            Il2CppDumper.OutputDirectory = SetupDirectory(Path.Combine(Il2CppDumper.BaseFolder, "DummyDll"));
            
            Directory.Delete(Il2CppAssemblyUnhollower.OutputDirectory, true);
            Il2CppAssemblyUnhollower.OutputDirectory = SetupDirectory(Path.Combine(Il2CppAssemblyUnhollower.BaseFolder, "Output"));
        }

        private static void Cleanup2()
        {
            string[] files = Directory.GetFiles(Il2CppAssemblyUnhollower.OutputDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                localConfig.OldFiles.Clear();
                for (int i = 0; i < files.Count(); i++)
                {
                    string file = files[i];
                    if (!string.IsNullOrEmpty(file))
                    {
                        string filename = Path.GetFileName(file);
                        localConfig.OldFiles.Add(filename);
                        File.Move(file, Path.Combine(AssemblyFolder, filename));
                    }
                }
            }
            CleanupOutputs();
        }

        private static string SetupDirectory(string path) { if (!Directory.Exists(path)) Directory.CreateDirectory(path); return path; }
        private static void FixIl2CppDumperConfig() => File.WriteAllText(Path.Combine(Il2CppDumper.BaseFolder, "config.json"), Encoder.Encode(il2cppConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private static void WriteTempConfig() => File.WriteAllText(tempConfigPath, Encoder.Encode(tempConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        private static void WriteLocalConfig()
        {
            WriteLocalConfigVersions();
            localConfig.UnityVersion = Imports.GetUnityFileVersion();
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
        private static void WriteLocalConfigVersions()
        {
            localConfig.Version_Il2CppDumper = tempConfig.Version_Il2CppDumper;
            localConfig.Version_Il2CppAssemblyUnhollower = tempConfig.Version_Il2CppAssemblyUnhollower;
            File.WriteAllText(localConfigPath, Encoder.Encode(localConfig, EncodeOptions.NoTypeHints | EncodeOptions.PrettyPrint));
        }
    }

    internal class Package
    {
        internal string Version = null;
        internal string BaseFolder = null;
        internal string TempFolder = null;
        internal string ZipName = null;
        internal string FileName = null;
        internal string URL = null;
        internal string GitHubRepo_User = null;
        internal string GithubRepo_Project = null;
        internal bool UseGitHubRepo = false;
        internal bool ShouldDownload = false;
        internal bool ShouldExtract = false;

        internal bool DownloadCheck()
        {
            string filepath = Path.Combine(TempFolder, ZipName);
            if (!File.Exists(filepath))
                return true;
            return false;
        }

        internal bool Download()
        {
            if (UseGitHubRepo)
            {
                // "http://api.github.com/repos/<GitHubRepo_User>/<GithubRepo_Project>/releases/latest"
                // "http://github.com/<GitHubRepo_User>/<GithubRepo_Project>/releases/download/" + githubVersion + ZipName

                WebRequest request = WebRequest.Create("http://api.github.com/repos/" + GitHubRepo_User + "/" + GithubRepo_Project + "/releases/latest");
                request.Credentials = CredentialCache.DefaultCredentials;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        MelonModLogger.Log(response.GetResponseStream().ToString());
                    }
                }
            }
            else
            {

            }
            return true;
        }

        internal bool ExtractCheck()
        {
            string filepath = Path.Combine(BaseFolder, FileName);
            if (!File.Exists(filepath))
                return true;
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
