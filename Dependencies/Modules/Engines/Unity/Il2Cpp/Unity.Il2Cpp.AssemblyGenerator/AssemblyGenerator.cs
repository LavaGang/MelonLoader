using System;
using System.IO;
using System.Net.Http;
using MelonLoader.Engine.Unity.Packages;
using MelonLoader.Properties;
using MelonLoader.Utils;

namespace MelonLoader.Engine.Unity
{
    public class AssemblyGenerator
    {
        internal static HttpClient webClient = null;

        internal static Cpp2IL cpp2il = null;
        internal static Cpp2IL_StrippedCodeRegSupport cpp2il_scrs = null;

        internal static Packages.Il2CppInterop il2cppinterop = null;
        internal static UnityDependencies unitydependencies = null;
        internal static DeobfuscationMap deobfuscationMap = null;
        internal static DeobfuscationRegex deobfuscationRegex = null;

        internal static bool AssemblyGenerationNeeded = false;
        internal static MelonLogger.Instance Logger;

        public static bool Run(string BasePath, string GameAssemblyPath, string OutputPath)
        {
            Environment.SetEnvironmentVariable("IL2CPP_INTEROP_DATABASES_LOCATION", OutputPath);
            Logger = new("Il2Cpp.AssemblyGenerator");

            webClient = new();
            webClient.DefaultRequestHeaders.Add("User-Agent", $"{BuildInfo.Name} v{BuildInfo.Version}");

            //AssemblyGenerationNeeded = LoaderConfig.Current.UnityEngine.ForceRegeneration;

            Config.Initialize(BasePath);

            //if (!LoaderConfig.Current.UnityEngine.ForceOfflineGeneration)
                RemoteAPI.Contact();

            Cpp2IL cpp2IL_netcore = new Cpp2IL(BasePath);
            if (OsUtils.IsWindows
                && (cpp2IL_netcore.VersionSem < Cpp2IL.NetCoreMinVersion))
                cpp2il = new Cpp2IL_NetFramework(BasePath);
            else
                cpp2il = cpp2IL_netcore;
            cpp2il_scrs = new Cpp2IL_StrippedCodeRegSupport(cpp2il, BasePath);

            il2cppinterop = new Packages.Il2CppInterop(BasePath);
            unitydependencies = new UnityDependencies(BasePath);
            deobfuscationMap = new DeobfuscationMap(BasePath);
            deobfuscationRegex = new DeobfuscationRegex();

            Logger.Msg($"Using Cpp2IL Version: {(string.IsNullOrEmpty(cpp2il.Version) ? "null" : cpp2il.Version)}");
            Logger.Msg($"Using Il2CppInterop Version = {(string.IsNullOrEmpty(il2cppinterop.Version) ? "null" : il2cppinterop.Version)}");
            Logger.Msg($"Using Unity Dependencies Version = {(string.IsNullOrEmpty(unitydependencies.Version) ? "null" : unitydependencies.Version)}");
            Logger.Msg($"Using Deobfuscation Regex = {(string.IsNullOrEmpty(deobfuscationRegex.Regex) ? "null" : deobfuscationRegex.Regex)}");

            if (!cpp2il.Setup()
                || !cpp2il_scrs.Setup()
                || !il2cppinterop.Setup()
                || !unitydependencies.Setup()
                || !deobfuscationMap.Setup())
            {
                webClient.Dispose();
                webClient = null;
                return false;
            }

            deobfuscationRegex.Setup();

            string CurrentGameAssemblyHash;
            Logger.Msg("Checking GameAssembly...");
            MelonDebug.Msg($"Last GameAssembly Hash: {Config.Values.GameAssemblyHash}");
            MelonDebug.Msg($"Current GameAssembly Hash: {CurrentGameAssemblyHash = MelonUtils.ComputeSimpleSHA512Hash(GameAssemblyPath)}");

            if (string.IsNullOrEmpty(Config.Values.GameAssemblyHash)
                || !Config.Values.GameAssemblyHash.Equals(CurrentGameAssemblyHash))
                AssemblyGenerationNeeded = true;

            if (!AssemblyGenerationNeeded)
            {
                Logger.Msg("Assembly is up to date. No Generation Needed.");
                webClient.Dispose();
                webClient = null;
                return true;
            }
            Logger.Msg("Assembly Generation Needed!");

            cpp2il.Cleanup();
            il2cppinterop.Cleanup();

            if (!cpp2il.Execute(GameAssemblyPath))
            {
                cpp2il.Cleanup();
                webClient.Dispose();
                webClient = null;
                return false;
            }

            if (!il2cppinterop.Execute(GameAssemblyPath))
            {
                cpp2il.Cleanup();
                il2cppinterop.Cleanup();
                webClient.Dispose();
                webClient = null;
                return false;
            }

            OldFiles_Cleanup(OutputPath);
            OldFiles_LAM(OutputPath);

            cpp2il.Cleanup();
            il2cppinterop.Cleanup();

            webClient.Dispose();
            webClient = null;

            Logger.Msg("Assembly Generation Successful!");
            deobfuscationRegex.Save();
            Config.Values.GameAssemblyHash = CurrentGameAssemblyHash;
            Config.Save();

            return true;
        }

        private static void OldFiles_Cleanup(string OutputPath)
        {
            if (Config.Values.OldFiles.Count <= 0)
                return;
            for (int i = 0; i < Config.Values.OldFiles.Count; i++)
            {
                string filename = Config.Values.OldFiles[i];
                string filepath = Path.Combine(OutputPath, filename);
                if (File.Exists(filepath))
                {
                    Logger.Msg("Deleting " + filename);
                    File.Delete(filepath);
                }
            }
            Config.Values.OldFiles.Clear();
        }

        private static void OldFiles_LAM(string OutputPath)
        {
            string[] filepathtbl = Directory.GetFiles(il2cppinterop.OutputFolder);
            string il2CppAssembliesDirectory = OutputPath;
            for (int i = 0; i < filepathtbl.Length; i++)
            {
                string filepath = filepathtbl[i];
                string filename = Path.GetFileName(filepath);
                Logger.Msg("Moving " + filename);
                Config.Values.OldFiles.Add(filename);
                string newfilepath = Path.Combine(il2CppAssembliesDirectory, filename);
                if (File.Exists(newfilepath))
                    File.Delete(newfilepath);
                Directory.CreateDirectory(il2CppAssembliesDirectory);
                File.Move(filepath, newfilepath);
            }
            Config.Save();
        }
    }
}