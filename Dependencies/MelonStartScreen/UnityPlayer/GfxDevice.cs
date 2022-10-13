using MelonLoader;
using MelonLoader.NativeUtils;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityPlayer
{
    internal class GfxDevice
    {
        private delegate void PresentFrameDelegate();
        private delegate void WaitForLastPresentationAndGetTimestampDelegate(IntPtr gfxDevice);
        private delegate IntPtr GetRealGfxDeviceDelegate();

#pragma warning disable 0649
        #region m_PresentFrame Signatures
        [NativeSignature(01, NativeSignatureFlags.X86, "e8 ?? ?? ?? ?? 85 c0 74 12 e8 ?? ?? ?? ?? 8b ?? 8b ?? 8b 42 70 ff d0 84 c0 75", "2017.1.0", "5.6.0", "2017.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X86, "55 8b ec 51 e8 ?? ?? ?? ?? 85 c0 74 12 e8 ?? ?? ?? ?? 8b c8 8b 10 8b 42 ?? ff d0 84 c0 75", "2018.1.0")]
        [NativeSignature(03, NativeSignatureFlags.X86, "55 8b ec 51 e8 ?? ?? ?? ?? 85 c0 74 15 e8 ?? ?? ?? ?? 8b c8 8b 10 8b 82 ?? 00 00 00 ff d0", "2018.4.9", "2019.1.0")]
        [NativeSignature(04, NativeSignatureFlags.X86, "55 8b ec 51 56 e8 ?? ?? ?? ?? 8b f0 8b ce e8 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? e8", "2018.4.18", "2019.3.0", "2020.1.0")]

        [NativeSignature(01, NativeSignatureFlags.X64, "48 83 ec 28 e8 ?? ?? ?? ?? 48 85 c0 74 15 e8 ?? ?? ?? ?? 48 8b c8 48 8b 10 ff 92 e0 00 00 00 84 c0", "5.6.0", "2017.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X64, "48 83 ec 28 e8 ?? ?? ?? ?? 48 85 c0 74 15 e8 ?? ?? ?? ?? 48 8b c8 48 8b 10 ff 92 ?? ?? 00 00 84 c0", "2018.3.0", "2019.1.0")] // We can't use this one too early, else we match multiple functions
        [NativeSignature(03, NativeSignatureFlags.X64, "40 53 48 83 ec 20 e8 ?? ?? ?? ?? 48 8b c8 48 8b d8 e8 ?? ?? ?? ?? e8 ?? ?? ?? ?? 48 85 c0 74", "2018.4.18", "2019.3.0", "2020.1.0")]
        #endregion
        private static PresentFrameDelegate m_PresentFrame;

        #region m_D3D11WaitForLastPresentationAndGetTimestamp Signatures
        [NativeSignature(00, NativeSignatureFlags.None, null, "2017.1.0")]

        [NativeSignature(01, NativeSignatureFlags.X86, "55 8b ec 83 ec 40 53 56 8b d9 57 89 5d fc e8 ?? ?? ?? ?? 6a 02 8b c8", "2020.2.7", "2020.3.0", "2021.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X86, "55 8b ec 83 ec 48 53 56 8b d9 57 89 5d fc e8 ?? ?? ?? ?? 6a 02 8b c8", "2021.1.5", "2021.2.0")]
        [NativeSignature(03, NativeSignatureFlags.X86, "55 8b ec 83 ec 58 53 56 8b d9 57 89 5d fc e8 ?? ?? ?? ?? 6a 02 8b c8", "2022.1.0")]
        [NativeSignature(04, NativeSignatureFlags.X86 | NativeSignatureFlags.Mono, null, "2020.3.9")] // TODO validate this with more advanced sigcheck

        [NativeSignature(01, NativeSignatureFlags.X64, "48 89 5c 24 10 56 48 81 ec 90 00 00 00 0f 29 b4 24 80 00 00 00 48 8b f1", "2020.2.7", "2020.3.0", "2021.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X64, "48 89 5c 24 10 56 48 81 ec b0 00 00 00 0f 29 b4 24 a0 00 00 00 48 8b f1", "2022.1.0")]
        #endregion
        private static WaitForLastPresentationAndGetTimestampDelegate m_D3D11WaitForLastPresentationAndGetTimestamp;

        #region m_D3D12WaitForLastPresentationAndGetTimestamp Signatures
        [NativeSignature(00, NativeSignatureFlags.None, null, "2017.1.0")]

        [NativeSignature(01, NativeSignatureFlags.X86, "55 8b ec 83 ec 40 53 56 57 8b f9 89 7d f4 e8 ?? ?? ?? ?? 6a 02 8b c8", "2020.2.7", "2020.3.0", "2021.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X86, "55 8b ec 83 ec 48 56 57 8b f9 89 7d f0 e8 ?? ?? ?? ?? 6a 02 8b c8", "2020.3.9", "2021.1.5")]
        [NativeSignature(03, NativeSignatureFlags.X86, "55 8b ec 83 ec 48 56 57 8b f9 89 7d f8 e8 ?? ?? ?? ?? 6a 02 8b c8", "2021.2.0")]
        [NativeSignature(04, NativeSignatureFlags.X86, "55 8b ec 83 ec 58 56 57 8b f9 89 7d f8 e8 ?? ?? ?? ?? 6a 02 8b c8", "2022.1.0")]

        [NativeSignature(01, NativeSignatureFlags.X64, "48 89 5c 24 08 57 48 81 ec 90 00 00 00 0f 29 b4 24 80 00 00 00 48 8b d9", "2020.2.7", "2020.3.0", "2021.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X64, "48 89 5c 24 08 57 48 81 ec b0 00 00 00 0f 29 b4 24 a0 00 00 00 48 8b d9", "2022.1.0")]
        #endregion
        private static WaitForLastPresentationAndGetTimestampDelegate m_D3D12WaitForLastPresentationAndGetTimestamp;
#pragma warning restore 0649

        private static GetRealGfxDeviceDelegate m_GetRealGfxDevice;

        static GfxDevice()
        {
            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new[] { "2020.2.7", "2020.3.0", "2021.1.0" }))
            {
                // `FrameTimingManager_CUSTOM_CaptureFrameTimings()` calls `GetRealGfxDevice()` after 4 bytes.
                m_GetRealGfxDevice = (GetRealGfxDeviceDelegate)Marshal.GetDelegateForFunctionPointer(
                    CppUtils.ResolveRelativeInstruction(
                        (IntPtr)((long)UnityInternals.ResolveICall("UnityEngine.FrameTimingManager::CaptureFrameTimings") + (MelonUtils.IsGame32Bit() ? 0 : 4))),
                    typeof(GetRealGfxDeviceDelegate));
            }
        }

        public static void PresentFrame() =>
            m_PresentFrame();

        public static IntPtr GetRealGfxDevice() =>
            m_GetRealGfxDevice();

        internal static void WaitForLastPresentationAndGetTimestamp(uint deviceType)
        {
            if (m_GetRealGfxDevice == null)
                throw new NotImplementedException();

            IntPtr gfxDevice = GetRealGfxDevice();
            if (gfxDevice == IntPtr.Zero)
                throw new NotImplementedException();

            switch (deviceType)
            {
                case /*DX11*/ 2:
                    if (m_D3D11WaitForLastPresentationAndGetTimestamp == null)
                        throw new NotImplementedException();
                    m_D3D11WaitForLastPresentationAndGetTimestamp(gfxDevice); 
                    break;

                case /*DX12*/ 18:
                    if (m_D3D12WaitForLastPresentationAndGetTimestamp == null)
                        throw new NotImplementedException();
                    m_D3D12WaitForLastPresentationAndGetTimestamp(gfxDevice); 
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
