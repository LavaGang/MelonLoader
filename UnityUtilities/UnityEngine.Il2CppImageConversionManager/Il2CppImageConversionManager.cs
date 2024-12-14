using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;

namespace UnityEngine;

public class Il2CppImageConversionManager
{
    static Il2CppImageConversionManager()
    {
        EncodeToTGADelegateField = IL2CPP.ResolveICall<TextureOnlyDelegate>("UnityEngine.ImageConversion::EncodeToTGA");
        EncodeToEXRDelegateField = IL2CPP.ResolveICall<TextureAndFlagDelegate>("UnityEngine.ImageConversion::EncodeToEXR");
        EncodeToPNGDelegateField = IL2CPP.ResolveICall<TextureOnlyDelegate>("UnityEngine.ImageConversion::EncodeToPNG");
        EncodeToJPGDelegateField = IL2CPP.ResolveICall<TextureAndQualityDelegate>("UnityEngine.ImageConversion::EncodeToJPG");
        LoadImageDelegateField = IL2CPP.ResolveICall<LoadImageDelegate>("UnityEngine.ImageConversion::LoadImage");
    }

    public static Il2CppStructArray<byte> EncodeToTGA(Texture2D tex)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (EncodeToTGADelegateField == null)
            throw new NullReferenceException("The EncodeToTGADelegateField cannot be null.");
        Il2CppStructArray<byte> il2CppStructArray;
        var encodeToTGADelegateField = EncodeToTGADelegateField(IL2CPP.Il2CppObjectBaseToPtr(tex));
        if (encodeToTGADelegateField != IntPtr.Zero)
            il2CppStructArray = new Il2CppStructArray<byte>(encodeToTGADelegateField);
        else
            il2CppStructArray = null;
        return il2CppStructArray;
    }

    public static Il2CppStructArray<byte> EncodeToPNG(Texture2D tex)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (EncodeToPNGDelegateField == null)
            throw new NullReferenceException("The EncodeToPNGDelegateField cannot be null.");
        Il2CppStructArray<byte> il2CppStructArray;
        var encodeToPNGDelegateField = EncodeToPNGDelegateField(IL2CPP.Il2CppObjectBaseToPtr(tex));
        if (encodeToPNGDelegateField != IntPtr.Zero)
            il2CppStructArray = new Il2CppStructArray<byte>(encodeToPNGDelegateField);
        else
            il2CppStructArray = null;
        return il2CppStructArray;
    }

    public static Il2CppStructArray<byte> EncodeToJPG(Texture2D tex, int quality)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (EncodeToJPGDelegateField == null)
            throw new NullReferenceException("The EncodeToJPGDelegateField cannot be null.");
        Il2CppStructArray<byte> il2CppStructArray;
        var encodeToJPGDelegateField = EncodeToJPGDelegateField(IL2CPP.Il2CppObjectBaseToPtr(tex), quality);
        if (encodeToJPGDelegateField != IntPtr.Zero)
            il2CppStructArray = new Il2CppStructArray<byte>(encodeToJPGDelegateField);
        else
            il2CppStructArray = null;
        return il2CppStructArray;
    }
    public static Il2CppStructArray<byte> EncodeToJPG(Texture2D tex) => EncodeToJPG(tex, 75);

    public static Il2CppStructArray<byte> EncodeToEXR(Texture2D tex, Texture2D.EXRFlags flags)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (EncodeToEXRDelegateField == null)
            throw new NullReferenceException("The EncodeToEXRDelegateField cannot be null.");
        Il2CppStructArray<byte> il2CppStructArray;
        var encodeToEXRDelegateField = EncodeToEXRDelegateField(IL2CPP.Il2CppObjectBaseToPtr(tex), flags);
        if (encodeToEXRDelegateField != IntPtr.Zero)
            il2CppStructArray = new Il2CppStructArray<byte>(encodeToEXRDelegateField);
        else
            il2CppStructArray = null;
        return il2CppStructArray;
    }
    public static Il2CppStructArray<byte> EncodeToEXR(Texture2D tex) => EncodeToEXR(tex, 0);

    public static bool LoadImage(Texture2D tex, Il2CppStructArray<byte> data, bool markNonReadable)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (data == null)
            throw new ArgumentException("The data cannot be null.");
        if (LoadImageDelegateField == null)
            throw new NullReferenceException("The LoadImageDelegateField cannot be null.");
        return LoadImageDelegateField(IL2CPP.Il2CppObjectBaseToPtr(tex), IL2CPP.Il2CppObjectBaseToPtr(data), markNonReadable);
    }
    public static bool LoadImage(Texture2D tex, Il2CppStructArray<byte> data) => LoadImage(tex, data, false);

    private delegate IntPtr TextureOnlyDelegate(IntPtr tex);
    private delegate IntPtr TextureAndQualityDelegate(IntPtr tex, int quality);
    private delegate IntPtr TextureAndFlagDelegate(IntPtr tex, Texture2D.EXRFlags flags);
    private delegate bool LoadImageDelegate(IntPtr tex, IntPtr data, bool markNonReadable);
    private readonly static TextureAndFlagDelegate EncodeToEXRDelegateField;
    private readonly static TextureOnlyDelegate EncodeToTGADelegateField;
    private readonly static TextureOnlyDelegate EncodeToPNGDelegateField;
    private readonly static TextureAndQualityDelegate EncodeToJPGDelegateField;
    private readonly static LoadImageDelegate LoadImageDelegateField;
}