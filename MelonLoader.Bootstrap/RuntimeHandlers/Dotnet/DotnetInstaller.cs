#if WINDOWS
using System.Diagnostics;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal static class DotnetInstaller
{
    private const string dotnetRuntimeDownload =
#if X64
        "https://aka.ms/dotnet/10.0/dotnet-runtime-win-x64.exe";
#else
        "https://aka.ms/dotnet/10.0/dotnet-runtime-win-x86.exe";
#endif
    private static readonly FileDownload downloadRequest = new(dotnetRuntimeDownload);

    public static bool AttemptInstall()
    {
        return AttemptInstallAsync().GetAwaiter().GetResult();
    }

    private static async Task<bool> AttemptInstallAsync()
    {
        Core.Logger.Msg($"Downloading the .NET Runtime Installer from: {dotnetRuntimeDownload}");
        
        var tempPath = Path.GetTempFileName() + ".exe";
        (bool, HttpResponseMessage?)? resp = null;
        try
        {
            resp = downloadRequest.Attempt(tempPath);
            if (!resp.Value.Item1)
            {
                Core.Logger.Error("Failed to download the .NET Runtime Installer. Check your internet connection.");
                
                if (resp.Value.Item2 != null)
                    Core.Logger.Error(resp.Value.Item2.ReasonPhrase!);
                
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                return false;
            }
        }
        catch (Exception ex)
        {
            Core.Logger.Error("Failed to download the .NET Runtime Installer. Check your internet connection.");
            
            if (resp.HasValue
                && (resp.Value.Item2 != null))
                Core.Logger.Error(resp.Value.Item2.ReasonPhrase!);
            
            Core.Logger.Error(ex.ToString());
            
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return false;
        }

        Core.Logger.Msg("Running the .NET Runtime Installer...");
        try
        {
            await Process.Start(tempPath, "/install /passive /norestart").WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Core.Logger.Error($"Failed to run the .NET Runtime Installer");
            Core.Logger.Error(ex.ToString());
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