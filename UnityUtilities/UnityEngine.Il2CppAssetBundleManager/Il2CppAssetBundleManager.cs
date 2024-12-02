using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;

namespace UnityEngine;

public class Il2CppAssetBundleManager
{
    static Il2CppAssetBundleManager()
    {
        GetAllLoadedAssetBundles_NativeDelegateField = IL2CPP.ResolveICall<GetAllLoadedAssetBundles_NativeDelegate>("UnityEngine.AssetBundle::GetAllLoadedAssetBundles_Native");
        LoadFromFile_InternalDelegateField = IL2CPP.ResolveICall<LoadFromFile_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFile_Internal(System.String,System.UInt32,System.UInt64)");
        LoadFromFileAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadFromFileAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFileAsync_Internal");
        LoadFromMemory_InternalDelegateField = IL2CPP.ResolveICall<LoadFromMemory_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemory_Internal");
        LoadFromMemoryAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadFromMemoryAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemoryAsync_Internal");
        LoadFromStreamInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamInternal");
        LoadFromStreamAsyncInternalDelegateField = IL2CPP.ResolveICall<LoadFromStreamAsyncInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamAsyncInternal");
        UnloadAllAssetBundlesDelegateField = IL2CPP.ResolveICall<UnloadAllAssetBundlesDelegate>("UnityEngine.AssetBundle::UnloadAllAssetBundles");
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
        if (LoadFromFile_InternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromFile_InternalDelegateField cannot be null.");
        var intPtr = LoadFromFile_InternalDelegateField(IL2CPP.ManagedStringToIl2Cpp(path), crc, offset);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
    }

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path) => LoadFromFileAsync(path, 0u, 0UL);

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path, uint crc) => LoadFromFileAsync(path, crc, 0UL);

    public static Il2CppAssetBundleCreateRequest LoadFromFileAsync(string path, uint crc, ulong offset)
    {
        if (string.IsNullOrEmpty(path))
            throw new System.ArgumentException("The input asset bundle path cannot be null or empty.");
        if (LoadFromFileAsync_InternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromFileAsync_InternalDelegateField cannot be null.");
        var intPtr = LoadFromFileAsync_InternalDelegateField(IL2CPP.ManagedStringToIl2Cpp(path), crc, offset);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
    }

    public static Il2CppAssetBundle LoadFromMemory(Il2CppStructArray<byte> binary) => LoadFromMemory(binary, 0u);

    public static Il2CppAssetBundle LoadFromMemory(Il2CppStructArray<byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (LoadFromMemory_InternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromMemory_InternalDelegateField cannot be null.");
        var intPtr = LoadFromMemory_InternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(binary), crc);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
    }

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppStructArray<byte> binary) => LoadFromMemoryAsync(binary, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppStructArray<byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (LoadFromMemoryAsync_InternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromMemoryAsync_InternalDelegateField cannot be null.");
        var intPtr = LoadFromMemoryAsync_InternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(binary), crc);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
    }

    public static Il2CppAssetBundle LoadFromStream(Stream stream) => LoadFromStream(stream, 0u, 0u);

    public static Il2CppAssetBundle LoadFromStream(Stream stream, uint crc) => LoadFromStream(stream, crc, 0u);

    public static Il2CppAssetBundle LoadFromStream(Stream stream, uint crc, uint managedReadBufferSize)
    {
        if (stream == null)
            throw new System.ArgumentException("The stream cannot be null or empty.");
        if (LoadFromStreamInternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromStreamInternalDelegateField cannot be null.");
        var intPtr = LoadFromStreamInternalDelegateField(IL2CPP.Il2CppObjectBaseToPtrNotNull(stream), crc, managedReadBufferSize);
        return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
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
}
