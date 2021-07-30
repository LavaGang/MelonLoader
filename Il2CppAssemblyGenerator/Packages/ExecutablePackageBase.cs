using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using MelonLoader.Utils;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class ExecutablePackageBase : PackageBase
    {
        internal static AutoResetEvent ResetEvent_Output;
        internal static AutoResetEvent ResetEvent_Error;
        internal string ExePath = null;
        internal string Output = null;

        internal virtual void Cleanup()
        {
            if (!Directory.Exists(Output))
                return;
            Directory.Delete(Output, true);
        }

        internal bool Execute(string[] args, bool parenthesize_args = true, Dictionary<string, string> environment = null)
        {
            if (!Directory.Exists(Output))
                Directory.CreateDirectory(Output);
            if (!File.Exists(ExePath))
            {
                MelonLogger.Error(Path.GetFileName(ExePath) + " Doesn't Exist!");
                return false;
            }
            try
            {
                ResetEvent_Output = new AutoResetEvent(false);
                ResetEvent_Error = new AutoResetEvent(false);

                ProcessStartInfo processStartInfo = new ProcessStartInfo(ExePath,
                    parenthesize_args
                    ?
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => "\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\""))
                    :
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => Regex.Replace(it, @"(\\+)$", @"$1$1"))));
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(ExePath);

                if (environment != null)
                {
                    foreach (var kvp in environment)
                    {
                        processStartInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
                    }
                }

                MelonLogger.Msg("\"" + ExePath + "\" " + processStartInfo.Arguments);

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
            catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            return false;
        }

        private static void OutputStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Output.Set(); else MelonLogger.Msg(e.Data); }
        private static void ErrorStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Error.Set(); else MelonLogger.Error(e.Data); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetProcessId(int id);
    }
}