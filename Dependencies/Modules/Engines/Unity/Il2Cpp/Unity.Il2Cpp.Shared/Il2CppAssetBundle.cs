using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using MelonLoader.Engine.Unity.Il2Cpp;

namespace UnityEngine;

public class Il2CppAssetBundle
{
    private readonly IntPtr bundleptr = IntPtr.Zero;

    public Il2CppAssetBundle(IntPtr ptr) { bundleptr = ptr; }

    static Il2CppAssetBundle()
    {
        get_isStreamedSceneAssetBundleDelegateField = IL2CPP.ResolveICall<get_isStreamedSceneAssetBundleDelegate>("UnityEngine.AssetBundle::get_isStreamedSceneAssetBundle");
        returnMainAssetDelegateField = IL2CPP.ResolveICall<returnMainAssetDelegate>("UnityEngine.AssetBundle::returnMainAsset");
        ContainsDelegateField = IL2CPP.ResolveICall<ContainsDelegate>("UnityEngine.AssetBundle::Contains");
        GetAllAssetNamesDelegateField = IL2CPP.ResolveICall<GetAllAssetNamesDelegate>("UnityEngine.AssetBundle::GetAllAssetNames");
        GetAllScenePathsDelegateField = IL2CPP.ResolveICall<GetAllScenePathsDelegate>("UnityEngine.AssetBundle::GetAllScenePaths");
        LoadAsset_InternalDelegateField = IL2CPP.ResolveICall<LoadAsset_InternalDelegate>("UnityEngine.AssetBundle::LoadAsset_Internal(System.String,System.Type)");
        LoadAssetAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadAssetAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetAsync_Internal");
        LoadAssetWithSubAssets_InternalDelegateField = IL2CPP.ResolveICall<LoadAssetWithSubAssets_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal");
        LoadAssetWithSubAssetsAsync_InternalDelegateField = IL2CPP.ResolveICall<LoadAssetWithSubAssetsAsync_InternalDelegate>("UnityEngine.AssetBundle::LoadAssetWithSubAssetsAsync_Internal");
        UnloadDelegateField = IL2CPP.ResolveICall<UnloadDelegate>("UnityEngine.AssetBundle::Unload");
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
            var intPtr = returnMainAssetDelegateField(bundleptr);
            return ((intPtr != IntPtr.Zero) ? new Object(intPtr) : null);
        }
    }

    public bool Contains(string name)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (ContainsDelegateField == null)
            throw new NullReferenceException("The ContainsDelegateField cannot be null.");
        return ContainsDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(name));
    }

    public Il2CppStringArray AllAssetNames() => GetAllAssetNames();

    public Il2CppStringArray GetAllAssetNames()
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (GetAllAssetNamesDelegateField == null)
            throw new NullReferenceException("The GetAllAssetNamesDelegateField cannot be null.");
        var intPtr = GetAllAssetNamesDelegateField(bundleptr);
        return ((intPtr != IntPtr.Zero) ? new Il2CppStringArray(intPtr) : null);
    }

    public Il2CppStringArray AllScenePaths() => GetAllScenePaths();

    public Il2CppStringArray GetAllScenePaths()
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (GetAllScenePathsDelegateField == null)
            throw new NullReferenceException("The GetAllScenePathsDelegateField cannot be null.");
        var intPtr = GetAllScenePathsDelegateField(bundleptr);
        return ((intPtr != IntPtr.Zero) ? new Il2CppStringArray(intPtr) : null);
    }

    public Object Load(string name) => LoadAsset(name);

    public Object LoadAsset(string name) => LoadAsset<Object>(name);

    public T Load<T>(string name) where T : Object => LoadAsset<T>(name);

    public T LoadAsset<T>(string name) where T : Object
    {
        if (!typeof(T).IsGeneratedAssemblyType())
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAsset(name, Il2CppType.Of<T>().Pointer);
        return ((intptr != IntPtr.Zero) ? intptr.Il2CppObjectPtrToIl2CppObject<T>() : null);
    }

    public Object Load(string name, Il2CppSystem.Type type) => LoadAsset(name, type);

    public Object LoadAsset(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAsset(name, type.Pointer);
        return ((intptr != IntPtr.Zero) ? new Object(intptr) : null);
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
        if (LoadAsset_InternalDelegateField == null)
            throw new NullReferenceException("The LoadAsset_InternalDelegateField cannot be null.");
        return LoadAsset_InternalDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(name), typeptr);
    }

    public Il2CppAssetBundleRequest LoadAssetAsync(string name) => LoadAssetAsync<Object>(name);

    public Il2CppAssetBundleRequest LoadAssetAsync<T>(string name) where T : Object
    {
        if (!typeof(T).IsGeneratedAssemblyType())
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetAsync(name, Il2CppType.Of<T>().Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null);
    }

    public Il2CppAssetBundleRequest LoadAssetAsync(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAssetAsync(name, type.Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppAssetBundleRequest(intptr) : null);
    }

    public IntPtr LoadAssetAsync(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (LoadAssetAsync_InternalDelegateField == null)
            throw new NullReferenceException("The LoadAssetAsync_InternalDelegateField cannot be null.");
        return LoadAssetAsync_InternalDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(name), typeptr);
    }

    public Il2CppReferenceArray<Object> LoadAll() => LoadAllAssets();

    public Il2CppReferenceArray<Object> LoadAllAssets() => LoadAllAssets<Object>();

    public Il2CppReferenceArray<T> LoadAll<T>() where T : Object => LoadAllAssets<T>();

    public Il2CppReferenceArray<T> LoadAllAssets<T>() where T : Object
    {
        if (!typeof(T).IsGeneratedAssemblyType())
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAllAssets(Il2CppType.Of<T>().Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppReferenceArray<T>(intptr) : null);
    }

    public Il2CppReferenceArray<Object> LoadAll(Il2CppSystem.Type type) => LoadAllAssets(type);

    public Il2CppReferenceArray<Object> LoadAllAssets(Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAllAssets(type.Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppReferenceArray<Object>(intptr) : null);
    }

    public IntPtr LoadAll(IntPtr typeptr) => LoadAllAssets(typeptr);

    public IntPtr LoadAllAssets(IntPtr typeptr)
    {
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (LoadAssetWithSubAssets_InternalDelegateField == null)
            throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField cannot be null.");
        return LoadAssetWithSubAssets_InternalDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(string.Empty), typeptr);
    }

    public Il2CppReferenceArray<Object> LoadAssetWithSubAssets(string name) => LoadAssetWithSubAssets<Object>(name);

    public Il2CppReferenceArray<T> LoadAssetWithSubAssets<T>(string name) where T : Object
    {
        if (!typeof(T).IsGeneratedAssemblyType())
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetWithSubAssets(name, Il2CppType.Of<T>().Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppReferenceArray<T>(intptr) : null);
    }

    public Il2CppReferenceArray<Object> LoadAssetWithSubAssets(string name, Il2CppSystem.Type type)
    {
        if (type == null)
            throw new NullReferenceException("The input type cannot be null.");
        var intptr = LoadAssetWithSubAssets(name, type.Pointer);
        return ((intptr != IntPtr.Zero) ? new Il2CppReferenceArray<Object>(intptr) : null);
    }

    public IntPtr LoadAssetWithSubAssets(string name, IntPtr typeptr)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The input asset name cannot be null or empty.");
        if (typeptr == IntPtr.Zero)
            throw new NullReferenceException("The input type cannot be IntPtr.Zero");
        if (LoadAssetWithSubAssets_InternalDelegateField == null)
            throw new NullReferenceException("The LoadAssetWithSubAssets_InternalDelegateField cannot be null.");
        return LoadAssetWithSubAssets_InternalDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(name), typeptr);
    }
    public Il2CppAssetBundleRequest LoadAssetWithSubAssetsAsync(string name) => LoadAssetWithSubAssetsAsync<Object>(name);

    public Il2CppAssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name) where T : Object
    {
        if (!typeof(T).IsGeneratedAssemblyType())
            throw new NullReferenceException("The type must be a Generated Assembly Type.");
        var intptr = LoadAssetWithSubAssetsAsync(name, Il2CppType.Of<T>().Pointer);
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
        if (LoadAssetWithSubAssetsAsync_InternalDelegateField == null)
            throw new NullReferenceException("The LoadAssetWithSubAssetsAsync_InternalDelegateField cannot be null.");
        return LoadAssetWithSubAssetsAsync_InternalDelegateField(bundleptr, IL2CPP.ManagedStringToIl2Cpp(name), typeptr);
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if (bundleptr == IntPtr.Zero)
            throw new NullReferenceException("The bundleptr cannot be IntPtr.Zero");
        if (UnloadDelegateField == null)
            throw new NullReferenceException("The UnloadDelegateField cannot be null.");
        UnloadDelegateField(bundleptr, unloadAllLoadedObjects);
    }

    private delegate bool get_isStreamedSceneAssetBundleDelegate(IntPtr _this);
    private static readonly returnMainAssetDelegate returnMainAssetDelegateField;
    private delegate IntPtr returnMainAssetDelegate(IntPtr _this);
    private static readonly get_isStreamedSceneAssetBundleDelegate get_isStreamedSceneAssetBundleDelegateField;
    private delegate bool ContainsDelegate(IntPtr _this, IntPtr name);
    private static readonly ContainsDelegate ContainsDelegateField;
    private delegate IntPtr GetAllAssetNamesDelegate(IntPtr _this);
    private static readonly GetAllAssetNamesDelegate GetAllAssetNamesDelegateField;
    private delegate IntPtr GetAllScenePathsDelegate(IntPtr _this);
    private static readonly GetAllScenePathsDelegate GetAllScenePathsDelegateField;
    private delegate IntPtr LoadAsset_InternalDelegate(IntPtr _this, IntPtr name, IntPtr type);
    private static readonly LoadAsset_InternalDelegate LoadAsset_InternalDelegateField;
    private delegate IntPtr LoadAssetAsync_InternalDelegate(IntPtr _this, IntPtr name, IntPtr type);
    private static readonly LoadAssetAsync_InternalDelegate LoadAssetAsync_InternalDelegateField;
    private delegate IntPtr LoadAssetWithSubAssets_InternalDelegate(IntPtr _this, IntPtr name, IntPtr type);
    private static readonly LoadAssetWithSubAssets_InternalDelegate LoadAssetWithSubAssets_InternalDelegateField;
    private delegate IntPtr LoadAssetWithSubAssetsAsync_InternalDelegate(IntPtr _this, IntPtr name, IntPtr type);
    private static readonly LoadAssetWithSubAssetsAsync_InternalDelegate LoadAssetWithSubAssetsAsync_InternalDelegateField;
    private delegate void UnloadDelegate(IntPtr _this, bool unloadAllObjects);
    private static readonly UnloadDelegate UnloadDelegateField;
}
