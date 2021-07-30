using System;
using System.IO;
using System.IO.Compression;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
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
				string filepath = Path.Combine(Destination, string.IsNullOrEmpty(NewFileName) ? filenamefromurl : NewFileName);
				MelonLogger.Msg($"Moving {tempfile} to {filepath}");
				if (File.Exists(filepath)) File.Delete(filepath);
				File.Move(tempfile, filepath);
				return true;
			}

			MelonLogger.Msg($"Extracting {tempfile} to {Destination}");
			try { ZipFile.ExtractToDirectory(tempfile, Destination); }
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
}