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
                MelonLogger.Msg($"{Name} is up to date.");
                return true;
            }

            Core.AssemblyGenerationNeeded = true;

            if (!MelonLaunchOptions.Il2CppAssemblyGenerator.OfflineMode
                && !File.Exists(FilePath))
            {
                MelonLogger.Msg($"Downloading {Name}...");
                if (!FileHandler.Download(URL, FilePath))
                {
                    ThrowInternalFailure($"Failed to Download {Name}!");
                    return false;
                }
            }

            MelonLogger.Msg($"Processing {Name}...");
            if (!FileHandler.Process(FilePath, Destination))
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void ThrowInternalFailure(string txt);
    }
}
