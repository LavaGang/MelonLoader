using MelonLoader.MelonStartScreen.NativeUtils;
using MelonLoader.MelonStartScreen.UI;
using System;
using MelonUnityEngine;
using MelonUnityEngine.CoreModule;
using UnityPlayer;

namespace MelonLoader.MelonStartScreen
{
    internal static class ScreenRenderer
    {
        private const float logoRatio = 1.2353f;

        private delegate void SetupPixelCorrectCoordinates(bool param_1); 

#pragma warning disable 0649
        #region m_SetupPixelCorrectCoordinates Signatures
        [NativeSignature(01, NativeSignatureFlags.X86, "55 8b ec 83 ec 60 56 e8 ?? ?? ?? ?? 8b f0 8b 45 08 50 8d 4d f0 51 e8", "2017.1.0")]
        [NativeSignature(02, NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? ff 75 08 8b d8 8d 45 f0 50 e8", "2017.3.0", "2018.1.0")]
        [NativeSignature(03, NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? 8b d8 e8 ?? ?? ?? ?? ff 75 08 8d 4d f0", "2019.1.0")]
        [NativeSignature(01, NativeSignatureFlags.X64, "48 89 5c 24 08 57 48 81 ec a0 00 00 00 8b d9 e8 ?? ?? ?? ?? 48 8b f8 e8", "2017.1.0")]
        #endregion
        private static SetupPixelCorrectCoordinates m_SetupPixelCorrectCoordinates;
#pragma warning restore 0649

        public static bool disabled = false;

        private static uint shouldCallWFLPAGT = 0;

        internal static bool Init()
        {
            try
            {
                MelonDebug.Msg("Initializing UIStyleValues");
                UIStyleValues.Init();
                MelonDebug.Msg("UIStyleValues Initialized");

                uint graphicsDeviceType = SystemInfo.GetGraphicsDeviceType();
                MelonDebug.Msg("Graphics Device Type: " + graphicsDeviceType);
                shouldCallWFLPAGT = NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new[] { "2020.2.7", "2020.3.0", "2021.1.0" })
                    && (graphicsDeviceType == /*DX11*/2 || graphicsDeviceType == /*DX12*/18)
                    ? graphicsDeviceType : 0;

                return true;
            }
            catch (Exception e)
            {
                MelonLogger.Error("Exception while init rendering: " + e);
            }
            return false;
        }

        internal static unsafe void Render()
        {
            if (disabled)
                return;

            try
            {
                m_SetupPixelCorrectCoordinates(false);

                UIStyleValues.Background.Render();
                UIStyleValues.LogoImage.Render();
                UIStyleValues.LoadingImage.Render();
                UIStyleValues.ProgressBar.Render();
                UIStyleValues.VersionText.Render();

                GfxDevice.PresentFrame();
                if (shouldCallWFLPAGT != 0)
                    GfxDevice.WaitForLastPresentationAndGetTimestamp(shouldCallWFLPAGT);
            }
            catch (Exception e)
            {
                MelonLogger.Error("Exception while rendering: " + e);
                disabled = true;
            }
        }

        internal static void UpdateMainProgress(string text, float progress)
        {
            if (UIStyleValues.ProgressBar == null)
                return;

            UIStyleValues.ProgressBar.text.text = text;
            UIStyleValues.ProgressBar.text.isDirty = true;
            UIStyleValues.ProgressBar.progress = progress;
        }

        internal static void UpdateProgressFromLog(string msg)
        {
            if (UIStyleValues.ProgressBar == null)
                return;

            UIStyleValues.ProgressBar.progress = ProgressParser.GetProgressFromLog(msg, ref UIStyleValues.ProgressBar.text.text, UIStyleValues.ProgressBar.progress);
            UIStyleValues.ProgressBar.text.isDirty = true;
        }

        internal static void UpdateProgressFromMod(string modname)
        {
            if (UIStyleValues.ProgressBar == null)
                return;

            UIStyleValues.ProgressBar.progress = ProgressParser.GetProgressFromMod(modname, ref UIStyleValues.ProgressBar.text.text);
            UIStyleValues.ProgressBar.text.isDirty = true;
        }

        internal static void UpdateProgressState(ModLoadStep step)
        {
            if (UIStyleValues.ProgressBar == null)
                return;

            UIStyleValues.ProgressBar.progress = ProgressParser.SetModState(step, ref UIStyleValues.ProgressBar.text.text);
            UIStyleValues.ProgressBar.text.isDirty = true;
        }
    }
}
