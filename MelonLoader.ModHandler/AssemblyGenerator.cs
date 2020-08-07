using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MelonLoader
{
    internal static class AssemblyGenerator
    {
        internal static bool HasGeneratedAssembly = false;

        internal static void Check()
        {
            if (!Imports.IsIl2CppGame() || Initialize())
                HasGeneratedAssembly = true;
            else
                MelonLoaderBase.UNLOAD(false);
        }

        private static bool Initialize()
        {
            string GeneratorProcessPath = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Imports.GetGameDirectory(), "MelonLoader"), "Dependencies"), "AssemblyGenerator"), "MelonLoader.AssemblyGenerator.exe");
            if (File.Exists(GeneratorProcessPath))
            {
                var generatorProcessInfo = new ProcessStartInfo(GeneratorProcessPath);
                generatorProcessInfo.Arguments = $"\"{MelonLoaderBase.UnityVersion}\" {"\"" + Regex.Replace(Imports.GetGameDirectory(), @"(\\+)$", @"$1$1") + "\""} {"\"" + Regex.Replace(Imports.GetGameDataDirectory(), @"(\\+)$", @"$1$1") + "\""} {(Force_Regenerate() ? "true" : "false")} {(string.IsNullOrEmpty(Force_Version_Unhollower()) ? "" : Force_Version_Unhollower())}";
                generatorProcessInfo.UseShellExecute = false;
                generatorProcessInfo.RedirectStandardOutput = true;
                generatorProcessInfo.CreateNoWindow = true;
                var process = Process.Start(generatorProcessInfo);
                if (process == null)
                    MelonLogger.LogError("Unable to Start Assembly Generator!");
                else
                {
                    var stdout = process.StandardOutput;
                    while (!stdout.EndOfStream)
                    {
                        var line = stdout.ReadLine();
                        MelonLogger.Log(line);
                    }
                    while (!process.HasExited)
                        Thread.Sleep(100);
                    if (process.ExitCode == 0)
                    {
                        if (Imports.IsDebugMode())
                            MelonLogger.Log($"Assembly Generator ran Successfully!");
                        return true;
                    }
                    MelonLogger.Native_ThrowInternalError($"Assembly Generator exited with code {process.ExitCode}");
                }
            }
            else
                MelonLogger.LogError("MelonLoader.AssemblyGenerator.exe does not Exist!");
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static bool Force_Regenerate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Force_Version_Unhollower();
    }
}
