using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal sealed class GL
    {
        private delegate bool d_get_sRGBWrite();
        private static readonly d_get_sRGBWrite m_get_sRGBWrite;

        unsafe static GL()
        {
            m_get_sRGBWrite = UnityInternals.ResolveICall<d_get_sRGBWrite>("UnityEngine.GL::get_sRGBWrite");
        }

        public unsafe static bool sRGBWrite
        {
            get => m_get_sRGBWrite();
            // set
        }
    }
}
