using System.Collections.Generic;
using System.IO;
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public static class MelonHandler
{
    public static void LoadMelonsFromDirectory<T>(string path) where T : MelonTypeBase<T>
    {
        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {MelonTypeBase<T>.TypeName}s from '{path}'...");

        string[] files = Directory.GetFiles(path, "*.dll");
        var melonAssemblies = new List<MelonAssembly>();

        foreach (string file in files)
        {
            MelonAssembly melonAssembly = MelonAssembly.LoadMelonAssembly(file, true);
            if (melonAssembly == null)
                continue;

            melonAssemblies.Add(melonAssembly);
        }
    }
}