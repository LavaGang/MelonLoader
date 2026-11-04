using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal static class DotnetPortable
{
    private static string GetDownloadURL()
    {
#if WINDOWS && X86
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x86.zip";
        
#elif WINDOWS && X64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x64.zip";
#elif LINUX && X64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-x64.tar.gz";
#elif OSX && X64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-osx-x64.tar.gz"; 
        
#elif WINDOWS && ARM64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-arm64.zip";
#elif LINUX && ARM64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-arm64.tar.gz";
#elif OSX && ARM64
        return "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-osx-arm64.tar.gz"; 
        
#else
        return null;
#endif
    }
    
    public static bool AttemptInstall()
    {
        string dotnetRuntimeDownload = GetDownloadURL();
        if (string.IsNullOrEmpty(dotnetRuntimeDownload))
        {
            Core.Logger.Error("No Portable .NET Runtime package exists for this Platform!");
            return false;
        }

        FileDownload downloadRequest = new(dotnetRuntimeDownload);
        
        Core.Logger.Msg($"Downloading the Portable .NET Runtime from: {dotnetRuntimeDownload}");
        var tempPath = Path.GetTempFileName() + ".tmp";
        (bool, HttpResponseMessage?)? resp = null;
        try
        {
            resp = downloadRequest.Attempt(tempPath);
            if (!resp.Value.Item1)
            {
                Core.Logger.Error("Failed to download the Portable .NET Runtime. Check your internet connection.");
                
                if (resp.Value.Item2 != null)
                    Core.Logger.Error(resp.Value.Item2.ReasonPhrase!);
                
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                return false;
            }
        }
        catch (Exception ex)
        {
            Core.Logger.Error("Failed to download the Portable .NET Runtime. Check your internet connection.");
            
            if (resp.HasValue
                && (resp.Value.Item2 != null))
                Core.Logger.Error(resp.Value.Item2.ReasonPhrase!);
            
            Core.Logger.Error(ex.ToString());
            
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return false;
        }

        Core.Logger.Msg("Extracting the Portable .NET Runtime...");
        string dependenciesDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Dependencies");
        string dotnetDir = Path.Combine(dependenciesDir, "dotnet");
        if (!Directory.Exists(dependenciesDir))
            Directory.CreateDirectory(dependenciesDir);
        try
        {
            if (Directory.Exists(dotnetDir))
                Directory.Delete(dotnetDir, true);
            Directory.CreateDirectory(dotnetDir);
            
#if WINDOWS
            ZipFile.ExtractToDirectory(tempPath, dotnetDir);
#else
            var fileStream = File.OpenRead(tempPath);
            var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            TarFile.ExtractToDirectory(
                gzipStream,
                dotnetDir,
                true);
            gzipStream.Close();
            fileStream.Close();
#endif
        }
        catch (Exception ex)
        {
            Core.Logger.Error($"Failed to extract the Portable .NET Runtime");
            Core.Logger.Error(ex.ToString());
            if (Directory.Exists(dotnetDir))
                Directory.Delete(dotnetDir, true);
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return false;
        }
        if (File.Exists(tempPath))
            File.Delete(tempPath);
        return true;
    }
}
