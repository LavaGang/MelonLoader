using MelonLoader.Utils;
using System.IO;

namespace MelonLoader.Engine.Unity.Packages.Models
{
    internal class PackageBase
    {
        internal string Name;
        internal string URL;
        internal string FilePath;
        internal string Destination;
        internal string Version;

        internal virtual bool ShouldSetup() => true;

        internal virtual bool OnProcess()
            => FileHandler.Process(FilePath, Destination, OsUtils.IsWindows ? null : Name);

        internal virtual bool Setup()
        {
            if (string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(URL))
                return true;

            if (!ShouldSetup())
            {
                AssemblyGenerator.Logger.Msg($"{Name} is up to date.");
                return true;
            }

            AssemblyGenerator.AssemblyGenerationNeeded = true;

            if (//!LoaderConfig.Current.UnityEngine.ForceOfflineGeneration &&
                ((this is DeobfuscationMap) || !File.Exists(FilePath)))
            {
                AssemblyGenerator.Logger.Msg($"Downloading {Name}...");
                if (!FileHandler.Download(URL, FilePath))
                {
                    ThrowInternalFailure($"Failed to Download {Name}!");
                    return false;
                }
            }

            AssemblyGenerator.Logger.Msg($"Processing {Name}...");
            if (!OnProcess())
            {
                ThrowInternalFailure($"Failed to Process {Name}!");
                return false;
            }

            Save();
            return true;
        }

        internal virtual void Save() { }
        internal void Save(ref string configPref)
        {
            configPref = Version;
            Config.Save();
        }
        
        internal static void ThrowInternalFailure(string txt) => MelonLogger.ThrowInternalFailure(txt);
    }
}
