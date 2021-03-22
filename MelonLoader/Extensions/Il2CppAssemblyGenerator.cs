using System;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    internal static class Il2CppAssemblyGenerator
    {
        private static MethodInfo RunMethod = null;

        internal static bool Run()
        {
            MelonLogger.Msg("Loading Il2Cpp Assembly Generator...");
            if (RunMethod == null)
            {
                string BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "Dependencies"), "AssemblyGenerator");
                if (!Directory.Exists(BaseDirectory))
                {
                    MelonLogger.Error("Failed to Find AssemblyGenerator Directory!");
                    return false;
                }
                string AssemblyPath = Path.Combine(BaseDirectory, "AssemblyGenerator.dll");
                if (!File.Exists(AssemblyPath))
                {
                    MelonLogger.Error("Failed to Find AssemblyGenerator.dll!");
                    return false;
                }
                try
                {
                    Assembly asm = Assembly.LoadFrom(AssemblyPath);
                    if (asm == null)
                    {
                        MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for AssemblyGenerator.dll!");
                        return false;
                    }
                    Type type = asm.GetType("MelonLoader.AssemblyGenerator.Core");
                    if (type == null)
                    {
                        MelonLogger.ThrowInternalFailure($"Failed to Get Type for MelonLoader.AssemblyGenerator.Core!");
                        return false;
                    }
                    RunMethod = type.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
                }
                catch (Exception ex)
                {
                    MelonLogger.ThrowInternalFailure($"Il2Cpp Assembly Generator Exception: {ex}");
                    return false;
                }
            }
            if (RunMethod != null)
            {
                int returnval = (int)RunMethod.Invoke(null, new object[0]);
                Fixes.ApplicationBase.Run(AppDomain.CurrentDomain);
                return returnval == 0;
            }
            MelonLogger.ThrowInternalFailure($"Failed to Get Run Method for MelonLoader.AssemblyGenerator.Core!");
            return false;
        }
    }
}