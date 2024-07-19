using System.IO;

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

        internal virtual bool OnProcess()
            => FileHandler.Process(FilePath, Destination, MelonUtils.IsWindows ? null : Name);

        internal virtual bool Setup()
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
