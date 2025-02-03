namespace MelonLoader.Engine.Unity.Packages
{
    internal class DeobfuscationRegex
    {
        internal string Regex = null;

        internal DeobfuscationRegex()
        {
            //Regex = LoaderConfig.Current.UnityEngine.ForceGeneratorRegex;
            if (string.IsNullOrEmpty(Regex))
                Regex = RemoteAPI.Info.ObfuscationRegex;
        }

        internal void Setup()
        {
            if (string.IsNullOrEmpty(Regex))
            {
                if (!string.IsNullOrEmpty(Config.Values.DeobfuscationRegex))
                {
                    AssemblyGenerator.AssemblyGenerationNeeded = true;
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Config.Values.DeobfuscationRegex))
                {
                    AssemblyGenerator.AssemblyGenerationNeeded = true;
                    return;
                }
                if (!Config.Values.DeobfuscationRegex.Equals(Regex))
                {
                    AssemblyGenerator.AssemblyGenerationNeeded = true;
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
