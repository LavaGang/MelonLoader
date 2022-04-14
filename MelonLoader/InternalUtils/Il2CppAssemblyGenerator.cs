using MelonLoader.Modules;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MelonLoader.InternalUtils
{
    internal static class Il2CppAssemblyGenerator
    {
        public static readonly MelonModule.Info moduleInfo = new MelonModule.Info(
            $"MelonLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}Il2CppAssemblyGenerator{Path.DirectorySeparatorChar}Il2CppAssemblyGenerator.dll"
            , () => !MelonUtils.IsGameIl2Cpp());

        internal static bool Run()
        {
            MelonLogger.Msg("Loading Il2CppAssemblyGenerator...");

            var module = MelonModule.Load(moduleInfo);
            if (module == null)
                return true;

            MonoInternals.MonoResolveManager.GetAssemblyResolveInfo("Il2CppAssemblyGenerator").Override = module.Assembly;

            IntPtr windowHandle = Process.GetCurrentProcess().MainWindowHandle;
            BootstrapInterop.DisableCloseButton(windowHandle);
            var ret = module.SendMessage("Run");
            BootstrapInterop.EnableCloseButton(windowHandle);
            MelonUtils.SetCurrentDomainBaseDirectory(MelonUtils.GameDirectory);
            return ret is int retVal && retVal == 0;
        }

        
    }
}