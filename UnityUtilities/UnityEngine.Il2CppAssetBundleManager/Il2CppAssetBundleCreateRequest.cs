using Il2CppInterop.Runtime;
using System;

namespace UnityEngine;

public sealed class Il2CppAssetBundleCreateRequest
{
    public Il2CppAssetBundleCreateRequest(IntPtr ptr)
    {
        Pointer = ptr;
    }

    public IntPtr Pointer { get; }

    public Il2CppAssetBundle assetBundle
    {
        get
        {
            var ptr = get_assetBundleDelegateField(this.Pointer);
            if (ptr == IntPtr.Zero)
                return null;
            return new Il2CppAssetBundle(ptr);
        }
    }

    private delegate Il2CppSystem.IntPtr get_assetBundleDelegate(Il2CppSystem.IntPtr _this);
    private static readonly get_assetBundleDelegate get_assetBundleDelegateField;

    static Il2CppAssetBundleCreateRequest()
    {
        get_assetBundleDelegateField = RuntimeInvoke.ResolveICall<get_assetBundleDelegate>("UnityEngine.AssetBundleCreateRequest::get_assetBundle");
    }
}
