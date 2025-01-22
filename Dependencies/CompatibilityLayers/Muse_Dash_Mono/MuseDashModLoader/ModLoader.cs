using MelonLoader;
using ModHelper;
using System.Collections.Generic;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ModLoader;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ModLoader
{
    internal static List<IMod> mods = [];
    internal static Dictionary<string, Assembly> depends = [];

    public static void LoadDependency(Assembly assembly)
    {
        foreach (var dependStr in assembly.GetManifestResourceNames())
        {
            var filter = $"{assembly.GetName().Name}.Depends.";
            if (dependStr.StartsWith(filter) && dependStr.EndsWith(".dll"))
            {
                var dependName = dependStr.Remove(dependStr.LastIndexOf(".dll")).Remove(0, filter.Length);
                if (depends.ContainsKey(dependName))
                {
                    MelonLogger.Error($"Dependency conflict: {dependName} First at: {depends[dependName].GetName().Name}");
                    continue;
                }

                Assembly dependAssembly;
                using (var stream = assembly.GetManifestResourceStream(dependStr))
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    dependAssembly = Assembly.Load(buffer);
                }

                depends.Add(dependName, dependAssembly);
            }
        }
    }
}