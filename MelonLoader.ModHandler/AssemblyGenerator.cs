using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        internal static bool Initialize()
        {
            string GeneratorProcessPath = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Imports.GetGameDirectory(), "MelonLoader"), "Dependencies"), "AssemblyGenerator"), "MelonLoader.AssemblyGenerator.exe");
            if (File.Exists(GeneratorProcessPath))
            {
                var generatorProcessInfo = new ProcessStartInfo(GeneratorProcessPath);
                generatorProcessInfo.Arguments = $"\"{MelonLoader.Main.UnityVersion}\" \"{Imports.GetGameDirectory()}\" \"{Imports.GetGameDataDirectory()}\" {(Imports.AG_Force_Regenerate() ? "true" : "")}";
                generatorProcessInfo.UseShellExecute = false;
                generatorProcessInfo.RedirectStandardOutput = true;
                generatorProcessInfo.CreateNoWindow = true;
                var process = Process.Start(generatorProcessInfo);
                if (process == null)
                    MelonModLogger.LogError("Unable to Start Assembly Generator!");
                else
                {
                    var stdout = process.StandardOutput;
                    while (!stdout.EndOfStream)
                    {
                        var line = stdout.ReadLine();
                        MelonModLogger.Log(line);
                    }
                    while (!process.HasExited)
                        Thread.Sleep(100);
                    if (Imports.IsDebugMode())
                        MelonModLogger.Log($"Assembly Generator exited with code {process.ExitCode}");
                    return (process.ExitCode == 0);
                }
            }
            else
                MelonModLogger.LogError("MelonLoader.AssemblyGenerator.exe does not Exist!");
            return false;
        }
    }
}
