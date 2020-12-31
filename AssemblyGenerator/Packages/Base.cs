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

        internal virtual bool Download()
        {
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

            string filenamefromurl = Path.GetFileName(URL);
            if (!filenamefromurl.EndsWith(".zip"))
            {
                string filepath = Path.Combine(Destination, filenamefromurl);
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
        internal string ExePath = null;
        internal string Output = null;

        internal virtual void Cleanup()
        {
            if (!Directory.Exists(Output))
                return;
            Directory.Delete(Output, true);
        }

        internal bool Execute(string[] args)
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
                Core.OverrideAppDomainBase(Destination);
                var generatorProcessInfo = new ProcessStartInfo(ExePath);
                generatorProcessInfo.Arguments = string.Join(" ", args.Where(s => !string.IsNullOrEmpty(s)).Select(it => ("\"" + Regex.Replace(it, @"(\\+)$", @"$1$1") + "\"")));
                generatorProcessInfo.UseShellExecute = false;
                generatorProcessInfo.RedirectStandardOutput = true;
                generatorProcessInfo.CreateNoWindow = true;
                Logger.Msg("\"" + ExePath + "\" " + generatorProcessInfo.Arguments);
                Process process = null;
                try { process = Process.Start(generatorProcessInfo); } catch (Exception e) { Logger.Error(e.ToString()); Core.OverrideAppDomainBase(Core.BasePath); return false; }
                StreamReader stdout = process.StandardOutput;
                Utils.SetProcessId(process.Id);
                while (!stdout.EndOfStream)
                    Logger.Msg(stdout.ReadLine());
                while (!process.HasExited)
                    Thread.Sleep(100);
                Utils.SetProcessId(0);
                Core.OverrideAppDomainBase(Core.BasePath);
                return true;
            }
            catch (Exception ex) { Logger.Error(ex.ToString()); }
            return false;
        }
    }
}