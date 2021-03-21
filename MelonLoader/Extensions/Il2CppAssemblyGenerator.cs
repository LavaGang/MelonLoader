using System;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    internal static class Il2CppAssemblyGenerator
    {
        internal static MethodInfo RunMethod = null;

        internal static bool Run()
        {
            if (RunMethod == null)
            {
                MelonLogger.Msg("Loading Assembly Generator...");
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
                        MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for Il2CppAssemblyGenerator!");
                        return false;
                    }
                    Type type = asm.GetType("MelonLoader.AssemblyGenerator.Core");
                    if (type == null)
                    {
                        MelonLogger.ThrowInternalFailure($"Failed to Get Type for Il2CppAssemblyGenerator!");
                        return false;
                    }
                    RunMethod = type.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
                }
                catch (Exception ex)
                {
                    MelonLogger.ThrowInternalFailure($"Il2CppAssemblyGenerator Exception: {ex}");
                    return false;
                }
            }
            if (RunMethod != null)
                return (((int)RunMethod.Invoke(null, new object[0])) == 0);
            MelonLogger.ThrowInternalFailure($"Failed to Get Run Method for Il2CppAssemblyGenerator!");
            return false;
        }
    }
}