using System.IO;
using System.Reflection;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Shared
{
    public class Core
    {
        public static void Startup()
        {
            MelonEnvironment.Initialize();
        }
    }
}