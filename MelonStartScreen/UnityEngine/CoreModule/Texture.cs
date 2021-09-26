using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Texture : Il2CppObjectBase
    {
        private delegate int GetDataWidthDelegate(IntPtr @this);
        private delegate int GetDataHeightDelegate(IntPtr @this);
        private delegate int set_filterModeDelegate(IntPtr @this, FilterMode filterMode);

        private static readonly GetDataWidthDelegate getDataWidth;
        private static readonly GetDataHeightDelegate getDataHeight;
        private static readonly set_filterModeDelegate set_filterMode_;

        static Texture()
        {
            Il2CppClassPointerStore<Texture>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Texture");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Texture>.NativeClassPtr);

            getDataWidth = IL2CPP.ResolveICall<GetDataWidthDelegate>("UnityEngine.Texture::GetDataWidth");
            getDataHeight = IL2CPP.ResolveICall<GetDataHeightDelegate>("UnityEngine.Texture::GetDataHeight");
            set_filterMode_ = IL2CPP.ResolveICall<set_filterModeDelegate>("UnityEngine.Texture::set_filterMode");
        }

        public Texture(IntPtr ptr) : base(ptr) { }

        public int width => getDataWidth(IL2CPP.Il2CppObjectBaseToPtrNotNull(this));
        public int height => getDataHeight(IL2CPP.Il2CppObjectBaseToPtrNotNull(this));

        public FilterMode filterMode
        {
            set => set_filterMode_(IL2CPP.Il2CppObjectBaseToPtrNotNull(this), value);
        }
    }
}
