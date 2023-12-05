using MelonLoader.Fixes;
using MelonLoader.Utils;
using System;

namespace MelonLoader
{
    public class Core
    {
        public static void Startup()
        {
            MelonDebug.Msg("MelonLoader.Core.Startup");

            MelonEnvironment.Initialize();

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