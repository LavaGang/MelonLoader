using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;

namespace UnityEngine;

public class Il2CppAssetBundleRequest
{
    public Il2CppAssetBundleRequest(IntPtr ptr)
    {
        Pointer = ptr;
    }

    public IntPtr Pointer { get; }

    public Object asset
    {
        get
        {
            var ptr = get_assetDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return (Object)Il2CppObjectPool.Get(ptr);
        }
    }

    public Il2CppArrayRank1<Object> allAssets
    {
        get
        {
            var ptr = get_allAssetsDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return (Il2CppArrayRank1<Object>)Il2CppObjectPool.Get(ptr);
        }
    }

    private delegate Il2CppSystem.IntPtr get_assetDelegate(Il2CppSystem.IntPtr _this);
    private static readonly get_assetDelegate get_assetDelegateField = RuntimeInvoke.ResolveICall<get_assetDelegate>("UnityEngine.AssetBundleRequest::get_asset");

    private delegate Il2CppSystem.IntPtr get_allAssetsDelegate(Il2CppSystem.IntPtr _this);
    private static readonly get_allAssetsDelegate get_allAssetsDelegateField = RuntimeInvoke.ResolveICall<get_allAssetsDelegate>("UnityEngine.AssetBundleRequest::get_allAssets");

    static Il2CppAssetBundleRequest()
    {
        get_assetDelegateField = RuntimeInvoke.ResolveICall<get_assetDelegate>("UnityEngine.AssetBundleRequest::get_asset");
        get_allAssetsDelegateField = RuntimeInvoke.ResolveICall<get_allAssetsDelegate>("UnityEngine.AssetBundleRequest::get_allAssets");
    }
}