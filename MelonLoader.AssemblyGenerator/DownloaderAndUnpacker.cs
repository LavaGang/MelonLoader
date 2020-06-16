using System.IO;
using System.IO.Compression;
using System.Net;

namespace MelonLoader.AssemblyGenerator
{
    public static class DownloaderAndUnpacker
    {
        public static void Run(string url, string targetVersion, string currentVersion, string destinationFolder)
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

            var tempFile = Path.GetTempFileName();
            Logger.Log($"Downloading {url} to {tempFile}");
            new WebClient().DownloadFile(url, tempFile);
            Logger.Log($"Extracting {tempFile} to {destinationFolder}");
            
            using var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
            using var zip = new ZipArchive(stream);
            
            foreach (var zipArchiveEntry in zip.Entries)
            {
                Logger.Log($"Extracting {zipArchiveEntry.FullName}");
                using var entryStream = zipArchiveEntry.Open();
                using var targetStream = new FileStream(Path.Combine(destinationFolder, zipArchiveEntry.FullName), FileMode.OpenOrCreate, FileAccess.Write);
                entryStream.CopyTo(targetStream);
            }
        }
    }
}