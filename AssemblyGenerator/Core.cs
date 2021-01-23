using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Core
    {
        internal static string BasePath = null;
        internal static string GameAssemblyPath = null;
        internal static string ManagedPath = null;
        private static string CurrentGameAssemblyHash = null;
        internal static WebClient webClient = null;
        internal static Cpp2IL cpp2il = null;
        internal static UnityDependencies unitydependencies = null;
        private static Il2CppAssemblyUnhollower il2cppassemblyunhollower = null;

        static Core()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | (SecurityProtocolType)3072;
            webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Unity web player");
            BasePath = Path.GetDirectoryName(Utils.GetAssemblyGeneratorPath());
            OverrideAppDomainBase(BasePath);
            cpp2il = new Cpp2IL();
            unitydependencies = new UnityDependencies();
            il2cppassemblyunhollower = new Il2CppAssemblyUnhollower();
            GameAssemblyPath = Utils.GetGameAssemblyPath();
            ManagedPath = Utils.GetManagedDirectory();
            using (MD5 md5 = MD5.Create())
                using (var stream = File.OpenRead(GameAssemblyPath))
                {
                    var hash = md5.ComputeHash(stream);
                    CurrentGameAssemblyHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
        }

        private static int Run(string nullarg)
        {
            Logger.Msg("Checking GameAssembly...");
            Logger.Msg("Old: " + Config.GameAssemblyHash);
            Logger.Msg("Current: " + CurrentGameAssemblyHash);
            if (!string.IsNullOrEmpty(Config.GameAssemblyHash)
                && Config.GameAssemblyHash.Equals(CurrentGameAssemblyHash))
            {
                Logger.Msg("Assembly is up to date. No Generation Needed.");
                return 0;
            }
            Logger.Msg("Assembly Generation Needed!");
            if (!unitydependencies.Download()
                || !cpp2il.Download()
                || !il2cppassemblyunhollower.Download())
                return 1;
            cpp2il.Cleanup();
            il2cppassemblyunhollower.Cleanup();
            if (!cpp2il.Execute())
            {
                cpp2il.Cleanup();
                return 1;
            }
            if (!il2cppassemblyunhollower.Execute())
            {
                cpp2il.Cleanup();
                il2cppassemblyunhollower.Cleanup();
                return 1;
            }
            OldFiles_Cleanup();
            OldFiles_LAM();
            cpp2il.Cleanup();
            il2cppassemblyunhollower.Cleanup();
            Config.GameAssemblyHash = CurrentGameAssemblyHash;
            Config.Save();
            Logger.Msg("Assembly Generation Successful!");
            return 0;
        }

        internal static void OverrideAppDomainBase(string basepath)
        {
            var appDomainBase = ((AppDomainSetup)typeof(AppDomain).GetProperty("FusionStore", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0]));
            appDomainBase.ApplicationBase = basepath;
            Directory.SetCurrentDirectory(basepath);
        }

        private static void OldFiles_Cleanup()
        {
            if (Config.OldFiles.Count <= 0)
                return;
            for (int i = 0; i < Config.OldFiles.Count; i++)
            {
                string filename = Config.OldFiles[i];
                string filepath = Path.Combine(ManagedPath, filename);
                if (File.Exists(filepath))
                {
                    Logger.Msg("Deleting " + filename);
                    File.Delete(filepath);
                }
            }
            Config.OldFiles.Clear();
        }

        private static void OldFiles_LAM()
        {
            string[] filepathtbl = Directory.GetFiles(il2cppassemblyunhollower.Output);
            for (int i = 0; i < filepathtbl.Length; i++)
            {
                string filepath = filepathtbl[i];
                string filename = Path.GetFileName(filepath);
                Logger.Msg("Moving " + filename);
                Config.OldFiles.Add(filename);
                string newfilepath = Path.Combine(ManagedPath, filename);
                if (File.Exists(newfilepath))
                    File.Delete(newfilepath);
                File.Move(filepath, newfilepath);
            }
            Config.Save();
        }
    }
}