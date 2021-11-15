using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal sealed class GL
    {
        private static unsafe readonly delegate* unmanaged[Cdecl]<bool> m_get_sRGBWrite;

        unsafe static GL()
        {
            m_get_sRGBWrite = (delegate* unmanaged[Cdecl]<bool>)UnityInternals.ResolveICall("UnityEngine.GL::get_sRGBWrite");
        }

        public unsafe static bool sRGBWrite
        {
            get => m_get_sRGBWrite();
            // set
        }
    }
}
