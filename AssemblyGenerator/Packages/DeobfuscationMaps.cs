using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class DeobfuscationMaps : PackageBase
    {
        internal DeobfuscationMaps()
        {
            Version = "";
            URL = "";
            Destination = Path.Combine(Core.BasePath, "Il2CppAssemblyUnhollower");
        }

        internal override bool Download()
        {
            return true;
        }
    }
}
