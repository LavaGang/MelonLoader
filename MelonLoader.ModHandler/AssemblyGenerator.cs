using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        internal static bool Initialize()
        {
            var generatorProcessInfo = new ProcessStartInfo(Path.Combine(Path.Combine(Imports.GetGameDirectory(), "MelonLoader"), "MelonLoader.GeneratorProcess.exe"));
            generatorProcessInfo.Arguments = $"\"{MelonLoader.Main.UnityVersion}\" \"{Imports.GetGameDirectory()}\" \"{Imports.GetGameDataDirectory()}\"";
            generatorProcessInfo.UseShellExecute = false;
            generatorProcessInfo.RedirectStandardOutput = true;
            generatorProcessInfo.CreateNoWindow = true;
            var process = Process.Start(generatorProcessInfo);
            if (process == null)
            {
                MelonModLogger.LogError("Unable to start generator process");
                return false;
            }
            var stdout = process.StandardOutput;
            while (!process.HasExited && !stdout.EndOfStream)
            {
                var line = stdout.ReadLine();
                MelonModLogger.Log(line);
            }

            while (!process.HasExited)
                Thread.Sleep(100);
            
            MelonModLogger.Log($"Generator process exited with code {process.ExitCode}");
            return process.ExitCode == 0;
        }
    }
}
