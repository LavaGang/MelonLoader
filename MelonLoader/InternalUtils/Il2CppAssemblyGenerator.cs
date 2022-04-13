using MelonLoader.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MelonLoader.InternalUtils
{
    internal static class Il2CppAssemblyGenerator
    {
        private static string FileName = "Il2CppAssemblyGenerator";
        private static Assembly asm = null;
        private static MethodInfo RunMethod = null;

        internal static bool Run()
        {
            Load();

            if (RunMethod != null)
            {
                IntPtr windowHandle = Process.GetCurrentProcess().MainWindowHandle;
                BootstrapInterop.DisableCloseButton(windowHandle);
                int returnval = (int)RunMethod.Invoke(null, new object[0]);
                BootstrapInterop.EnableCloseButton(windowHandle);
                MelonUtils.SetCurrentDomainBaseDirectory(MelonUtils.GameDirectory);
                return returnval == 0;
            }

            return false;
        }

        private static void Load()
        {
            MelonLogger.Msg("Loading Il2CppAssemblyGenerator...");

            string BaseDirectory = MelonEnvironment.Il2CppAssemblyGeneratorDirectory;
            if (!Directory.Exists(BaseDirectory))
            {
                MelonLogger.Error("Failed to Find Il2CppAssemblyGenerator Directory!");
                return;
            }

            string AssemblyPath = Path.Combine(BaseDirectory, $"{FileName}.dll");
            if (!File.Exists(AssemblyPath))
            {
                MelonLogger.Error($"Failed to Find {FileName}.dll!");
                return;
            }

            try
            {
#if NET6_0
                asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(AssemblyPath);
#else
                asm = Assembly.LoadFrom(AssemblyPath);
#endif
                if (asm == null)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for {FileName}.dll!");
                    return;
                }

                MonoInternals.MonoResolveManager.GetAssemblyResolveInfo(FileName).Override = asm;

                Type type = asm.GetType("MelonLoader.Il2CppAssemblyGenerator.Core");
                if (type == null)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to Get Type for MelonLoader.Il2CppAssemblyGenerator.Core!");
                    return;
                }

                RunMethod = type.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch (Exception ex) { MelonLogger.ThrowInternalFailure($"Il2CppAssemblyGenerator Exception: {ex}"); }
        }

        
    }
}