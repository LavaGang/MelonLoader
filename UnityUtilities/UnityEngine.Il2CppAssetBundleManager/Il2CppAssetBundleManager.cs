using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using MelonLoader.InternalUtils;
using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

// New struct needed for Unity 6 function calls.
[StructLayout(LayoutKind.Sequential)]
public struct ManagedSpanWrapper
{
    public unsafe void* begin;
    public int length;
}

public class Il2CppAssetBundleManager
{
    static Il2CppAssetBundleManager()
    {
        // icalls whose signature hasn't changed.
        GetAllLoadedAssetBundles_NativeDelegateField = IL2CPP.ResolveICall<GetAllLoadedAssetBundles_NativeDelegate>("UnityEngine.AssetBundle::GetAllLoadedAssetBundles_Native");
        UnloadAllAssetBundlesDelegateField = IL2CPP.ResolveICall<UnloadAllAssetBundlesDelegate>("UnityEngine.AssetBundle::UnloadAllAssetBundles");

        if (UnityInformationHandler.EngineVersion.Major >= 6000) // Unity 6 icalls added the _Injected postfix
        {
            LoadFromFile_InternalDelegateField_Unity6 = IL2CPP.ResolveICall<LoadFromFile_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromFile_Internal_Injected(System.String,System.UInt32,System.UInt64)");
            LoadFromFileAsync_InternalDelegateField_Unity6 = IL2CPP.ResolveICall<LoadFromFileAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromFileAsync_Internal_Injected");
            LoadFromMemory_InternalDelegateField_Unity6 = IL2CPP.ResolveICall<LoadFromMemory_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromMemory_Internal_Injected");
            LoadFromMemoryAsync_InternalDelegateField_Unity6 = IL2CPP.ResolveICall<LoadFromMemoryAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromMemoryAsync_Internal_Injected");
            // The parameters of these functions below didn't change, but the signature DID change.
            LoadFromStreamInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamInternal_Injected");
            LoadFromStreamAsyncInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamAsyncInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamAsyncInternal_Injected");   
        }
        else
        {
            LoadFromFile_InternalDelegateField = IL2CPP.ResolveICall<LoadFromFile_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFile_Internal(System.String,System.UInt32,System.UInt64)");
            LoadFromFileAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadFromFileAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFileAsync_Internal");
            LoadFromMemory_InternalDelegateField = IL2CPP.ResolveICall<LoadFromMemory_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemory_Internal");
            LoadFromMemoryAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadFromMemoryAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemoryAsync_Internal");
            LoadFromStreamInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamInternal");
            LoadFromStreamAsyncInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamAsyncInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamAsyncInternal");
            UnloadAllAssetBundlesDelegateField = IL2CPP.ResolveICall<UnloadAllAssetBundlesDelegate>("UnityEngine.AssetBundle::UnloadAllAssetBundles");
        }
    }

    public static Il2CppAssetBundle[] GetAllLoadedAssetBundles()
    {
        if (GetAllLoadedAssetBundles_NativeDelegateField == null)
            throw new System.NullReferenceException("The GetAllLoadedAssetBundles_NativeDelegateField cannot be null.");
        var intPtr = GetAllLoadedAssetBundles_NativeDelegateField();
        var refarr = ((intPtr != System.IntPtr.Zero) ? new Il2CppReferenceArray<Object>(intPtr) : null);
        if (refarr == null)
            throw new System.NullReferenceException("The refarr cannot be null.");
        System.Collections.Generic.List<Il2CppAssetBundle> bundlelist = [];
        for (var i = 0; i < refarr.Length; i++)
            bundlelist.Add(new Il2CppAssetBundle(IL2CPP.Il2CppObjectBaseToPtrNotNull(refarr[i])));
        return bundlelist.ToArray();
    }

    public static Il2CppAssetBundle LoadFromFile(string path) => LoadFromFile(path, 0u, 0UL);

    public static Il2CppAssetBundle LoadFromFile(string path, uint crc) => LoadFromFile(path, crc, 0UL);

    public static Il2CppAssetBundle LoadFromFile(string path, uint crc, ulong offset)
    {
        if (string.IsNullOrEmpty(path))
            throw new System.ArgumentException("The input asset bundle path cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromFile_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromFile_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = path)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = charPtr,
                        length = path.Length
                    };
                    var gcHandle = LoadFromFile_InternalDelegateField_Unity6(ref span, crc, offset);
                    return ((gcHandle != System.IntPtr.Zero) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
                }
            }
        }
        else
        {
            if (LoadFromFile_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromFile_InternalDelegateField cannot be null.");
            var intPtr = LoadFromFile_InternalDelegateField(IL2CPP.ManagedStringToIl2Cpp(path), crc, offset);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
        }
    }

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path) => LoadFromFileAsync(path, 0u, 0UL);

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path, uint crc) => LoadFromFileAsync(path, crc, 0UL);

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path, uint crc, ulong offset)
    {
        if (string.IsNullOrEmpty(path))
            throw new System.ArgumentException("The input asset bundle path cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromFileAsync_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromFileAsync_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = path)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = charPtr,
                        length = path.Length
                    };
                    var intPtr = LoadFromFileAsync_InternalDelegateField_Unity6(ref span, crc, offset);
                    return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
                }
            }
        }
        else
        {
            if (LoadFromFileAsync_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromFileAsync_InternalDelegateField cannot be null.");
            var intPtr = LoadFromFileAsync_InternalDelegateField(IL2CPP.ManagedStringToIl2Cpp(path), crc, offset);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
        }
    }

    public static Il2CppAssetBundle LoadFromMemory(Il2CppStructArray<byte> binary) => LoadFromMemory(binary, 0u);

    public static Il2CppAssetBundle LoadFromMemory(Il2CppStructArray<byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromMemory_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromMemory_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                var arrayPtr = IL2CPP.Il2CppObjectBaseToPtr(binary);
                var span = new ManagedSpanWrapper
                {
                    begin = (void*)(arrayPtr + 0x20), // Skip the IL2CPP object header.
                    length = binary.Length
                };
                var gcHandle = LoadFromMemory_InternalDelegateField_Unity6(ref span, crc);
                return ((gcHandle != System.IntPtr.Zero) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
            }
        }
        else
        {
            if (LoadFromMemory_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromMemory_InternalDelegateField cannot be null.");
            var intPtr = LoadFromMemory_InternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(binary), crc);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
        }
    }

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppStructArray<byte> binary) => LoadFromMemoryAsync(binary, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppStructArray<byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromMemoryAsync_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromMemoryAsync_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                var arrayPtr = IL2CPP.Il2CppObjectBaseToPtr(binary);
                var span = new ManagedSpanWrapper
                {
                    begin = (void*)(arrayPtr + 0x20), // Skip the IL2CPP object header.
                    length = binary.Length
                };
                var intPtr = LoadFromMemoryAsync_InternalDelegateField_Unity6(ref span, crc);
                return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
            }
        }
        else
        {
            if (LoadFromMemoryAsync_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromMemoryAsync_InternalDelegateField cannot be null.");
            var intPtr = LoadFromMemoryAsync_InternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(binary), crc);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
        }
    }

    public static Il2CppAssetBundle LoadFromStream(Stream stream) => LoadFromStream(stream, 0u, 0u);

    public static Il2CppAssetBundle LoadFromStream(Stream stream, uint crc) => LoadFromStream(stream, crc, 0u);

    public static Il2CppAssetBundle LoadFromStream(Stream stream, uint crc, uint managedReadBufferSize)
    {
        if (stream == null)
            throw new System.ArgumentException("The stream cannot be null or empty.");
        if (LoadFromStreamInternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromStreamInternalDelegateField cannot be null.");
        var gcHandle = LoadFromStreamInternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(stream), crc, managedReadBufferSize);
        return ((gcHandle != System.IntPtr.Zero) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
    }

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream) => LoadFromStreamAsync(stream, 0u, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc) => LoadFromStreamAsync(stream, crc, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc, uint managedReadBufferSize)
    {
        if (stream == null)
            throw new System.ArgumentException("The stream cannot be null or empty.");
        if (LoadFromStreamAsyncInternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromStreamAsyncInternalDelegateField cannot be null.");
        var intPtr = LoadFromStreamAsyncInternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(stream), crc, managedReadBufferSize);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
    }

    public static void UnloadAllAssetBundles(bool unloadAllObjects)
    {
        if (UnloadAllAssetBundlesDelegateField == null)
            throw new System.NullReferenceException("The UnloadAllAssetBundlesDelegateField cannot be null.");
        UnloadAllAssetBundlesDelegateField(unloadAllObjects);
    }

    private delegate System.IntPtr GetAllLoadedAssetBundles_NativeDelegate();
    private static readonly GetAllLoadedAssetBundles_NativeDelegate GetAllLoadedAssetBundles_NativeDelegateField;
    private delegate System.IntPtr LoadFromFile_InternalDelegate(System.IntPtr path, uint crc, ulong offset);
    private static readonly LoadFromFile_InternalDelegate LoadFromFile_InternalDelegateField;
    private delegate System.IntPtr LoadFromFileAsync_InternalDelegate(System.IntPtr path, uint crc, ulong offset);
    private static readonly LoadFromFileAsync_InternalDelegate LoadFromFileAsync_InternalDelegateField;
    private delegate System.IntPtr LoadFromMemory_InternalDelegate(System.IntPtr binary, uint crc);
    private static readonly LoadFromMemory_InternalDelegate LoadFromMemory_InternalDelegateField;
    private delegate System.IntPtr LoadFromMemoryAsync_InternalDelegate(System.IntPtr binary, uint crc);
    private static readonly LoadFromMemoryAsync_InternalDelegate LoadFromMemoryAsync_InternalDelegateField;
    private delegate System.IntPtr LoadFromStreamInternalDelegate(System.IntPtr stream, uint crc, uint managedReadBufferSize);
    private static readonly LoadFromStreamInternalDelegate LoadFromStreamInternalDelegateField;
    private delegate System.IntPtr LoadFromStreamAsyncInternalDelegate(System.IntPtr stream, uint crc, uint managedReadBufferSize);
    private static readonly LoadFromStreamAsyncInternalDelegate LoadFromStreamAsyncInternalDelegateField;
    private delegate System.IntPtr UnloadAllAssetBundlesDelegate(bool unloadAllObjects);
    private static readonly UnloadAllAssetBundlesDelegate UnloadAllAssetBundlesDelegateField;

    // Unity 6 new signatures.
    // GetAllLoadedAssetBundles doesn't change.
    private delegate System.IntPtr LoadFromFile_InternalDelegate_Unity6(ref ManagedSpanWrapper path, uint crc, ulong offset);
    private static readonly LoadFromFile_InternalDelegate_Unity6 LoadFromFile_InternalDelegateField_Unity6;
    private delegate System.IntPtr LoadFromFileAsync_InternalDelegate_Unity6(ref ManagedSpanWrapper path, uint crc, ulong offset);
    private static readonly LoadFromFileAsync_InternalDelegate_Unity6 LoadFromFileAsync_InternalDelegateField_Unity6;
    private delegate System.IntPtr LoadFromMemory_InternalDelegate_Unity6(ref ManagedSpanWrapper binary, uint crc);
    private static readonly LoadFromMemory_InternalDelegate_Unity6 LoadFromMemory_InternalDelegateField_Unity6;
    private delegate System.IntPtr LoadFromMemoryAsync_InternalDelegate_Unity6(ref ManagedSpanWrapper binary, uint crc);
    private static readonly LoadFromMemoryAsync_InternalDelegate_Unity6 LoadFromMemoryAsync_InternalDelegateField_Unity6;
    // LoadFromStream doesn't change.
    // LoadFromStreamAsync doesn't change.
    // UnloadAllAssetBundles doesn't change.
}
