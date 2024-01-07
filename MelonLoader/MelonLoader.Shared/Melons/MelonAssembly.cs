using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MelonLoader.SemVer;
#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public sealed class MelonAssembly
{
    internal static List<MelonAssembly> loadedAssemblies = new();
    
    
    private bool melonsLoaded;
    private readonly List<MelonBase> loadedMelons = new();
    
    public Assembly Assembly { get; private set; }
    public string Location { get; private set; }
    public string Hash { get; private set; }
    
    
    private MelonAssembly(Assembly assembly, string location)
    {
        Assembly = assembly;
        Location = location ?? "";
        Hash = MelonUtils.ComputeSimpleSHA256Hash(Location);
    }
    
    public static MelonAssembly LoadMelonAssembly(string path, bool loadMelons = true)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        if (!File.Exists(path))
            return null;
        
        path = Path.GetFullPath(path);


        try
        {
#if NET6_0_OR_GREATER
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#else
            Assembly assembly = Assembly.LoadFile(path);
#endif

            return LoadMelonAssembly(path, assembly, loadMelons);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Failed to load MelonAssembly from '{path}':\n{ex}");
            return null;
        }
    }
    
    public static MelonAssembly LoadMelonAssembly(string path, Assembly assembly, bool loadMelons)
    {
        if (!File.Exists(path))
            path = assembly.Location;

        if (assembly == null)
        {
            MelonLogger.Error("Failed to load a Melon Assembly: Assembly cannot be null.");
            return null;
        }

        var ma = loadedAssemblies.Find(x => x.Assembly.FullName == assembly.FullName);
        if (ma != null)
            return ma;

        var shortPath = path;
        if (shortPath.StartsWith(MelonEnvironment.MelonBaseDirectory))
            shortPath = "." + shortPath.Remove(0, MelonEnvironment.MelonBaseDirectory.Length);

        ma = new MelonAssembly(assembly, path);
        loadedAssemblies.Add(ma);
        
        if (loadMelons)
            ma.LoadMelons();

        MelonLogger.MsgDirect(Color.DarkGray, $"Melon Assembly loaded: '{shortPath}'");
        MelonLogger.MsgDirect(Color.DarkGray, $"SHA256 Hash: '{ma.Hash}'");
        return ma;
    }

    public void LoadMelons()
    {
        if (melonsLoaded)
            return;
        
        melonsLoaded = true;
        
        //TODO: Custom Resolvers

        MelonInfoAttribute info = MelonUtils.PullAttributeFromAssembly<MelonInfoAttribute>(Assembly);
        
        if (info == null)
        {
            MelonLogger.Error($"Failed to load MelonAssembly from '{Location}': Assembly does not contain a MelonInfoAttribute.");
            return;
        }
        
        if (info.SystemType == null)
        {
            MelonLogger.Error($"Failed to load MelonAssembly from '{Location}': MelonInfoAttribute.SystemType cannot be null.");
            return;
        }
        
        if (!info.SystemType.IsSubclassOf(typeof(MelonBase)))
        {
            MelonLogger.Error($"Failed to load MelonAssembly from '{Location}': MelonInfoAttribute.SystemType must be a subclass of MelonBase.");
            return;
        }

        if (!SemVersion.TryParse(info.Version, out _))
        {
            MelonLogger.Error($"Melon '{info.Name}' by '{info.Author}' has an invalid version: '{info.Version}'. Versions must follow the Semantic Versioning standard.");
            return;
        }

        MelonBase melon;

        try
        {
            melon = (MelonBase) Activator.CreateInstance(info.SystemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            loadedMelons.Add(melon);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Failed to load MelonAssembly from '{Location}': Failed to create an instance of '{info.SystemType.FullName}'.\n{ex}");
            return;
        }
    }
}