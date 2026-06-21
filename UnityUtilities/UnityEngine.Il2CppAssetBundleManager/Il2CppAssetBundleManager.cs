using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Structs;
using Il2CppSystem.IO;
using MelonLoader.InternalUtils;
using System;

namespace UnityEngine;

public class Il2CppAssetBundleManager
{
    static Il2CppAssetBundleManager()
    {
        // icalls whose signature hasn't changed.
        GetAllLoadedAssetBundles_NativeDelegateField = RuntimeInvoke.ResolveICall<GetAllLoadedAssetBundles_NativeDelegate>("UnityEngine.AssetBundle::GetAllLoadedAssetBundles_Native");
        UnloadAllAssetBundlesDelegateField = RuntimeInvoke.ResolveICall<UnloadAllAssetBundlesDelegate>("UnityEngine.AssetBundle::UnloadAllAssetBundles");

        if (UnityInformationHandler.EngineVersion.Major >= 6000) // Unity 6 icalls added the _Injected postfix
        {
            LoadFromFile_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadFromFile_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromFile_Internal_Injected(System.String,System.UInt32,System.UInt64)");
            LoadFromFileAsync_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadFromFileAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromFileAsync_Internal_Injected");
            LoadFromMemory_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadFromMemory_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromMemory_Internal_Injected");
            LoadFromMemoryAsync_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadFromMemoryAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadFromMemoryAsync_Internal_Injected");
            // The parameters of these functions below didn't change, but the signature DID change.
            LoadFromStreamInternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromStreamInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamInternal_Injected");
            LoadFromStreamAsyncInternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromStreamAsyncInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamAsyncInternal_Injected");   
        }
        else
        {
            LoadFromFile_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromFile_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFile_Internal(System.String,System.UInt32,System.UInt64)");
            LoadFromFileAsync_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromFileAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromFileAsync_Internal");
            LoadFromMemory_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromMemory_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemory_Internal");
            LoadFromMemoryAsync_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromMemoryAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadFromMemoryAsync_Internal");
            LoadFromStreamInternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromStreamInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamInternal");
            LoadFromStreamAsyncInternalDelegateField = RuntimeInvoke.ResolveICall<LoadFromStreamAsyncInternalDelegate>("UnityEngine.AssetBundle::LoadFromStreamAsyncInternal");
            UnloadAllAssetBundlesDelegateField = RuntimeInvoke.ResolveICall<UnloadAllAssetBundlesDelegate>("UnityEngine.AssetBundle::UnloadAllAssetBundles");
        }
    }

    public static unsafe Il2CppAssetBundle[] GetAllLoadedAssetBundles()
    {
        var arrayStartOffset = sizeof(Il2CppObject) /* base */ + sizeof(void*) /* bounds */ + sizeof(nuint) /* max_length */;

        if (GetAllLoadedAssetBundles_NativeDelegateField == null)
            throw new System.NullReferenceException("The GetAllLoadedAssetBundles_NativeDelegateField cannot be null.");
        IntPtr intPtr = GetAllLoadedAssetBundles_NativeDelegateField();
        if (intPtr == default)
            throw new System.NullReferenceException("The intPtr cannot be null.");

        uint length = IL2CPP.il2cpp_array_length(intPtr);
        if (length == 0)
            return [];

        var span = new ReadOnlySpan<IntPtr>((void*)(intPtr + arrayStartOffset), (int)length);
        var result = new Il2CppAssetBundle[span.Length];
        for (var i = 0; i < span.Length; i++)
            result[i] = new Il2CppAssetBundle(span[i]);

        return result;
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
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = path.Length
                    };
                    var gcHandle = LoadFromFile_InternalDelegateField_Unity6((ByReference<ManagedSpanWrapper>)(&span), crc, offset);
                    return ((gcHandle != System.IntPtr.Zero) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
                }
            }
        }
        else
        {
            if (LoadFromFile_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromFile_InternalDelegateField cannot be null.");
            var intPtr = LoadFromFile_InternalDelegateField(path, crc, offset);
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
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = path.Length
                    };
                    var intPtr = LoadFromFileAsync_InternalDelegateField_Unity6((ByReference<ManagedSpanWrapper>)(&span), crc, offset);
                    return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
                }
            }
        }
        else
        {
            if (LoadFromFileAsync_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromFileAsync_InternalDelegateField cannot be null.");
            var intPtr = LoadFromFileAsync_InternalDelegateField(path, crc, offset);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
        }
    }

    public static Il2CppAssetBundle LoadFromMemory(Il2CppArrayRank1<Il2CppSystem.Byte> binary) => LoadFromMemory(binary, 0u);

    public static Il2CppAssetBundle LoadFromMemory(Il2CppArrayRank1<Il2CppSystem.Byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromMemory_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromMemory_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                var arrayPtr = binary.Pointer;
                var span = new ManagedSpanWrapper
                {
                    begin = (Pointer<Il2CppSystem.Void>)(void*)binary.GetElementAddress(0),
                    length = binary.Length
                };
                var gcHandle = LoadFromMemory_InternalDelegateField_Unity6((ByReference<ManagedSpanWrapper>)(&span), crc);
                return ((gcHandle != System.IntPtr.Zero) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
            }
        }
        else
        {
            if (LoadFromMemory_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromMemory_InternalDelegateField cannot be null.");
            var intPtr = LoadFromMemory_InternalDelegateField(binary, crc);
            return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundle(intPtr) : null);
        }
    }

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppArrayRank1<Il2CppSystem.Byte> binary) => LoadFromMemoryAsync(binary, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromMemoryAsync(Il2CppArrayRank1<Il2CppSystem.Byte> binary, uint crc)
    {
        if (binary == null)
            throw new System.ArgumentException("The binary cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadFromMemoryAsync_InternalDelegateField_Unity6 == null)
                throw new System.NullReferenceException("The LoadFromMemoryAsync_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                var span = new ManagedSpanWrapper
                {
                    begin = (Pointer<Il2CppSystem.Void>)(void*)binary.GetElementAddress(0),
                    length = binary.Length
                };
                var intPtr = LoadFromMemoryAsync_InternalDelegateField_Unity6((ByReference<ManagedSpanWrapper>)(&span), crc);
                return ((intPtr != System.IntPtr.Zero) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
            }
        }
        else
        {
            if (LoadFromMemoryAsync_InternalDelegateField == null)
                throw new System.NullReferenceException("The LoadFromMemoryAsync_InternalDelegateField cannot be null.");
            var intPtr = LoadFromMemoryAsync_InternalDelegateField(binary, crc);
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
        var gcHandle = LoadFromStreamInternalDelegateField(stream, crc, managedReadBufferSize);
        return ((gcHandle != default) ? new Il2CppAssetBundle(IL2CPP.il2cpp_gchandle_get_target(gcHandle)) : null);
    }

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream) => LoadFromStreamAsync(stream, 0u, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc) => LoadFromStreamAsync(stream, crc, 0u);

    public static Il2CppAssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc, uint managedReadBufferSize)
    {
        if (stream == null)
            throw new System.ArgumentException("The stream cannot be null or empty.");
        if (LoadFromStreamAsyncInternalDelegateField == null)
            throw new System.NullReferenceException("The LoadFromStreamAsyncInternalDelegateField cannot be null.");
        var intPtr = LoadFromStreamAsyncInternalDelegateField(stream, crc, managedReadBufferSize);
        return ((intPtr != default) ? new Il2CppAssetBundleCreateRequest(intPtr) : null);
    }

    public static void UnloadAllAssetBundles(bool unloadAllObjects)
    {
        if (UnloadAllAssetBundlesDelegateField == null)
            throw new System.NullReferenceException("The UnloadAllAssetBundlesDelegateField cannot be null.");
        UnloadAllAssetBundlesDelegateField(unloadAllObjects);
    }

    private delegate Il2CppSystem.IntPtr GetAllLoadedAssetBundles_NativeDelegate();
    private static readonly GetAllLoadedAssetBundles_NativeDelegate GetAllLoadedAssetBundles_NativeDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromFile_InternalDelegate(Il2CppSystem.String path, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt64 offset);
    private static readonly LoadFromFile_InternalDelegate LoadFromFile_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromFileAsync_InternalDelegate(Il2CppSystem.String path, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt64 offset);
    private static readonly LoadFromFileAsync_InternalDelegate LoadFromFileAsync_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromMemory_InternalDelegate(Il2CppArrayRank1<Il2CppSystem.Byte> binary, Il2CppSystem.UInt32 crc);
    private static readonly LoadFromMemory_InternalDelegate LoadFromMemory_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromMemoryAsync_InternalDelegate(Il2CppArrayRank1<Il2CppSystem.Byte> binary, Il2CppSystem.UInt32 crc);
    private static readonly LoadFromMemoryAsync_InternalDelegate LoadFromMemoryAsync_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromStreamInternalDelegate(Stream stream, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt32 managedReadBufferSize);
    private static readonly LoadFromStreamInternalDelegate LoadFromStreamInternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadFromStreamAsyncInternalDelegate(Stream stream, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt32 managedReadBufferSize);
    private static readonly LoadFromStreamAsyncInternalDelegate LoadFromStreamAsyncInternalDelegateField;
    private delegate Il2CppSystem.IntPtr UnloadAllAssetBundlesDelegate(Il2CppSystem.Boolean unloadAllObjects);
    private static readonly UnloadAllAssetBundlesDelegate UnloadAllAssetBundlesDelegateField;

    // Unity 6 new signatures.
    // GetAllLoadedAssetBundles doesn't change.
    private delegate Il2CppSystem.IntPtr LoadFromFile_InternalDelegate_Unity6(ByReference<ManagedSpanWrapper> path, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt64 offset);
    private static readonly LoadFromFile_InternalDelegate_Unity6 LoadFromFile_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadFromFileAsync_InternalDelegate_Unity6(ByReference<ManagedSpanWrapper> path, Il2CppSystem.UInt32 crc, Il2CppSystem.UInt64 offset);
    private static readonly LoadFromFileAsync_InternalDelegate_Unity6 LoadFromFileAsync_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadFromMemory_InternalDelegate_Unity6(ByReference<ManagedSpanWrapper> binary, Il2CppSystem.UInt32 crc);
    private static readonly LoadFromMemory_InternalDelegate_Unity6 LoadFromMemory_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadFromMemoryAsync_InternalDelegate_Unity6(ByReference<ManagedSpanWrapper> binary, Il2CppSystem.UInt32 crc);
    private static readonly LoadFromMemoryAsync_InternalDelegate_Unity6 LoadFromMemoryAsync_InternalDelegateField_Unity6;
    // LoadFromStream doesn't change.
    // LoadFromStreamAsync doesn't change.
    // UnloadAllAssetBundles doesn't change.
}
