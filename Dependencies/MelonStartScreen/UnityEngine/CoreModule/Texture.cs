using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Texture : UnityObject
    {
        private delegate int GetDataWidthDelegate(IntPtr @this);
        private delegate int GetDataHeightDelegate(IntPtr @this);
        private delegate int set_filterModeDelegate(IntPtr @this, FilterMode filterMode);

        private static readonly GetDataWidthDelegate getDataWidth;
        private static readonly GetDataHeightDelegate getDataHeight;
        private static readonly set_filterModeDelegate set_filterMode_;

        static Texture()
        {
            InternalClassPointerStore<Texture>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Texture");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.1.0" }))
            {
                getDataWidth = UnityInternals.ResolveICall<GetDataWidthDelegate>("UnityEngine.Texture::GetDataWidth");
                getDataHeight = UnityInternals.ResolveICall<GetDataHeightDelegate>("UnityEngine.Texture::GetDataHeight");
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.1.0" }))
            {
                getDataWidth = UnityInternals.ResolveICall<GetDataWidthDelegate>("UnityEngine.Texture::Internal_GetWidth");
                getDataHeight = UnityInternals.ResolveICall<GetDataHeightDelegate>("UnityEngine.Texture::Internal_GetHeight");
            }
            set_filterMode_ = UnityInternals.ResolveICall<set_filterModeDelegate>("UnityEngine.Texture::set_filterMode");
        }

        public Texture(IntPtr ptr) : base(ptr) { }

        public int width => getDataWidth(UnityInternals.ObjectBaseToPtrNotNull(this));
        public int height => getDataHeight(UnityInternals.ObjectBaseToPtrNotNull(this));

        public FilterMode filterMode
        {
            set => set_filterMode_(UnityInternals.ObjectBaseToPtrNotNull(this), value);
        }
    }
}
