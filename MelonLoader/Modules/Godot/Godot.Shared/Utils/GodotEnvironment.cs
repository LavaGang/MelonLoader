using System;
using System.IO;
using MelonLoader.Utils;
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
using GodotPCKExplorer;
#endif
namespace MelonLoader.Godot.Utils;

public readonly struct GodotVersion  : 
    IEquatable<GodotVersion>,
    IComparable,
    IComparable<GodotVersion>
{
    public int Major { get; }
    public int Minor { get; }
    public int Revision { get; }

    public GodotVersion(int major, int minor, int revision)
    {
        Major = major;
        Minor = minor;
        Revision = revision;
    }
    
    public bool Equals(GodotVersion other)
    {
        return CompareTo(other) == 0;
    }

    public int CompareTo(object obj)
    {
        if (obj is GodotVersion other)
            return CompareTo(other);
        return 0;
    }

    public int CompareTo(GodotVersion other)
    {
        if (Major != other.Major)
            return Major.CompareTo(other.Major);
        if (Minor != other.Minor)
            return Minor.CompareTo(other.Minor);
        if (Revision != other.Revision)
            return Revision.CompareTo(other.Revision);
        return 0;
    }
    
    public override string ToString() => $"{Major}.{Minor}.{Revision}";
}

public static class GodotEnvironment
{
    private static bool _isInitialized;
    
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    private static PCKReader _pckReader = new PCKReader();
#endif
    
    public static string GameName { get; private set; }
    public static string GameDeveloper { get; private set; }
    public static string GameDataPath { get; private set; }
    public static GodotVersion EngineVersion { get; private set; }
    
    public static void Initialize(string pckPath)
    {
        // Check if already Initialized
        if (_isInitialized)
            return;
        
        _isInitialized = true;

        ReadGameInfo(pckPath);
    }
    
    private static void ReadGameInfo(string pckPath)
    {
#if NET6_0_OR_GREATER
        if (!_pckReader.OpenFile(pckPath))
            return;

        GameName = Path.GetFileNameWithoutExtension(_pckReader.PackPath);
        EngineVersion = new GodotVersion(_pckReader.PCK_VersionMajor, _pckReader.PCK_VersionMinor, _pckReader.PCK_VersionRevision);
#endif
        
        GameDataPath = $"data_{MelonEnvironment.GameExecutableName}";
    }
}