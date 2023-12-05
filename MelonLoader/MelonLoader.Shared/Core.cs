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

            UnhandledException.Install(AppDomain.CurrentDomain);
            AssemblyResolveSearchFix.Install();
        }

        public static void OnApplicationPreStart()
        {

        }

        public static void OnApplicationStart()
        {

        }
    }
}