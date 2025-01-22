using System.Diagnostics;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class DotnetInstaller
{
    private static readonly string dotnetRuntimeDownload =
#if X64
        "https://aka.ms/dotnet/6.0/dotnet-runtime-win-x64.exe";
#else
        "https://aka.ms/dotnet/6.0/dotnet-runtime-win-x86.exe";
#endif

    public static void AttemptInstall()
    {
#if WINDOWS
        AttemptInstallAsync().GetAwaiter().GetResult();
#endif
    }

    private static async Task AttemptInstallAsync()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "MelonLoader");

        Core.Logger.Msg("Downloading the .NET Runtime installer...");

        HttpResponseMessage resp;
        try
        {
            resp = await http.GetAsync(dotnetRuntimeDownload, HttpCompletionOption.ResponseContentRead);
        }
        catch
        {
            Core.Logger.Error("Failed to download the .NET Runtime installer. Check your internet connection.");
            return;
        }

        if (!resp.IsSuccessStatusCode)
        {
            Core.Logger.Error($"Failed to download the .NET Runtime installer. Reason: '{resp.ReasonPhrase}'");
            return;
        }

        Core.Logger.Msg("Installing the .NET Runtime...");

        var installerPath = Path.GetTempFileName() + ".exe";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(installerPath)!);

            using var str = File.Create(installerPath);
            await resp.Content.CopyToAsync(str);
        }
        catch
        {
            Core.Logger.Error($"Failed to copy the installer to path: '{installerPath}'");
            return;
        }

        try
        {
            await Process.Start(installerPath, "/install /passive /norestart").WaitForExitAsync();
        }
        catch
        {
            Core.Logger.Error($"Failed to start the .NET installer");
        }

        File.Delete(installerPath);
    }
}
