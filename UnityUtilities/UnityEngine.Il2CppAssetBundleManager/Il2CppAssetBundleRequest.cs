using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityEngine;

[SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case", Justification = "Deprecated members")]
public class Il2CppAssetBundleCreateRequest(IntPtr ptr) : AsyncOperation(ptr)
{
    static Il2CppAssetBundleCreateRequest()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<Il2CppAssetBundleCreateRequest>();

        get_assetBundleDelegateField = IL2CPP.ResolveICall<get_assetBundleDelegate>("UnityEngine.AssetBundleCreateRequest::get_assetBundle");
    }

    public Il2CppAssetBundle AssetBundle
    {
        [Il2CppInterop.Runtime.Attributes.HideFromIl2Cpp]
        get
        {
            var ptr = get_assetBundleDelegateField(Pointer);
            return ptr == IntPtr.Zero ? null : new Il2CppAssetBundle(ptr);
        }
    }

    [Obsolete("Use AssetBundle (starting with upper-case) instead. This will be removed in a future version.", true)]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "It's deprecated")]
    public Il2CppAssetBundle assetBundle => AssetBundle;

    private delegate IntPtr get_assetBundleDelegate(IntPtr _this);
    private static readonly get_assetBundleDelegate get_assetBundleDelegateField;
}

[SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case", Justification = "Deprecated members")]
public class Il2CppAssetBundleRequest(IntPtr ptr) : AsyncOperation(ptr)
{
    static Il2CppAssetBundleRequest()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<Il2CppAssetBundleRequest>();

        get_assetDelegateField = IL2CPP.ResolveICall<get_assetDelegate>("UnityEngine.AssetBundleRequest::get_asset");
        get_allAssetsDelegateField = IL2CPP.ResolveICall<get_allAssetsDelegate>("UnityEngine.AssetBundleRequest::get_allAssets");
    }

    public Object Asset
    {
        get
        {
            var ptr = get_assetDelegateField(Pointer);
            return ptr == IntPtr.Zero ? null : new Object(ptr);
        }
    }

    [Obsolete("Use Asset (starting with upper-case) instead. This will be removed in a future version.", true)]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "It's deprecated")]
    public Object asset => Asset;

    public Il2CppReferenceArray<Object> AllAssets
    {
        get
        {
            var ptr = get_allAssetsDelegateField(Pointer);
            return ptr == IntPtr.Zero ? null : new Il2CppReferenceArray<Object>(ptr);
        }
    }

    [Obsolete("Use AllAssets (starting with upper-case) instead. This will be removed in a future version.", true)]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "It's deprecated")]
    public Il2CppReferenceArray<Object> allAssets => AllAssets;

    private delegate IntPtr get_assetDelegate(IntPtr _this);
    private static readonly get_assetDelegate get_assetDelegateField;

    private delegate IntPtr get_allAssetsDelegate(IntPtr _this);
    private static readonly get_allAssetsDelegate get_allAssetsDelegateField;
}