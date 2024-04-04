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
        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {typeof(T).Name}s from '{path}'...");

        var hasWroteLine = false;

        string[] files = Directory.GetFiles(MelonEnvironment.MelonsDirectory, "*.dll");
        foreach (string file in files)
        {
            if (!hasWroteLine)
            {
                hasWroteLine = true;
                MelonLogger.WriteLine(Color.Magenta);
            }

            var assembly = Assembly.LoadFrom(file);
            LoadMelonsFromAssembly<T>(assembly);
        }
    }

    public static void LoadMelonsFromAssembly<T>(Assembly assembly) where T : IMelonBase
    {
        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {typeof(T).Name}s from '{assembly.FullName}'...");

        var hasWroteLine = false;

        var melons = new List<T>();
        object[] attributes = assembly.GetCustomAttributes(typeof(MelonInfoAttribute), false);
        if (attributes.Length == 0)
        {
            MelonLogger.Warning(
                $"Failed to load {typeof(T).Name}s from '{assembly.FullName}': No MelonInfoAttribute found.");
            return;
        }
        
        //TODO: This stuff should be turned into extension methods. Or perhaps recreate our "MelonAssembly" wrapper.
        foreach (object attribute in attributes)
        {
            if (attribute is not MelonInfoAttribute melonInfo)
                continue;

            if (!hasWroteLine)
            {
                hasWroteLine = true;
                MelonLogger.WriteLine(Color.Magenta);
            }

            if (!typeof(T).IsAssignableFrom(melonInfo.SystemType))
                continue;

            var melon = (T)Activator.CreateInstance(melonInfo.SystemType);
            melon.Logger = new MelonLogger.Instance(melonInfo.Name);
            melons.Add(melon);
        }


        if (hasWroteLine)
            MelonLogger.WriteSpacer();

        MelonLogger.Msg($"Loaded {melons.Count} {typeof(T).Name}s.");
    }
}