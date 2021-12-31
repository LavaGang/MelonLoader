using UnhollowerMini;

namespace UnityEngine.CoreModule
{
    internal sealed class SystemInfo
    {
        private delegate uint d_GetGraphicsDeviceType();
        private static readonly d_GetGraphicsDeviceType m_GetGraphicsDeviceType;

        unsafe static SystemInfo()
        {
            m_GetGraphicsDeviceType = UnityInternals.ResolveICall<d_GetGraphicsDeviceType>("UnityEngine.SystemInfo::GetGraphicsDeviceType");
        }

        public static uint GetGraphicsDeviceType() =>
            m_GetGraphicsDeviceType();
    }
}
