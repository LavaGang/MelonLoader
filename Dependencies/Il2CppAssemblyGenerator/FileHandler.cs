using System;
using System.IO;
using System.IO.Compression;
using MelonLoader.Lemons.Cryptography;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal static class FileHandler
    {
        private static LemonSHA512 sha512 = new LemonSHA512();

        internal static string Hash(string filepath)
            => BitConverter.ToString(sha512.ComputeHash(File.ReadAllBytes(filepath))).Replace("-", "").ToLowerInvariant();
            
        internal static bool Download(string url, string destination)
        {
            if (string.IsNullOrEmpty(url))
            {
                MelonLogger.Error($"url cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                MelonLogger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (File.Exists(destination))
                File.Delete(destination);

            MelonLogger.Msg($"Downloading {url} to {destination}");
            try { Core.webClient.DownloadFile(url, destination); }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.ToString());

                if (File.Exists(destination))
                    File.Delete(destination);

                return false;
            }

            return true;
        }

        internal static bool Process(string filepath, string destination)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                MelonLogger.Error($"filepath cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                MelonLogger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (filepath.Equals(destination))
                return true;

            if (!File.Exists(filepath))
            {
                MelonLogger.Error($"{filepath} does not Exist!");
                return false;
            }

            if (Path.HasExtension(destination))
            {
                if (File.Exists(destination))
                    File.Delete(destination);
            }
            else
            {
                if (Directory.Exists(destination))
                {
                    MelonLogger.Msg($"Cleaning {destination}");
                    foreach (var entry in Directory.EnumerateFileSystemEntries(destination))
                    {
                        if (Directory.Exists(entry))
                            Directory.Delete(entry, true);
                        else
                            File.Delete(entry);
                    }
                }
                else
                {
                    MelonLogger.Msg($"Creating Directory {destination}");
                    Directory.CreateDirectory(destination);
                }
            }

            string filename = Path.GetFileName(filepath);
            if (!filename.EndsWith(".zip"))
            {
                MelonLogger.Msg($"Moving {filepath} to {destination}");
                File.Move(filepath, destination);
                return true;
            }

            MelonLogger.Msg($"Extracting {filepath} to {destination}");
            try { ZipFile.ExtractToDirectory(filepath, destination); }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.ToString());

                if (File.Exists(filepath))
                    File.Delete(filepath);

                if (Directory.Exists(destination))
                {
                    foreach (var entry in Directory.EnumerateFileSystemEntries(destination))
                    {
                        if (Directory.Exists(entry))
                            Directory.Delete(entry, true);
                        else
                            File.Delete(entry);
                    }
                }

                return false;
            }

            if (File.Exists(filepath))
                File.Delete(filepath);

            return true;
        }
    }
}
