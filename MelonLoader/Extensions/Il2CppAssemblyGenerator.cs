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
            MelonLogger.Msg("Loading Il2CppAssemblyGenerator...");
            if (RunMethod == null)
            {
#if __ANDROID__
                string BaseDirectory = Path.Combine(string.Copy(MelonUtils.GetApplicationPath()), "files", "melonloader", "etc", "assembly_generation", "managed");
#else
                string BaseDirectory = Path.Combine(MelonUtils.GameDirectory, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator");
#endif
                if (!Directory.Exists(BaseDirectory))
                {
                    MelonLogger.Error("Failed to Find Il2CppAssemblyGenerator Directory!");
                    return false;
                }
                string AssemblyPath = Path.Combine(BaseDirectory, "Il2CppAssemblyGenerator.dll");
                if (!File.Exists(AssemblyPath))
                {
                    MelonLogger.Error("Failed to Find Il2CppAssemblyGenerator.dll!");
                    return false;
                }
                try
                {
                    Assembly asm = Assembly.LoadFrom(AssemblyPath);
                    if (asm == null)
                    {
                        MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for Il2CppAssemblyGenerator.dll!");
                        return false;
                    }
                    Type type = asm.GetType("MelonLoader.Il2CppAssemblyGenerator.Core");
                    if (type == null)
                    {
                        MelonLogger.ThrowInternalFailure($"Failed to Get Type for MelonLoader.Il2CppAssemblyGenerator.Core!");
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