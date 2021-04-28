using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class PackageBase
    {
        internal string Version = null;
        internal string URL = null;
        internal string Destination = null;
        internal string NewFileName = null;

        internal virtual bool Download() => Download(false);
        internal virtual bool Download(bool directory_check)
        {
            Core.AssemblyGenerationNeeded = true;
            string tempfile = Path.GetTempFileName();
            MelonLogger.Msg($"Downloading {URL} to {tempfile}");
            try { Core.webClient.DownloadFile(URL, tempfile); }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.ToString());
                if (File.Exists(tempfile))
                    File.Delete(tempfile);
                return false;
            }

            if (!directory_check)
            {
                if (Directory.Exists(Destination))
                {
                    MelonLogger.Msg($"Cleaning {Destination}");
                    foreach (var entry in Directory.EnumerateFileSystemEntries(Destination))
                    {
                        if (Directory.Exists(entry))
                            Directory.Delete(entry, true);
                        else
                            File.Delete(entry);
                    }
                }
                else
                {
                    MelonLogger.Msg($"Creating Directory {Destination}");
                    Directory.CreateDirectory(Destination);
                }
            }

            string filenamefromurl = Path.GetFileName(URL);
            if (!filenamefromurl.EndsWith(".zip"))
            {
                string filepath = Path.Combine(Destination, (string.IsNullOrEmpty(NewFileName) ? filenamefromurl : NewFileName));
                MelonLogger.Msg($"Moving {tempfile} to {filepath}");
                if (File.Exists(filepath)) File.Delete(filepath);
                File.Move(tempfile, filepath);
                return true;
            }

            MelonLogger.Msg($"Extracting {tempfile} to {Destination}");
            try
            {
                using var stream = new FileStream(tempfile, FileMode.Open, FileAccess.Read);
                using var zip = new ZipArchive(stream);
                foreach (var zipArchiveEntry in zip.Entries)
                {
                    MelonLogger.Msg($"Extracting {zipArchiveEntry.FullName}");
                    using var entryStream = zipArchiveEntry.Open();
                    using var targetStream = new FileStream(Path.Combine(Destination, zipArchiveEntry.FullName), FileMode.OpenOrCreate, FileAccess.Write);
                    entryStream.CopyTo(targetStream);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.ToString());
                if (File.Exists(tempfile))
                    File.Delete(tempfile);
                return false;
            }
            return true;
        }
    }

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

        internal bool Execute(string[] args, bool parenthesize_args = true)
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
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => ("\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\"")))
                    :
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => Regex.Replace(it, @"(\\+)$", @"$1$1"))));
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(ExePath);

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
                return (process.ExitCode == 0);
            }
            catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            return false;
        }

        private static void OutputStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Output.Set(); else MelonLogger.Msg(e.Data); }
        private static void ErrorStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Error.Set(); else MelonLogger.Error(e.Data); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void SetProcessId(int id);
    }
}