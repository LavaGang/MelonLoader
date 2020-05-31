using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        internal static bool Initialize()
        {
            string GeneratorProcessPath = Path.Combine(Path.Combine(Imports.GetGameDirectory(), "MelonLoader"), "MelonLoader.GeneratorProcess.exe");
            if (File.Exists(GeneratorProcessPath))
            {
                var generatorProcessInfo = new ProcessStartInfo(GeneratorProcessPath);
                generatorProcessInfo.Arguments = $"\"{MelonLoader.Main.UnityVersion}\" \"{Imports.GetGameDirectory()}\" \"{Imports.GetGameDataDirectory()}\"";
                generatorProcessInfo.UseShellExecute = false;
                generatorProcessInfo.RedirectStandardOutput = true;
                generatorProcessInfo.CreateNoWindow = true;
                var process = Process.Start(generatorProcessInfo);
                if (process == null)
                    MelonModLogger.LogError("Unable to Start Generator Process!");
                else
                {
                    var stdout = process.StandardOutput;
                    while (!process.HasExited && !stdout.EndOfStream)
                    {
                        var line = stdout.ReadLine();
                        MelonModLogger.Log(line);
                    }
                    while (!process.HasExited)
                        Thread.Sleep(100);
                    if (Imports.IsDebugMode())
                        MelonModLogger.Log($"Generator Process exited with code {process.ExitCode}");
                    return (process.ExitCode == 0);
                }
            }
            else
                MelonModLogger.LogError("MelonLoader.GeneratorProcess.exe does not Exist!");
            return false;
        }
    }
}
