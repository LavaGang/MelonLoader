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
                ResetEvent_Output = new AutoResetEvent(false);
                ResetEvent_Error = new AutoResetEvent(false);

                ProcessStartInfo processStartInfo = new ProcessStartInfo($"\"{ExeFilePath.Replace("\"", "\\\"")}\"", // Replacing double quotes for Linux
                    parenthesize_args
                    ?
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => "\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\""))
                    :
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => Regex.Replace(it, @"(\\+)$", @"$1$1"))));
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(ExeFilePath);

                if (environment != null)
                {
                    foreach (var kvp in environment)
                    {
                        processStartInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
                    }
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void SetProcessId(int id);
    }
}
