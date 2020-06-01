using System.IO;
using System.IO.Compression;
using System.Net;

namespace MelonLoader.Installer
{
    public static class PackageManager
    {

        public static void Run(string destinationFolder, bool isIl2Cpp)
        {
            Program.mainForm.label1.Text = "Downloading...";
            var tempFile = Path.GetTempFileName();
            new WebClient().DownloadFile(("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v" + "0.2" + "/" + (isIl2Cpp ? "MelonLoader_Il2Cpp.zip" : "MelonLoader_Mono.zip")), tempFile);
            
            Program.mainForm.label1.Text = "Extracting...";
            if (Directory.Exists(Path.Combine(destinationFolder, "MelonLoader")))
                Directory.Delete(Path.Combine(destinationFolder, "MelonLoader"), true);
            if (Directory.Exists(Path.Combine(destinationFolder, "Logs")))
                Directory.Delete(Path.Combine(destinationFolder, "Logs"), true);
            if (File.Exists(Path.Combine(destinationFolder, "version.dll")))
                File.Delete(Path.Combine(destinationFolder, "version.dll"));
            if (File.Exists(Path.Combine(destinationFolder, "winmm.dll")))
                File.Delete(Path.Combine(destinationFolder, "winmm.dll"));

            /*
            using var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
            using var zip = new ZipArchive(stream);
            foreach (var zipArchiveEntry in zip.Entries)
            {
                using var entryStream = zipArchiveEntry.Open();
                using var targetStream = new FileStream(Path.Combine(destinationFolder, zipArchiveEntry.FullName), FileMode.OpenOrCreate, FileAccess.Write);
                entryStream.CopyTo(targetStream);
            }
            */
        }
    }
}