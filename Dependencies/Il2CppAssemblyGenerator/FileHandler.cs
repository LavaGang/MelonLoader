using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
                Core.Logger.Error($"url cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                Core.Logger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (File.Exists(destination))
                File.Delete(destination);

            Core.Logger.Msg($"Downloading {url} to {destination}");
            try { Core.webClient.DownloadFile(url, destination); }
            catch (Exception ex)
            {
                Core.Logger.Error(ex.ToString());

                if (File.Exists(destination))
                    File.Delete(destination);

                return false;
            }

            return true;
        }

        internal static bool Process(string filepath, string destination, string targetName = null)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                Core.Logger.Error($"filepath cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                Core.Logger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (filepath.Equals(destination))
                return true;

            if (!File.Exists(filepath))
            {
                Core.Logger.Error($"{filepath} does not Exist!");
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
                    Core.Logger.Msg($"Cleaning {destination}");
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
                    Core.Logger.Msg($"Creating Directory {destination}");
                    Directory.CreateDirectory(destination);
                }
            }

            string filename = Path.GetFileName(filepath);
            if (!filename.EndsWith(".zip"))
            {
                Core.Logger.Msg($"Moving {filepath} to {destination}");
                
                if (!string.IsNullOrEmpty(targetName))
                    destination = Path.Combine(destination, targetName);
                
                File.Move(filepath, destination);
                return true;
            }

            Core.Logger.Msg($"Extracting {filepath} to {destination}");
            try { ZipFile.ExtractToDirectory(filepath, destination); }
            catch (Exception ex)
            {
                Core.Logger.Error(ex.ToString());

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
