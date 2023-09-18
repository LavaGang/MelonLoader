using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using MelonLoader.Shared.Interfaces;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Bootstrap;

public class ModuleManager
{
    public static BootstrapModule FindBootstrapModule()
    {
        BootstrapModule obj = null;
        
        foreach (var folder in Directory.GetDirectories(MelonEnvironment.ModuleDirectory))
        {
            var bootstrapPath = Path.Combine(folder, "Bootstrap.dll");
            if (!File.Exists(bootstrapPath))
                continue;
            
            MelonDebug.Msg($"Loading {bootstrapPath}");
            //TODO: Fix dotnet stupidly not using default load context when hosting coreclr.
            var assembly = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).LoadFromAssemblyPath(bootstrapPath);

            var type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i == typeof(BootstrapModule)));
            if (type == null)
            {
                MelonLogger.Warning($"Failed to load BootstrapModule '{bootstrapPath}': No type deriving from BootstrapModule found.");
                continue;
            }
            
            try
            {
                obj = (BootstrapModule)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to initialize BootstrapModule '{bootstrapPath}':\n{ex}");
                continue;
            }
            
            if (!obj.IsMyEngine)
                obj = null;
        }

        return obj;
    }
}