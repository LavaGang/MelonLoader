using System.IO;
using System.Runtime.CompilerServices;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages.Models
{
    internal class PackageBase
    {
        internal string Name;
        internal string URL;
        internal string FilePath;
        internal string Destination;
        internal string Version;

        internal virtual bool ShouldSetup() => true;
        internal bool Setup()
        {
            if (string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(URL))
                return true;

            if (!ShouldSetup())
            {
                Core.Logger.Msg($"{Name} is up to date.");
                return true;
            }

            Core.AssemblyGenerationNeeded = true;

            if (!MelonLaunchOptions.Il2CppAssemblyGenerator.OfflineMode
                && ((this is DeobfuscationMap) || !File.Exists(FilePath)))
            {
                Core.Logger.Msg($"Downloading {Name}...");
                if (!FileHandler.Download(URL, FilePath))
                {
                    ThrowInternalFailure($"Failed to Download {Name}!");
                    return false;
                }
            }

            Core.Logger.Msg($"Processing {Name}...");
            if (!FileHandler.Process(FilePath, Destination, MelonUtils.IsWindows ? null : Name))
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
