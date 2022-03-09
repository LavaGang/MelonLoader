﻿using System;
using System.IO;
using System.Reflection;

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
            if (!MelonUtils.IsGameIl2Cpp())
                return true;

            if (RunMethod != null)
            {
                int returnval = (int)RunMethod.Invoke(null, new object[0]);
                Fixes.ApplicationBase.Run(AppDomain.CurrentDomain);
                return (returnval == 0);
            }

            return false;
        }

        internal static void Load()
        {
            if (!MelonUtils.IsGameIl2Cpp())
                return;

            MelonLogger.Msg("Loading Il2CppAssemblyGenerator...");

#if __ANDROID__
            string BaseDirectory = Path.Combine(string.Copy(MelonUtils.GetApplicationPath()), "melonloader/etc/assembly_generation/managed");
#else
            string BaseDirectory = Path.Combine(MelonUtils.GameDirectory, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator");
#endif
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
    }
}