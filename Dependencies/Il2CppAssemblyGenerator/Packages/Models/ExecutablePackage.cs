using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages.Models
{
    internal class ExecutablePackage : PackageBase
    {
        internal static AutoResetEvent ResetEvent_Output;
        internal static AutoResetEvent ResetEvent_Error;
        internal string OutputFolder;
        internal string ExeFilePath;

        internal virtual void Cleanup()
        {
            if (!Directory.Exists(OutputFolder))
                return;
            Directory.Delete(OutputFolder, true);
        }

        internal virtual bool Execute() => true;
        internal bool Execute(string[] args, bool parenthesize_args = true, Dictionary<string, string> environment = null)
        {
            Cleanup();

            if (!File.Exists(ExeFilePath))
            {
                Core.Logger.Error($"{ExeFilePath} does not Exist!");
                ThrowInternalFailure($"Failed to Execute {Name}!");
                return false;
            }

            Core.Logger.Msg($"Executing {Name}...");
            try
            {
#if LINUX
                // Make the file executable on Unix
                chmod(ExeFilePath, S_IRUSR | S_IXUSR | S_IWUSR | S_IRGRP | S_IXGRP | S_IROTH | S_IXOTH);
#endif
                
                string tempFolder = Path.Combine(Path.GetDirectoryName(ExeFilePath),
                    $"{Path.GetFileNameWithoutExtension(ExeFilePath)}_Temp");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                ResetEvent_Output = new AutoResetEvent(false);
                ResetEvent_Error = new AutoResetEvent(false);

                ProcessStartInfo processStartInfo = new ProcessStartInfo(ExeFilePath, parenthesize_args
                    ?
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => "\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\""))
                    :
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => Regex.Replace(it, @"(\\+)$", @"$1$1"))));
                
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(ExeFilePath)!;

                if (environment != null)
                {
                    processStartInfo.EnvironmentVariables["DOTNET_BUNDLE_EXTRACT_BASE_DIR"] = tempFolder;
                    foreach (var kvp in environment)
                        processStartInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
                }

                Core.Logger.Msg("\"" + ExeFilePath + "\" " + processStartInfo.Arguments);

                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.OutputDataReceived += OutputStream;
                process.ErrorDataReceived += ErrorStream;
                process.Start();

                SetProcessId(process.Id);

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                ResetEvent_Output.WaitOne();
                ResetEvent_Error.WaitOne();

                SetProcessId(0);

                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Core.Logger.Error(ex.ToString());
                ThrowInternalFailure($"Failed to Execute {Name}!");
            }

            return false;
        }

        private static void OutputStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Output.Set(); else Core.Logger.Msg(e.Data); }
        private static void ErrorStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Error.Set(); else Core.Logger.Error(e.Data); }
        
        private static void SetProcessId(int id)
        {
            //MelonLogger.Warning($"TODO: SetProcessId({id})");
        }
        
#if LINUX// user permissions
        const int S_IRUSR = 0x100;
        const int S_IWUSR = 0x80;
        const int S_IXUSR = 0x40;

        // group permission
        const int S_IRGRP = 0x20;
        const int S_IWGRP = 0x10;
        const int S_IXGRP = 0x8;

        // other permissions
        const int S_IROTH = 0x4;
        const int S_IWOTH = 0x2;
        const int S_IXOTH = 0x1;
        
        [System.Runtime.InteropServices.DllImport("libc")]
        private static extern int chmod(string pathname, int mode);
#endif
    }
}
