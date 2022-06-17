using MelonLoader.Modules;
using System.Collections.Generic;
using System.IO;

namespace MelonLoader
{
    public static class MelonCompatibilityLayer
    {
        public static string baseDirectory = $"MelonLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}CompatibilityLayers";

        private static List<MelonModule.Info> layers = new List<MelonModule.Info>()
        {
            // Il2Cpp Unity Tls
            new MelonModule.Info(Path.Combine(baseDirectory, "Il2CppUnityTls.dll"), () => !MelonUtils.IsGameIl2Cpp()),

            // Illusion Plugin Architecture
            new MelonModule.Info(Path.Combine(baseDirectory, "IPA.dll"), MelonUtils.IsGameIl2Cpp),

            // MuseDashModLoader
            new MelonModule.Info(Path.Combine(baseDirectory, "MDML.dll"), MelonUtils.IsGameIl2Cpp, () => !MelonUtils.IsMuseDash),

            // Demeo Integration
            new MelonModule.Info(Path.Combine(baseDirectory, "Demeo.dll"), MelonUtils.IsGameIl2Cpp, () => !MelonUtils.IsDemeo)
        };

        internal static void LoadModules()
        {
            if (!Directory.Exists(baseDirectory))
                return;

            foreach (var m in layers)
                MelonModule.Load(m);
        }
    }
}