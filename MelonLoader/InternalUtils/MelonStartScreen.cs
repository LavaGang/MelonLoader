using System;
using System.IO;
using System.Reflection;

namespace MelonLoader.InternalUtils
{
    internal class MelonStartScreen
    {
        private static string FileName = "MelonStartScreen";
        private static Assembly asm = null;

        private static MethodInfo LoadAndRunMethod = null;
        private static MethodInfo LoadingModsMethod = null;
        private static MethodInfo FinishMethod = null;
        private static MethodInfo DisplayModLoadIssuesIfNeededMethod = null;
        private static MethodInfo OnApplicationStart_PluginsMethod = null;
        private static MethodInfo OnApplicationStart_PluginMethod = null;
        private static MethodInfo OnApplicationStart_ModsMethod = null;
        private static MethodInfo OnApplicationStart_ModMethod = null;
        private static MethodInfo OnApplicationLateStart_PluginsMethod = null;
        private static MethodInfo OnApplicationLateStart_PluginMethod = null;
        private static MethodInfo OnApplicationLateStart_ModsMethod = null;
        private static MethodInfo OnApplicationLateStart_ModMethod = null;

        internal static int LoadAndRun(Func<int> functionToWaitForAsync)
        {
            if (!MelonLaunchOptions.Core.StartScreen)
                return functionToWaitForAsync();

            Load();

            if (LoadAndRunMethod != null)
            {
                return (int)LoadAndRunMethod.Invoke(null, new object[] { functionToWaitForAsync });
            }

            return -1;
        }

        private static void Load()
        {
            MelonLogger.Msg("Loading MelonStartScreen...");

            string BaseDirectory = Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies");
            string AssemblyPath = Path.Combine(BaseDirectory, $"{FileName}.dll");
            if (!File.Exists(AssemblyPath))
            {
                MelonLogger.Error($"Failed to Find {FileName}.dll!");
                return;
            }

            try
            {
                asm = Assembly.LoadFrom(AssemblyPath);
                if (asm == null)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to Load Assembly for {FileName}.dll!");
                    return;
                }

                MonoInternals.MonoResolveManager.GetAssemblyResolveInfo(FileName).Override = asm;

                Type type = asm.GetType("MelonLoader.MelonStartScreen.Core");
                if (type == null)
                {
                    MelonLogger.ThrowInternalFailure($"Failed to Get Type for MelonLoader.MelonStartScreen.Core!");
                    return;
                }

                LoadAndRunMethod = type.GetMethod("LoadAndRun", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationStart_PluginsMethod = type.GetMethod("OnApplicationStart_Plugins", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationStart_PluginMethod = type.GetMethod("OnApplicationStart_Plugin", BindingFlags.NonPublic | BindingFlags.Static);
                LoadingModsMethod = type.GetMethod("LoadingMods", BindingFlags.NonPublic | BindingFlags.Static);
                DisplayModLoadIssuesIfNeededMethod = type.GetMethod("DisplayModLoadIssuesIfNeeded", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationStart_ModsMethod = type.GetMethod("OnApplicationStart_Mods", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationStart_ModMethod = type.GetMethod("OnApplicationStart_Mod", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationLateStart_PluginsMethod = type.GetMethod("OnApplicationLateStart_Plugins", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationLateStart_PluginMethod = type.GetMethod("OnApplicationLateStart_Plugin", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationLateStart_ModsMethod = type.GetMethod("OnApplicationLateStart_Mods", BindingFlags.NonPublic | BindingFlags.Static);
                OnApplicationLateStart_ModMethod = type.GetMethod("OnApplicationLateStart_Mod", BindingFlags.NonPublic | BindingFlags.Static);
                FinishMethod = type.GetMethod("Finish", BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch (Exception ex) { MelonLogger.ThrowInternalFailure($"MelonStartScreen Exception: {ex}"); }
        }

        internal static void OnApplicationStart_Plugins() =>
            OnApplicationStart_PluginsMethod?.Invoke(null, null);

        internal static void OnApplicationStart_Plugin(string name) =>
            OnApplicationStart_PluginMethod?.Invoke(null, new object[] { name });

        internal static void LoadingMods() =>
            LoadingModsMethod?.Invoke(null, null);

        internal static void DisplayModLoadIssuesIfNeeded() =>
            DisplayModLoadIssuesIfNeededMethod?.Invoke(null, null);

        internal static void OnApplicationStart_Mods() =>
            OnApplicationStart_ModsMethod?.Invoke(null, null);

        internal static void OnApplicationStart_Mod(string name) =>
            OnApplicationStart_ModMethod?.Invoke(null, new object[] { name });

        internal static void OnApplicationLateStart_Plugins() =>
            OnApplicationLateStart_PluginsMethod?.Invoke(null, null);

        internal static void OnApplicationLateStart_Plugin(string name) =>
            OnApplicationLateStart_PluginMethod?.Invoke(null, new object[] { name });

        internal static void OnApplicationLateStart_Mods() =>
            OnApplicationLateStart_ModsMethod?.Invoke(null, null);

        internal static void OnApplicationLateStart_Mod(string name) =>
            OnApplicationLateStart_ModMethod?.Invoke(null, new object[] { name });

        internal static void Finish() =>
            FinishMethod?.Invoke(null, null);
    }
}
