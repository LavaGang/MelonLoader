using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using UnhollowerMini;

namespace MelonUnityEngine.CoreModule
{
    internal sealed class SystemInfo
    {
        private delegate uint d_GetGraphicsDeviceType();
        private static readonly d_GetGraphicsDeviceType m_GetGraphicsDeviceType;

        unsafe static SystemInfo()
        {
            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.1.0" }))
                m_GetGraphicsDeviceType = UnityInternals.ResolveICall<d_GetGraphicsDeviceType>("UnityEngine.SystemInfo::GetGraphicsDeviceType");
            else
                m_GetGraphicsDeviceType = UnityInternals.ResolveICall<d_GetGraphicsDeviceType>("UnityEngine.SystemInfo::get_graphicsDeviceType");
        }

        public static uint GetGraphicsDeviceType() =>
            m_GetGraphicsDeviceType();
    }
}
