using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public static class MelonHandler
{
    internal static void Setup()
    {
        if (!Directory.Exists(MelonEnvironment.MelonsDirectory))
            Directory.CreateDirectory(MelonEnvironment.MelonsDirectory);
    }

    public static void LoadMelonsFromDirectory<T>(string path) where T : IMelonBase
    {
        //create a dummy instance
        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {typeof(T).Name}s from '{path}'...");

        bool hasWroteLine = false;

        string[] files = Directory.GetFiles(MelonEnvironment.MelonsDirectory, "*.dll");
        var assemblies = new List<Assembly>();
        foreach (string file in files)
        {
            if (!hasWroteLine)
            {
                hasWroteLine = true;
                MelonLogger.WriteLine(Color.Magenta);
            }

            Assembly assembly = Assembly.LoadFrom(file);
            assemblies.Add(assembly);
        }

        var melons = new List<T>();
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.GetInterface(typeof(T).FullName) != null)
                {
                    T melon = (T)Activator.CreateInstance(type);
                    melons.Add(melon);
                }
            }
        }

        if (hasWroteLine)
            MelonLogger.WriteSpacer();

        MelonLogger.Msg($"Loaded {melons.Count} {typeof(T).Name}s.");
    }

    public static void LoadMelonsFromAssembly<T>(Assembly assembly) where T : IMelonBase
    {
        //create a dummy instance

        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {typeof(T).Name}s from '{assembly.FullName}'...");

        var hasWroteLine = false;

        var melons = new List<T>();
        //get custom attributes
        object[] attributes = assembly.GetCustomAttributes(typeof(MelonInfoAttribute), false);
        if (attributes.Length == 0)
        {
            MelonLogger.Warning($"Failed to load {typeof(T).Name}s from '{assembly.FullName}': No MelonInfoAttribute found.");
            return;
        }
        
        foreach (object attribute in attributes)
        {
            if (attribute is not MelonInfoAttribute melonInfo) 
                continue;
            
            if (!hasWroteLine)
            {
                hasWroteLine = true;
                MelonLogger.WriteLine(Color.Magenta);
            }
                
            var melon = (T)Activator.CreateInstance(melonInfo.SystemType);
            melon.Logger = new MelonLogger.Instance(melonInfo.Name);
            melons.Add(melon);
        }
        
        
        if (hasWroteLine)
            MelonLogger.WriteSpacer();
        
        MelonLogger.Msg($"Loaded {melons.Count} {typeof(T).Name}s.");
    }
}