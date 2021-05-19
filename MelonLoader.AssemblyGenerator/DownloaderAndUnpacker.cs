using System.IO;
using System.IO.Compression;
using System.Net;

namespace MelonLoader.AssemblyGenerator
{
    public static class DownloaderAndUnpacker
    {
        public static void Run(string url, string targetVersion, string currentVersion, string destinationFolder, string tempFile)
        {
            if (targetVersion == currentVersion)
            {
                Logger.Log($"{destinationFolder} already contains required version, skipping download");
                return;
            }
            
            Logger.Log($"Cleaning {destinationFolder}");
            foreach (var entry in Directory.EnumerateFileSystemEntries(destinationFolder))
            {
                if (Directory.Exists(entry))
                    Directory.Delete(entry, true);
                else
                    File.Delete(entry);
            }

            Logger.Log($"Downloading {url} to {tempFile}");
            Program.webClient.DownloadFile(url, tempFile);
            Logger.Log($"Extracting {tempFile} to {destinationFolder}");

            ZipFile.ExtractToDirectory(tempFile, destinationFolder);
        }
    }
}