using System;
using System.IO;
using System.IO.Compression;

namespace MelonLoader.Engine.Unity
{
    internal static class FileHandler
    {
        internal static bool Download(string url, string destination)
        {
            if (string.IsNullOrEmpty(url))
            {
                AssemblyGenerator.Logger.Error($"url cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                AssemblyGenerator.Logger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (File.Exists(destination))
                File.Delete(destination);

            AssemblyGenerator.Logger.Msg($"Downloading {url} to {destination}");
            try { AssemblyGenerator.webClient.DownloadFile(url, destination); }
            catch (Exception ex)
            {
                AssemblyGenerator.Logger.Error(ex.ToString());

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
                AssemblyGenerator.Logger.Error($"filepath cannot be Null or Empty!");
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                AssemblyGenerator.Logger.Error($"destination cannot be Null or Empty!");
                return false;
            }

            if (filepath.Equals(destination))
                return true;

            if (!File.Exists(filepath))
            {
                AssemblyGenerator.Logger.Error($"{filepath} does not Exist!");
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
                    AssemblyGenerator.Logger.Msg($"Cleaning {destination}");
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
                    AssemblyGenerator.Logger.Msg($"Creating Directory {destination}");
                    Directory.CreateDirectory(destination);
                }
            }

            string filename = Path.GetFileName(filepath);
            if (!filename.EndsWith(".zip"))
            {
                AssemblyGenerator.Logger.Msg($"Moving {filepath} to {destination}");
                
                if (!string.IsNullOrEmpty(targetName))
                    destination = Path.Combine(destination, targetName);
                
                File.Move(filepath, destination);
                return true;
            }

            AssemblyGenerator.Logger.Msg($"Extracting {filepath} to {destination}");
            try { ZipFile.ExtractToDirectory(filepath, destination); }
            catch (Exception ex)
            {
                AssemblyGenerator.Logger.Error(ex.ToString());

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
