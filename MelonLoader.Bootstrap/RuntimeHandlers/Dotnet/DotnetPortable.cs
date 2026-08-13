#if X64 && (WINDOWS || OSX || LINUX)
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal static class DotnetPortable
{
    private const string dotnetRuntimeDownload =
#if LINUX
#if X64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-x64.tar.gz";
#elif ARM64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-arm64.tar.gz";
#elif ARM32
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-arm.tar.gz";
#endif
#elif OSX
#if X64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-osx-x64.tar.gz"; 
#elif ARM64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-osx-arm64.tar.gz"; 
#endif
#elif WINDOWS
#if X64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x64.zip";
#elif X86
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x86.zip";
#elif ARM64
        "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-arm64.zip";
#endif
#endif
    
    private static readonly FileDownload downloadRequest = new(dotnetRuntimeDownload);

    public static bool AttemptInstall()
    {
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
#if WINDOWS
            ZipFile.ExtractToDirectory(tempPath, dependenciesDir);
#else
            TarFile.ExtractToDirectory(tempPath, dependenciesDir, false);
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
#endif
