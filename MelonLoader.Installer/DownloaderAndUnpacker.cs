using System.IO;
using System.IO.Compression;
using System.Net;

namespace MelonLoader.Installer
{
    public static class DownloaderAndUnpacker
    {
        public static void Run(string url, string targetVersion, string destinationFolder)
        {
            var downloadVersionMark = Path.Combine(destinationFolder, ".v-" + targetVersion);
            if (File.Exists(downloadVersionMark))
            {
                //Logger.Log($"{destinationFolder} already contains required version, skipping download");
                return;
            }
            //Logger.Log($"Cleaning {destinationFolder}");
            foreach (var entry in Directory.EnumerateFileSystemEntries(destinationFolder))
            {
                if (Directory.Exists(entry))
                    Directory.Delete(entry, true);
                else
                    File.Delete(entry);
            }
            var tempFile = Path.GetTempFileName();
            //Logger.Log($"Downloading {url} to {tempFile}");
            new WebClient().DownloadFile(url, tempFile);
            //Logger.Log($"Extracting {tempFile} to {destinationFolder}");
            var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
            var zip = new ZipArchive(stream);
            foreach (var zipArchiveEntry in zip.Entries)
            {
                //Logger.Log($"Extracting {zipArchiveEntry.FullName}");
                var entryStream = zipArchiveEntry.Open();
                var targetStream = new FileStream(Path.Combine(destinationFolder, zipArchiveEntry.FullName), FileMode.OpenOrCreate, FileAccess.Write);
                entryStream.CopyTo(targetStream);
            }
            File.WriteAllBytes(downloadVersionMark, new byte[0]);
        }
    }
}