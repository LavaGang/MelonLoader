using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Core
    {
        internal static string GameName = null;
        internal static string BasePath = null;
        internal static string GameAssemblyPath = null;
        internal static string ManagedPath = null;

        internal static WebClient webClient = null;

        internal static Il2CppDumper dumper = null;
        internal static UnityDependencies unitydependencies = null;
        internal static DeobfuscationMap deobfuscationMap = null;
        internal static Il2CppAssemblyUnhollower il2cppassemblyunhollower = null;

        internal static bool AssemblyGenerationNeeded = false;

        static Core()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | (SecurityProtocolType)3072;

            webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Unity web player");

            //AssemblyGenerationNeeded = Utils.ForceRegeneration();

            GameAssemblyPath = Utils.GetGameAssemblyPath();
            ManagedPath = string.Copy(MelonUtils.GetManagedDirectory());
            GameName = MelonUtils.GameName;

            BasePath = Path.GetDirectoryName(Utils.GetAssemblyGeneratorPath());
            OverrideAppDomainBase(BasePath);
        }

        private static int Run()
        {
            RemoteAPI.Contact();

            unitydependencies = new UnityDependencies();
            if (!unitydependencies.Download())
                return 1;

            dumper = new Il2CppDumper();
            if (!dumper.Download())
                return 1;

            il2cppassemblyunhollower = new Il2CppAssemblyUnhollower();
            if (!il2cppassemblyunhollower.Download())
                return 1;

            deobfuscationMap = new DeobfuscationMap();
            if (!deobfuscationMap.Download())
                return 1;

            // Check for Regex Change against Config

            string CurrentGameAssemblyHash;
            MelonLogger.Msg("Checking GameAssembly...");
            MelonDebug.Msg($"Last GameAssembly Hash: {Config.GameAssemblyHash}");
            MelonDebug.Msg($"Current GameAssembly Hash: {CurrentGameAssemblyHash = GetGameAssemblyHash()}");

            if (!AssemblyGenerationNeeded
                && (string.IsNullOrEmpty(Config.GameAssemblyHash)
                    || !Config.GameAssemblyHash.Equals(CurrentGameAssemblyHash)))
                AssemblyGenerationNeeded = true;

            if (!AssemblyGenerationNeeded)
            {
                MelonLogger.Msg("Assembly is up to date. No Generation Needed.");
                return 0;
            }
            MelonLogger.Msg("Assembly Generation Needed!");

            dumper.Cleanup();
            il2cppassemblyunhollower.Cleanup();

            if (!dumper.Execute())
            {
                dumper.Cleanup();
                return 1;
            }

            if (!il2cppassemblyunhollower.Execute())
            {
                dumper.Cleanup();
                il2cppassemblyunhollower.Cleanup();
                return 1;
            }

            OldFiles_Cleanup();
            OldFiles_LAM();

            dumper.Cleanup();
            il2cppassemblyunhollower.Cleanup();

            Config.GameAssemblyHash = CurrentGameAssemblyHash;
            deobfuscationMap.Save();

            MelonLogger.Msg("Assembly Generation Successful!");
            return 0;
        }

        internal static void OverrideAppDomainBase(string basepath)
        {
            MelonUtils.SetCurrentDomainBaseDirectory(basepath);

            /*
            var property = typeof(AppDomain).GetProperty("FusionStore", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
            {
                var appDomainBase = ((AppDomainSetup)property.GetValue(AppDomain.CurrentDomain, new object[0]));
                appDomainBase.ApplicationBase = basepath;
            }
            Directory.SetCurrentDirectory(basepath);
            */
        }

        private static string GetGameAssemblyHash()
        {
            string returnval = null;
            using (MD5 md5 = MD5.Create())
            using (var stream = File.OpenRead(GameAssemblyPath))
            {
                var hash = md5.ComputeHash(stream);
                returnval = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            return returnval;
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
                    MelonLogger.Msg("Deleting " + filename);
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
                MelonLogger.Msg("Moving " + filename);
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