using System.IO;
using System.Reflection;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Shared
{
    public class Core
    {
        public static void Startup()
        {
            var runtimeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var runtimeDirInfo = new DirectoryInfo(runtimeFolder);
            MelonEnvironment.MelonLoaderDirectory = runtimeDirInfo.Parent!.FullName;
        }
    }
}