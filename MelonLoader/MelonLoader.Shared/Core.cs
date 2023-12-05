using MelonLoader.Fixes;
using MelonLoader.Utils;
using System;

namespace MelonLoader
{
    public class Core
    {
        public static void Startup()
        {
            MelonEnvironment.Initialize();
            MelonDebug.Msg("MelonLoader.Core.Startup");

            UnhandledException.Install(AppDomain.CurrentDomain);
            AssemblyResolveSearchFix.Install();

        }

        public static void OnApplicationPreStart()
        {
            MelonDebug.Msg("MelonLoader.Core.OnApplicationPreStart");
        }

        public static void OnApplicationStart()
        {
            MelonDebug.Msg("MelonLoader.Core.OnApplicationStart");
        }
    }
}