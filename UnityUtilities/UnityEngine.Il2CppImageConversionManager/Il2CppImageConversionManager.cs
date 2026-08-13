using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader.InternalUtils;
using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

public static class Il2CppImageConversionManager
{
    static Il2CppImageConversionManager()
    {
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            EncodeToTGADelegateField_Unity6 = RuntimeInvoke.ResolveICall<TextureOnlyDelegate_Unity6>("UnityEngine.ImageConversion::EncodeToTGA_Injected");
            EncodeToEXRDelegateField_Unity6 = RuntimeInvoke.ResolveICall<TextureAndFlagDelegate_Unity6>("UnityEngine.ImageConversion::EncodeToEXR_Injected");
            EncodeToPNGDelegateField_Unity6 = RuntimeInvoke.ResolveICall<TextureOnlyDelegate_Unity6>("UnityEngine.ImageConversion::EncodeToPNG_Injected");
            EncodeToJPGDelegateField_Unity6 = RuntimeInvoke.ResolveICall<TextureAndQualityDelegate_Unity6>("UnityEngine.ImageConversion::EncodeToJPG_Injected");
            LoadImageDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadImageDelegate_Unity6>("UnityEngine.ImageConversion::LoadImage_Injected");
        }
        else
        {
            EncodeToTGADelegateField = RuntimeInvoke.ResolveICall<TextureOnlyDelegate>("UnityEngine.ImageConversion::EncodeToTGA");
            EncodeToEXRDelegateField = RuntimeInvoke.ResolveICall<TextureAndFlagDelegate>("UnityEngine.ImageConversion::EncodeToEXR");
            EncodeToPNGDelegateField = RuntimeInvoke.ResolveICall<TextureOnlyDelegate>("UnityEngine.ImageConversion::EncodeToPNG");
            EncodeToJPGDelegateField = RuntimeInvoke.ResolveICall<TextureAndQualityDelegate>("UnityEngine.ImageConversion::EncodeToJPG");
            LoadImageDelegateField = RuntimeInvoke.ResolveICall<LoadImageDelegate>("UnityEngine.ImageConversion::LoadImage");
        }
    }

    public static unsafe Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToTGA(Texture2D tex)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (EncodeToTGADelegateField_Unity6 == null)
                throw new NullReferenceException("The EncodeToTGADelegateField_Unity6 cannot be null.");

            BlittableArrayWrapper arrayWrapper = default;
            EncodeToTGADelegateField_Unity6(tex.m_CachedPtr, (ByReference<BlittableArrayWrapper>)(&arrayWrapper));
            Il2CppSystem.Byte[] array = null;
            arrayWrapper.Unmarshal(ref array);

            return (Il2CppArrayRank1<Il2CppSystem.Byte>)array;
        }
        else
        {
            if (EncodeToTGADelegateField == null)
                throw new NullReferenceException("The EncodeToTGADelegateField cannot be null.");

            var arrayPtr = EncodeToTGADelegateField(tex.Pointer);
            return Il2CppObjectPool.Get(arrayPtr) as Il2CppArrayRank1<Il2CppSystem.Byte>;
        }
    }

    public static unsafe Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToPNG(Texture2D tex)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (EncodeToPNGDelegateField_Unity6 == null)
                throw new NullReferenceException("The EncodeToPNGDelegateField_Unity6 cannot be null.");

            BlittableArrayWrapper arrayWrapper = default;
            EncodeToPNGDelegateField_Unity6(tex.m_CachedPtr, (ByReference<BlittableArrayWrapper>)(&arrayWrapper));
            Il2CppSystem.Byte[] array = null;
            arrayWrapper.Unmarshal(ref array);

            return (Il2CppArrayRank1<Il2CppSystem.Byte>)array;
        }
        else
        {
            if (EncodeToPNGDelegateField == null)
                throw new NullReferenceException("The EncodeToPNGDelegateField cannot be null.");

            var arrayPtr = EncodeToPNGDelegateField(tex.Pointer);
            return Il2CppObjectPool.Get(arrayPtr) as Il2CppArrayRank1<Il2CppSystem.Byte>;
        }
    }

    public static unsafe Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToJPG(Texture2D tex, int quality)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (EncodeToJPGDelegateField_Unity6 == null)
                throw new NullReferenceException("The EncodeToJPGDelegateField_Unity6 cannot be null.");

            BlittableArrayWrapper arrayWrapper = default;
            EncodeToJPGDelegateField_Unity6(tex.m_CachedPtr, quality, (ByReference<BlittableArrayWrapper>)(&arrayWrapper));
            Il2CppSystem.Byte[] array = null;
            arrayWrapper.Unmarshal(ref array);

            return (Il2CppArrayRank1<Il2CppSystem.Byte>)array;
        }
        else
        {
            if (EncodeToJPGDelegateField == null)
                throw new NullReferenceException("The EncodeToJPGDelegateField cannot be null.");

            var arrayPtr = EncodeToJPGDelegateField(tex.Pointer, quality);
            return Il2CppObjectPool.Get(arrayPtr) as Il2CppArrayRank1<Il2CppSystem.Byte>;
        }
    }
    public static Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToJPG(Texture2D tex) => EncodeToJPG(tex, 75);

    public static unsafe Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToEXR(Texture2D tex, Texture2D.EXRFlags flags)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (EncodeToEXRDelegateField_Unity6 == null)
                throw new NullReferenceException("The EncodeToEXRDelegateField_Unity6 cannot be null.");

            BlittableArrayWrapper arrayWrapper = default;
            EncodeToEXRDelegateField_Unity6(tex.m_CachedPtr, flags, (ByReference<BlittableArrayWrapper>)(&arrayWrapper));
            Il2CppSystem.Byte[] array = null;
            arrayWrapper.Unmarshal(ref array);

            return (Il2CppArrayRank1<Il2CppSystem.Byte>)array;
        }
        else
        {
            if (EncodeToEXRDelegateField == null)
                throw new NullReferenceException("The EncodeToEXRDelegateField cannot be null.");

            var arrayPtr = EncodeToEXRDelegateField(tex.Pointer, flags);
            return Il2CppObjectPool.Get(arrayPtr) as Il2CppArrayRank1<Il2CppSystem.Byte>;
        }
    }
    public static Il2CppArrayRank1<Il2CppSystem.Byte> EncodeToEXR(Texture2D tex) => EncodeToEXR(tex, default);

    public static bool LoadImage(Texture2D tex, Il2CppArrayRank1<Il2CppSystem.Byte> data, bool markNonReadable)
    {
        if (tex == null)
            throw new ArgumentException("The texture cannot be null.");
        if (data == null)
            throw new ArgumentException("The data cannot be null.");

        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadImageDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadImageDelegateField_Unity6 cannot be null.");

            unsafe
            {
                var span = new ManagedSpanWrapper
                {
                    begin = (Pointer<Il2CppSystem.Void>)(void*)data.GetElementAddress(0),
                    length = data.Length
                };
                return LoadImageDelegateField_Unity6(tex.m_CachedPtr, (ByReference<ManagedSpanWrapper>)(&span), markNonReadable);
            }
        }
        else
        {
            if (LoadImageDelegateField == null)
                throw new NullReferenceException("The LoadImageDelegateField cannot be null.");
            return LoadImageDelegateField(tex.Pointer, data.Pointer, markNonReadable);
        }
    }
    public static bool LoadImage(Texture2D tex, Il2CppArrayRank1<Il2CppSystem.Byte> data) => LoadImage(tex, data, false);

    private delegate Il2CppSystem.IntPtr TextureOnlyDelegate(Il2CppSystem.IntPtr tex);
    private delegate Il2CppSystem.IntPtr TextureAndQualityDelegate(Il2CppSystem.IntPtr tex, Il2CppSystem.Int32 quality);
    private delegate Il2CppSystem.IntPtr TextureAndFlagDelegate(Il2CppSystem.IntPtr tex, Texture2D.EXRFlags flags);
    private delegate bool LoadImageDelegate(Il2CppSystem.IntPtr tex, Il2CppSystem.IntPtr data, Il2CppSystem.Boolean markNonReadable);
    private readonly static TextureAndFlagDelegate EncodeToEXRDelegateField;
    private readonly static TextureOnlyDelegate EncodeToTGADelegateField;
    private readonly static TextureOnlyDelegate EncodeToPNGDelegateField;
    private readonly static TextureAndQualityDelegate EncodeToJPGDelegateField;
    private readonly static LoadImageDelegate LoadImageDelegateField;
    // ----------
    private delegate void TextureOnlyDelegate_Unity6(Il2CppSystem.IntPtr tex, [Out] ByReference<BlittableArrayWrapper> ret);
    private delegate void TextureAndQualityDelegate_Unity6(Il2CppSystem.IntPtr tex, Il2CppSystem.Int32 quality, [Out] ByReference<BlittableArrayWrapper> ret);
    private delegate void TextureAndFlagDelegate_Unity6(Il2CppSystem.IntPtr tex, Texture2D.EXRFlags flags, [Out] ByReference<BlittableArrayWrapper> ret);
    private delegate bool LoadImageDelegate_Unity6(Il2CppSystem.IntPtr tex, ByReference<ManagedSpanWrapper> data, Il2CppSystem.Boolean markNonReadable);
    private readonly static TextureAndFlagDelegate_Unity6 EncodeToEXRDelegateField_Unity6;
    private readonly static TextureOnlyDelegate_Unity6 EncodeToTGADelegateField_Unity6;
    private readonly static TextureOnlyDelegate_Unity6 EncodeToPNGDelegateField_Unity6;
    private readonly static TextureAndQualityDelegate_Unity6 EncodeToJPGDelegateField_Unity6;
    private readonly static LoadImageDelegate_Unity6 LoadImageDelegateField_Unity6;
}