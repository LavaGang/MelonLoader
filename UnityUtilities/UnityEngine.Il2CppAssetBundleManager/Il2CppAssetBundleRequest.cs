using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;

namespace UnityEngine;

public class Il2CppAssetBundleCreateRequest : AsyncOperation
{
    public Il2CppAssetBundleCreateRequest(IntPtr ptr) : base(ptr) { }

    static Il2CppAssetBundleCreateRequest()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<Il2CppAssetBundleCreateRequest>();

        get_assetBundleDelegateField = IL2CPP.ResolveICall<get_assetBundleDelegate>("UnityEngine.AssetBundleCreateRequest::get_assetBundle");
    }

    public Il2CppAssetBundle assetBundle
    {
        [Il2CppInterop.Runtime.Attributes.HideFromIl2Cpp]
        get
        {
            var ptr = get_assetBundleDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return new Il2CppAssetBundle(ptr);
        }
    }

    private delegate IntPtr get_assetBundleDelegate(IntPtr _this);
    private static readonly get_assetBundleDelegate get_assetBundleDelegateField;
}

public class Il2CppAssetBundleRequest : AsyncOperation
{
    public Il2CppAssetBundleRequest(IntPtr ptr) : base(ptr) { }

    static Il2CppAssetBundleRequest()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<Il2CppAssetBundleRequest>();

        get_assetDelegateField = IL2CPP.ResolveICall<get_assetDelegate>("UnityEngine.AssetBundleRequest::get_asset");
        get_allAssetsDelegateField = IL2CPP.ResolveICall<get_allAssetsDelegate>("UnityEngine.AssetBundleRequest::get_allAssets");
    }

    public Object asset
    {
        get
        {
            var ptr = get_assetDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return new Object(ptr);
        }
    }

    public Il2CppReferenceArray<Object> allAssets
    {
        get
        {
            var ptr = get_allAssetsDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return new Il2CppReferenceArray<Object>(ptr);
        }
    }

    private delegate IntPtr get_assetDelegate(IntPtr _this);
    private static readonly get_assetDelegate get_assetDelegateField;

    private delegate IntPtr get_allAssetsDelegate(IntPtr _this);
    private static readonly get_allAssetsDelegate get_allAssetsDelegateField;
}