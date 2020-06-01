using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace MelonLoader.Installer
{
    public static class PackageManager
    {
        public static void Run(string destinationFolder, bool isIl2Cpp)
        {
            var tempFile = Path.GetTempFileName();
            Program.mainForm.label1.Text = "Downloading...";
            new WebClient().DownloadFile("https://github.com/HerpDerpinstine/MelonLoader/releases/download/v" + "0.2" + "/" + (isIl2Cpp ? "MelonLoader_Il2Cpp.zip": "MelonLoader_Mono.zip"), tempFile);
            Program.mainForm.label1.Text = "Extracting...";
            if (Directory.Exists(Path.Combine(destinationFolder, "MelonLoader")))
                Directory.Delete(Path.Combine(destinationFolder, "MelonLoader"), true);
            if (Directory.Exists(Path.Combine(destinationFolder, "Logs")))
                Directory.Delete(Path.Combine(destinationFolder, "Logs"), true);
            if (File.Exists(Path.Combine(destinationFolder, "version.dll")))
                File.Delete(Path.Combine(destinationFolder, "version.dll"));
            if (File.Exists(Path.Combine(destinationFolder, "winmm.dll")))
                File.Delete(Path.Combine(destinationFolder, "winmm.dll"));
            var fastZip = new FastZip();
            fastZip.ExtractZip(tempFile, destinationFolder, "");
            File.Delete(tempFile);
        }
    }
}