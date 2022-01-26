namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class DeobfuscationRegex
    {
        internal string Regex = null;

        internal DeobfuscationRegex()
        {
            Regex = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceRegex;
            if (string.IsNullOrEmpty(Regex))
                Regex = RemoteAPI.Info.ObfuscationRegex;
        }

        internal void Setup()
        {
            if (string.IsNullOrEmpty(Regex))
            {
                if (!string.IsNullOrEmpty(Config.Values.DeobfuscationRegex))
                {
                    Core.AssemblyGenerationNeeded = true;
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Config.Values.DeobfuscationRegex))
                {
                    Core.AssemblyGenerationNeeded = true;
                    return;
                }
                if (!Config.Values.DeobfuscationRegex.Equals(Regex))
                {
                    Core.AssemblyGenerationNeeded = true;
                    return;
                }
            }
        }

        internal void Save()
        {
            Config.Values.DeobfuscationRegex = Regex;
            Config.Save();
        }
    }
}
