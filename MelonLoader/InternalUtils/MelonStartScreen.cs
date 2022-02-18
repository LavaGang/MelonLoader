using System;
using System.IO;
using System.Reflection;
using AssetRipper.VersionUtilities;
using Semver;

namespace MelonLoader.InternalUtils
{
    internal class MelonStartScreen
    {
        private static string FileName = "MelonStartScreen";
        private static Assembly asm = null;

        private static MethodInfo LoadAndRunMethod = null;

        internal static int LoadAndRun(LemonFunc<int> functionToWaitForAsync)
        {
            if (!MelonLaunchOptions.Core.StartScreen)
                return functionToWaitForAsync();

            // Doesn't support Unity versions lower than 2017.2.0 (yet?)
            // Doesn't support Unity versions lower than 2018 (Crashing Issue)
            UnityVersion unityVersion = UnityInformationHandler.EngineVersion;
            UnityVersion minimumVersion = new UnityVersion(2018);
            if (unityVersion < minimumVersion)
                return functionToWaitForAsync();

            // Doesn't support Unity versions higher than to 2020.3.21 (Crashing Issue)
            UnityVersion maximumVersion = new UnityVersion(2020, 3, 21);
            if (unityVersion > maximumVersion)
                return functionToWaitForAsync();

            if (!Load())
                return functionToWaitForAsync();

            if (LoadAndRunMethod != null)
                return (int)LoadAndRunMethod.Invoke(null, new object[] { functionToWaitForAsync });

            return -1;
        }

        private static bool Load()
        {
            MelonLogger.Msg("Loading MelonStartScreen...");

            string BaseDirectory = Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies");
            string AssemblyPath = Path.Combine(BaseDirectory, $"{FileName}.dll");
            if (!File.Exists(AssemblyPath))
            {
                MelonLogger.Error($"Failed to Find {FileName}.dll!");
                return false;
            }

            try
            {
                asm = Assembly.LoadFrom(AssemblyPath);
                if (asm == null)
                {
                    MelonLogger.Error($"Failed to Load Assembly for {FileName}.dll!");
                    return false;
                }

                MonoInternals.MonoResolveManager.GetAssemblyResolveInfo(FileName).Override = asm;

                Type type = asm.GetType("MelonLoader.MelonStartScreen.Core");
                if (type == null)
                {
                    MelonLogger.Error($"Failed to Get Type for MelonLoader.MelonStartScreen.Core!");
                    return false;
                }

                LoadAndRunMethod = type.GetMethod("LoadAndRun", BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch (Exception ex) { MelonLogger.Error($"MelonStartScreen Exception: {ex}"); return false; }
            return true;
        }
    }
}
