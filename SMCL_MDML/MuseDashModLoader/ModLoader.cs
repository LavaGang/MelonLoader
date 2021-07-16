using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using ModHelper;
#pragma warning disable 0618

namespace ModLoader
{
    public class ModLoader
    {
        [Obsolete("ModLoader.ModLoader.mods is Only Here for Compatibility Reasons. Please use MelonHandler.Mods instead.")]
        internal static List<IMod> mods = new List<IMod>();

        [Obsolete("ModLoader.ModLoader.depends is Only Here for Compatibility Reasons.")]
        internal static Dictionary<string, Assembly> depends = new Dictionary<string, Assembly>();
        
        public static void LoadDependency(Assembly assembly)
        {
            foreach (string dependStr in assembly.GetManifestResourceNames())
            {
                string filter = $"{assembly.GetName().Name}.Depends.";
                if (dependStr.StartsWith(filter) && dependStr.EndsWith(".dll"))
                {
                    string dependName = dependStr.Remove(dependStr.LastIndexOf(".dll")).Remove(0, filter.Length);
                    if (depends.ContainsKey(dependName))
                    {
                        MelonLogger.Error($"Dependency conflict: {dependName} First at: {depends[dependName].GetName().Name}");
                        continue;
                    }

                    Assembly dependAssembly;
                    using (var stream = assembly.GetManifestResourceStream(dependStr))
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        dependAssembly = Assembly.Load(buffer);
                    }
                    depends.Add(dependName, dependAssembly);
                }
            }
        }
    }
}