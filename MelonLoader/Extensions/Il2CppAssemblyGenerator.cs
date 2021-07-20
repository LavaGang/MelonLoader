using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    internal static class Il2CppAssemblyGenerator
    {
        private static string FileNameWithExtension = "Il2CppAssemblyGenerator.dll";
        private static Assembly asm = null;
        private static MethodInfo RunMethod = null;
        internal static Assembly AssemblyResolver(object sender, ResolveEventArgs args) => args.Name.StartsWith($"{Path.GetFileNameWithoutExtension(FileNameWithExtension)}, Version=") ? asm : null;

        internal static bool Run()
        {
            Load();

            if (RunMethod != null)
            {
                DisableCloseButton();
                int returnval = (int)RunMethod.Invoke(null, new object[0]);
                EnableCloseButton();
                Fixes.ApplicationBase.Run(AppDomain.CurrentDomain);
                return (returnval == 0);
            }

            return false;
        }

        private static void Load()
        {
            MelonLogger.Msg("Loading Il2CppAssemblyGenerator...");

            string BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies"), "Il2CppAssemblyGenerator");
            if (!Directory.Exists(BaseDirectory))
            {
                MelonLogger.Error("Failed to Find Il2CppAssemblyGenerator Directory!");
                return;
            }

            string AssemblyPath = Path.Combine(BaseDirectory, FileNameWithExtension);
            if (!File.Exists(AssemblyPath))
            {
                MelonLogger.Error($"Failed to Find {FileNameWithExtension}!");
                return;
            }

            try
            {
                asm = Assembly.LoadFrom(AssemblyPath);
                if (asm == null)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for {FileNameWithExtension}!");
                    return;
                }

                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void EnableCloseButton();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void DisableCloseButton();
    }
}