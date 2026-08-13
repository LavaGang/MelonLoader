using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using MelonLoader.InternalUtils;
using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

public class Il2CppAssetBundle
{
    // Since here we're not using il2cpp_runtime_invoke, but we're resolving the icalls directly, we need to unwrap the object by ourselves.
    private readonly IntPtr wrappedBundlePtr = IntPtr.Zero;
    private readonly IntPtr bundleptr = IntPtr.Zero;

    public Il2CppAssetBundle(IntPtr ptr)
    {
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            wrappedBundlePtr = ptr;
            bundleptr = Marshal.ReadIntPtr(wrappedBundlePtr + 0x10); // Skip the Il2CppObject header + read the REAL object ptr from there.
        }
        else
        {
            bundleptr = ptr;
        }
    }

    static Il2CppAssetBundle()
    {
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            get_isStreamedSceneAssetBundleDelegateField = RuntimeInvoke.ResolveICall<get_isStreamedSceneAssetBundleDelegate>("UnityEngine.AssetBundle::get_isStreamedSceneAssetBundle_Injected");
            returnMainAssetDelegateField = RuntimeInvoke.ResolveICall<returnMainAssetDelegate>("UnityEngine.AssetBundle::returnMainAsset_Injected");
            ContainsDelegateField_Unity6 = RuntimeInvoke.ResolveICall<ContainsDelegate_Unity6>("UnityEngine.AssetBundle::Contains_Injected");
            GetAllAssetNamesDelegateField = RuntimeInvoke.ResolveICall<GetAllAssetNamesDelegate>("UnityEngine.AssetBundle::GetAllAssetNames_Injected");
            GetAllScenePathsDelegateField = RuntimeInvoke.ResolveICall<GetAllScenePathsDelegate>("UnityEngine.AssetBundle::GetAllScenePaths_Injected");
            LoadAsset_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadAsset_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadAsset_Internal_Injected(System.String,System.Type)");
            LoadAssetAsync_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadAssetAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadAssetAsync_Internal_Injected");
            LoadAssetWithSubAssets_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadAssetWithSubAssets_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal_Injected");
            LoadAssetWithSubAssetsAsync_InternalDelegateField_Unity6 = RuntimeInvoke.ResolveICall<LoadAssetWithSubAssetsAsync_InternalDelegate_Unity6>("UnityEngine.AssetBundle::LoadAssetWithSubAssetsAsync_Internal_Injected");
            UnloadDelegateField = RuntimeInvoke.ResolveICall<UnloadDelegate>("UnityEngine.AssetBundle::Unload_Injected");
        }
        else
        {
            get_isStreamedSceneAssetBundleDelegateField = RuntimeInvoke.ResolveICall<get_isStreamedSceneAssetBundleDelegate>("UnityEngine.AssetBundle::get_isStreamedSceneAssetBundle");
            returnMainAssetDelegateField = RuntimeInvoke.ResolveICall<returnMainAssetDelegate>("UnityEngine.AssetBundle::returnMainAsset");
            ContainsDelegateField = RuntimeInvoke.ResolveICall<ContainsDelegate>("UnityEngine.AssetBundle::Contains");
            GetAllAssetNamesDelegateField = RuntimeInvoke.ResolveICall<GetAllAssetNamesDelegate>("UnityEngine.AssetBundle::GetAllAssetNames");
            GetAllScenePathsDelegateField = RuntimeInvoke.ResolveICall<GetAllScenePathsDelegate>("UnityEngine.AssetBundle::GetAllScenePaths");
            LoadAsset_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadAsset_InternalDelegate>("UnityEngine.AssetBundle::LoadAsset_Internal(System.String,System.Type)");
            LoadAssetAsync_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadAssetAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetAsync_Internal");
            LoadAssetWithSubAssets_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadAssetWithSubAssets_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal");
            LoadAssetWithSubAssetsAsync_InternalDelegateField = RuntimeInvoke.ResolveICall<LoadAssetWithSubAssetsAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetWithSubAssetsAsync_Internal");
            UnloadDelegateField = RuntimeInvoke.ResolveICall<UnloadDelegate>("UnityEngine.AssetBundle::Unload");
        }
    }

    public bool isStreamedSceneAssetBundle
    {
        get
        {
            if (bundleptr == IntPtr.Zero)
                throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
            if (get_isStreamedSceneAssetBundleDelegateField == null)
                throw new NullReferenceException("The get_isStreamedSceneAssetBundleDelegateField cannot be null.");
            return get_isStreamedSceneAssetBundleDelegateField(bundleptr);
        }
    }

    public Object mainAsset
    {
        get
        {
            if (bundleptr == IntPtr.Zero)
                throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
            if (returnMainAssetDelegateField == null)
                throw new NullReferenceException("The returnMainAssetDelegateField cannot be null.");
            if (UnityInformationHandler.EngineVersion.Major >= 6000)
            {
                // Signature doesn't change, but the resulting ptr is actually a gchandle in Unity 6.
                var gcHandle = returnMainAssetDelegateField(bundleptr);
                return ((gcHandle != IntPtr.Zero) ? Il2CppObjectPool.Get(Marshal.ReadIntPtr(gcHandle)) as Object : null);
            }
            else
            {
                var intPtr = returnMainAssetDelegateField(bundleptr);
                return Il2CppObjectPool.Get(intPtr) as Object;
            }
        }
    }

    public bool Contains(string name)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (ContainsDelegateField_Unity6 == null)
                throw new NullReferenceException("The ContainsDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = name)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = name.Length
                    };
                    return ContainsDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span));
                }
            }
        }
        else
        {
            if (ContainsDelegateField == null)
                throw new NullReferenceException("The ContainsDelegateField cannot be null.");
            return ContainsDelegateField(bundleptr, name);
        }
    }

    public Il2CppArrayRank1<Il2CppSystem.String> AllAssetNames() => GetAllAssetNames();

    public Il2CppArrayRank1<Il2CppSystem.String> GetAllAssetNames()
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (GetAllAssetNamesDelegateField == null)
            throw new NullReferenceException("The GetAllAssetNamesDelegateField cannot be null.");
        var intPtr = GetAllAssetNamesDelegateField(bundleptr);
        return Il2CppObjectPool.Get(intPtr) as Il2CppArrayRank1<Il2CppSystem.String>;
    }

    public Il2CppArrayRank1<Il2CppSystem.String> AllScenePaths() => GetAllScenePaths();

    public Il2CppArrayRank1<Il2CppSystem.String> GetAllScenePaths()
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (GetAllScenePathsDelegateField == null)
            throw new NullReferenceException("The GetAllScenePathsDelegateField cannot be null.");
        var intPtr = GetAllScenePathsDelegateField(bundleptr);
        return Il2CppObjectPool.Get(intPtr) as Il2CppArrayRank1<Il2CppSystem.String>;
    }

    public Object Load(string name) => LoadAsset(name);

    public Object LoadAsset(string name) => LoadAsset<Object>(name);

    public T Load<T>(string name) where T : Object => LoadAsset<T>(name);

    public T LoadAsset<T>(string name) where T : Object
    {
        if (!InteropSupport.IsGeneratedAssemblyType(typeof(T)))
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAsset(name, ManagedTypeToIl2CppType(typeof(T)).Pointer);
        return (T)Il2CppObjectPool.Get(intptr);
    }

    public Object Load(string name, Il2CppSystem.Type type) => LoadAsset(name, type);

    public Object LoadAsset(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAsset(name, type.Pointer);
        return Il2CppObjectPool.Get(intptr) as Object;
    }

    public IntPtr Load(string name, IntPtr typeptr) => LoadAsset(name, typeptr);

    public IntPtr LoadAsset(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadAsset_InternalDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadAsset_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = name)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = name.Length
                    };
                    var gcHandle = LoadAsset_InternalDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span), typeptr);
                    return ((gcHandle != IntPtr.Zero) ? Marshal.ReadIntPtr(gcHandle) : IntPtr.Zero);
                }
            }
        }
        else
        {
            if (LoadAsset_InternalDelegateField == null)
                throw new NullReferenceException("The LoadAsset_InternalDelegateField cannot be null.");
            return LoadAsset_InternalDelegateField(bundleptr, name, typeptr);
        }
    }

    public Il2CppAssetBundleRequest LoadAssetAsync(string name) => LoadAssetAsync<Object>(name);

    public Il2CppAssetBundleRequest LoadAssetAsync<T>(string name) where T : Object
    {
        if (!InteropSupport.IsGeneratedAssemblyType(typeof(T)))
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetAsync(name, ManagedTypeToIl2CppType(typeof(T)).Pointer);
        return (intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null;
    }

    public Il2CppAssetBundleRequest LoadAssetAsync(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAssetAsync(name, type.Pointer);
        return (intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null;
    }

    public IntPtr LoadAssetAsync(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadAssetAsync_InternalDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadAssetAsync_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = name)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = name.Length
                    };
                    return LoadAssetAsync_InternalDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span), typeptr);
                }
            }
        }
        else
        {
            if (LoadAssetAsync_InternalDelegateField == null)
                throw new NullReferenceException("The LoadAssetAsync_InternalDelegateField cannot be null.");
            return LoadAssetAsync_InternalDelegateField(bundleptr, name, typeptr);
        }
    }

    public Il2CppArrayRank1<Object> LoadAll() => LoadAllAssets();

    public Il2CppArrayRank1<Object> LoadAllAssets() => LoadAllAssets<Object>();

    public Il2CppArrayRank1<T> LoadAll<T>() where T : Object, IIl2CppType<T> => LoadAllAssets<T>();

    public Il2CppArrayRank1<T> LoadAllAssets<T>() where T : Object, IIl2CppType<T>
    {
        if (!InteropSupport.IsGeneratedAssemblyType(typeof(T)))
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAllAssets(ManagedTypeToIl2CppType(typeof(T)).Pointer);
        return Il2CppObjectPool.Get(intptr) as Il2CppArrayRank1<T>;
    }

    public Il2CppArrayRank1<Object> LoadAll(Il2CppSystem.Type type) => LoadAllAssets(type);

    public Il2CppArrayRank1<Object> LoadAllAssets(Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAllAssets(type.Pointer);
        return Il2CppObjectPool.Get(intptr) as Il2CppArrayRank1<Object>;
    }

    public IntPtr LoadAll(IntPtr typeptr) => LoadAllAssets(typeptr);

    public IntPtr LoadAllAssets(IntPtr typeptr)
    {
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadAssetWithSubAssets_InternalDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = string.Empty)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = 0
                    };
                    return LoadAssetWithSubAssets_InternalDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span), typeptr);
                }
            }
        }
        else
        {
            if (LoadAssetWithSubAssets_InternalDelegateField == null)
                throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField cannot be null.");
            return LoadAssetWithSubAssets_InternalDelegateField(bundleptr, string.Empty, typeptr);
        }
    }

    public Il2CppArrayRank1<Object> LoadAssetWithSubAssets(string name) => LoadAssetWithSubAssets<Object>(name);

    public Il2CppArrayRank1<T> LoadAssetWithSubAssets<T>(string name) where T : Object, IIl2CppType<T>
    {
        if (!InteropSupport.IsGeneratedAssemblyType(typeof(T)))
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetWithSubAssets(name, ManagedTypeToIl2CppType(typeof(T)).Pointer);
        return Il2CppObjectPool.Get(intptr) as Il2CppArrayRank1<T>;
    }

    public Il2CppArrayRank1<Object> LoadAssetWithSubAssets(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAssetWithSubAssets(name, type.Pointer);
        return Il2CppObjectPool.Get(intptr) as Il2CppArrayRank1<Object>;
    }

    public IntPtr LoadAssetWithSubAssets(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadAssetWithSubAssets_InternalDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = name)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = name.Length
                    };
                    return LoadAssetWithSubAssets_InternalDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span), typeptr);
                }
            }
        }
        else
        {
            if (LoadAssetWithSubAssets_InternalDelegateField == null)
                throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField cannot be null.");
            return LoadAssetWithSubAssets_InternalDelegateField(bundleptr, name, typeptr);
        }
    }
    public Il2CppAssetBundleRequest LoadAssetWithSubAssetsAsync(string name) => LoadAssetWithSubAssetsAsync<Object>(name);

    public Il2CppAssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name) where T : Object
    {
        if (!InteropSupport.IsGeneratedAssemblyType(typeof(T)))
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetWithSubAssetsAsync(name, ManagedTypeToIl2CppType(typeof(T)).Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null);
    }

    public Il2CppAssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAssetWithSubAssetsAsync(name, type.Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null);
    }

    public IntPtr LoadAssetWithSubAssetsAsync(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (UnityInformationHandler.EngineVersion.Major >= 6000)
        {
            if (LoadAssetWithSubAssetsAsync_InternalDelegateField_Unity6 == null)
                throw new NullReferenceException("The LoadAssetWithSubAssetsAsync_InternalDelegateField_Unity6 cannot be null.");
            unsafe
            {
                fixed (char* charPtr = name)
                {
                    var span = new ManagedSpanWrapper
                    {
                        begin = (Pointer<Il2CppSystem.Void>)charPtr,
                        length = name.Length
                    };
                    return LoadAssetWithSubAssetsAsync_InternalDelegateField_Unity6(bundleptr, (ByReference<ManagedSpanWrapper>)(&span), typeptr);
                }
            }
        }
        else
        {
            if (LoadAssetWithSubAssetsAsync_InternalDelegateField == null)
                throw new NullReferenceException("The LoadAssetWithSubAssetsAsync_InternalDelegateField cannot be null.");
            return LoadAssetWithSubAssetsAsync_InternalDelegateField(bundleptr, name, typeptr);
        }
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (UnloadDelegateField == null)
            throw new NullReferenceException("The UnloadDelegateField cannot be null.");
        UnloadDelegateField(bundleptr, unloadAllLoadedObjects);
    }

    private static Il2CppSystem.Type ManagedTypeToIl2CppType(Type type)
    {
        return GenerationInternals.ManagedTypeToIl2CppType(type);
    }

    private delegate bool get_isStreamedSceneAssetBundleDelegate(Il2CppSystem.IntPtr _this);
    private static readonly get_isStreamedSceneAssetBundleDelegate get_isStreamedSceneAssetBundleDelegateField;
    private delegate Il2CppSystem.IntPtr returnMainAssetDelegate(Il2CppSystem.IntPtr _this);
    private static readonly returnMainAssetDelegate returnMainAssetDelegateField;
    private delegate Il2CppSystem.Boolean ContainsDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.String name);
    private static readonly ContainsDelegate ContainsDelegateField;
    private delegate Il2CppSystem.IntPtr GetAllAssetNamesDelegate(Il2CppSystem.IntPtr _this);
    private static readonly GetAllAssetNamesDelegate GetAllAssetNamesDelegateField;
    private delegate Il2CppSystem.IntPtr GetAllScenePathsDelegate(Il2CppSystem.IntPtr _this);
    private static readonly GetAllScenePathsDelegate GetAllScenePathsDelegateField;
    private delegate Il2CppSystem.IntPtr LoadAsset_InternalDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.String name, Il2CppSystem.IntPtr type);
    private static readonly LoadAsset_InternalDelegate LoadAsset_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadAssetAsync_InternalDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.String name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetAsync_InternalDelegate LoadAssetAsync_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadAssetWithSubAssets_InternalDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.String name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetWithSubAssets_InternalDelegate LoadAssetWithSubAssets_InternalDelegateField;
    private delegate Il2CppSystem.IntPtr LoadAssetWithSubAssetsAsync_InternalDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.String name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetWithSubAssetsAsync_InternalDelegate LoadAssetWithSubAssetsAsync_InternalDelegateField;
    private delegate void UnloadDelegate(Il2CppSystem.IntPtr _this, Il2CppSystem.Boolean unloadAllObjects);
    private static readonly UnloadDelegate UnloadDelegateField;

    // get_isStreamedSceneAssetBundle doesn't change.
    // returnMainAsset doesn't change.
    private delegate Il2CppSystem.Boolean ContainsDelegate_Unity6(Il2CppSystem.IntPtr _this, ByReference<ManagedSpanWrapper> name);
    private static readonly ContainsDelegate_Unity6 ContainsDelegateField_Unity6;
    // GetAllAssetNames doesn't change.
    // GetAllScenePaths doesn't change.
    private delegate Il2CppSystem.IntPtr LoadAsset_InternalDelegate_Unity6(Il2CppSystem.IntPtr _this, ByReference<ManagedSpanWrapper> name, Il2CppSystem.IntPtr type);
    private static readonly LoadAsset_InternalDelegate_Unity6 LoadAsset_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadAssetAsync_InternalDelegate_Unity6(Il2CppSystem.IntPtr _this, ByReference<ManagedSpanWrapper> name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetAsync_InternalDelegate_Unity6 LoadAssetAsync_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadAssetWithSubAssets_InternalDelegate_Unity6(Il2CppSystem.IntPtr _this, ByReference<ManagedSpanWrapper> name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetWithSubAssets_InternalDelegate_Unity6 LoadAssetWithSubAssets_InternalDelegateField_Unity6;
    private delegate Il2CppSystem.IntPtr LoadAssetWithSubAssetsAsync_InternalDelegate_Unity6(Il2CppSystem.IntPtr _this, ByReference<ManagedSpanWrapper> name, Il2CppSystem.IntPtr type);
    private static readonly LoadAssetWithSubAssetsAsync_InternalDelegate_Unity6 LoadAssetWithSubAssetsAsync_InternalDelegateField_Unity6;
    // Unload doesn't change.
}
