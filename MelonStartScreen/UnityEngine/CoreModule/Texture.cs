using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Texture : InternalObjectBase
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
            UnityInternals.runtime_class_init(InternalClassPointerStore<Texture>.NativeClassPtr);

            getDataWidth = UnityInternals.ResolveICall<GetDataWidthDelegate>("UnityEngine.Texture::GetDataWidth");
            getDataHeight = UnityInternals.ResolveICall<GetDataHeightDelegate>("UnityEngine.Texture::GetDataHeight");
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
