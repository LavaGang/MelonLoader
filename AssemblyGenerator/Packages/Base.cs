using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MelonLoader.AssemblyGenerator
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
            Logger.Msg($"Downloading {URL} to {tempfile}");
            try { Core.webClient.DownloadFile(URL, tempfile); }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                if (File.Exists(tempfile))
                    File.Delete(tempfile);
                return false;
            }

            if (!directory_check)
            {
                if (Directory.Exists(Destination))
                {
                    Logger.Msg($"Cleaning {Destination}");
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
                    Logger.Msg($"Creating Directory {Destination}");
                    Directory.CreateDirectory(Destination);
                }
            }

            string filenamefromurl = Path.GetFileName(URL);
            if (!filenamefromurl.EndsWith(".zip"))
            {
                string filepath = Path.Combine(Destination, (string.IsNullOrEmpty(NewFileName) ? filenamefromurl : NewFileName));
                Logger.Msg($"Moving {tempfile} to {filepath}");
                File.Move(tempfile, filepath);
                return true;
            }

            Logger.Msg($"Extracting {tempfile} to {Destination}");
            try
            {
                using var stream = new FileStream(tempfile, FileMode.Open, FileAccess.Read);
                using var zip = new ZipArchive(stream);
                foreach (var zipArchiveEntry in zip.Entries)
                {
                    Logger.Msg($"Extracting {zipArchiveEntry.FullName}");
                    using var entryStream = zipArchiveEntry.Open();
                    using var targetStream = new FileStream(Path.Combine(Destination, zipArchiveEntry.FullName), FileMode.OpenOrCreate, FileAccess.Write);
                    entryStream.CopyTo(targetStream);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
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
                Logger.Error(Path.GetFileName(ExePath) + " Doesn't Exist!");
                return false;
            }
            try
            {
                ResetEvent_Output = new AutoResetEvent(false);
                ResetEvent_Error = new AutoResetEvent(false);
                Core.OverrideAppDomainBase(Destination);

                ProcessStartInfo processStartInfo = new ProcessStartInfo(ExePath);
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;

                processStartInfo.Arguments =
                    parenthesize_args
                    ?
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => ("\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\"")))
                    :
                    string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => Regex.Replace(it, @"(\\+)$", @"$1$1")));

                Logger.Msg("\"" + ExePath + "\" " + processStartInfo.Arguments);

                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.OutputDataReceived += OutputStream;
                process.ErrorDataReceived += ErrorStream;
                process.Start();

                Utils.SetProcessId(process.Id);

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                ResetEvent_Output.WaitOne();
                ResetEvent_Error.WaitOne();

                Utils.SetProcessId(0);
                Core.OverrideAppDomainBase(Core.BasePath);
                return (process.ExitCode == 0);
            }
            catch (Exception ex) { Logger.Error(ex.ToString()); Core.OverrideAppDomainBase(Core.BasePath); }
            return false;
        }

        private static void OutputStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Output.Set(); else Logger.Msg(e.Data); }
        private static void ErrorStream(object sender, DataReceivedEventArgs e) { if (e.Data == null) ResetEvent_Error.Set(); else Logger.Error(e.Data); }
    }
}