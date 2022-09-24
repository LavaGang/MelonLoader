using System.IO;
using System.Net.Http;

namespace MelonLoader.Il2CppAssemblyGenerator;

internal static class Extensions
{
    public static void DownloadFile(this HttpClient client, string url, string dest)
    {
        using var dlStream = client.GetStreamAsync(url).Result;
        using var fileStream = File.Open(dest, FileMode.Create, FileAccess.Write);
        dlStream.CopyTo(fileStream);
    }  
}