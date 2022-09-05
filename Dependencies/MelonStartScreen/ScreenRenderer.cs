using MelonLoader.MelonStartScreen.NativeUtils;
using MelonLoader.MelonStartScreen.UI;
using System;
using System.Reflection;
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

        internal static void Init()
        {
            if (disabled)
                return;

            try
            {
                MelonDebug.Msg("Initializing UIStyleValues");
                UI_StyleValues.Init();
                MelonDebug.Msg("UIStyleValues Initialized");

                uint graphicsDeviceType = SystemInfo.GetGraphicsDeviceType();
                MelonDebug.Msg("Graphics Device Type: " + graphicsDeviceType);
                shouldCallWFLPAGT = NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new[] { "2020.2.7", "2020.3.0", "2021.1.0" })
                    && (graphicsDeviceType == /*DX11*/2 || graphicsDeviceType == /*DX12*/18)
                    ? graphicsDeviceType : 0;
            }
            catch (Exception e)
            {
                Core.Logger.Error("Exception while init rendering: " + e);
                disabled = true;
            }
        }

        internal static unsafe void Render()
        {
            if (disabled)
                return;

            try
            {
                m_SetupPixelCorrectCoordinates(false);

                UI_StyleValues.Background.Render();
                UI_StyleValues.LogoImage.Render();
                UI_StyleValues.LoadingImage.Render();
                UI_StyleValues.ProgressBar.Render();
                UI_StyleValues.VersionText.Render();

                /*
                if (melonloaderversionTextmesh != null)
                {
                    if (UIConfig.VersionText.AutoAlign)
                        Graphics.DrawMeshNow(melonloaderversionTextmesh, new Vector3(sw / 2, sh - (sh / 2 + (logoHeight / 2) - 35), 0), Quaternion.identity);
                    else
                        Graphics.DrawMeshNow(melonloaderversionTextmesh, new Vector3(UIConfig.VersionText.CustomPosition.Item1, sh - UIConfig.VersionText.CustomPosition.Item2, 0), Quaternion.identity);
                }

                if (progressBar != null)
                {
                    int x, y, width, height = 0;
                    width = 540;
                    height = 36;

                    if (UIConfig.ProgressBar.AutoAlign)
                    {
                        x = (sw - width) / 2;
                        y = sh - ((sh - height) / 2 + (logoHeight / 2) + 50);
                    }
                    else
                    {
                        x = UIConfig.ProgressBar.CustomPosition.Item1;
                        y = UIConfig.ProgressBar.CustomPosition.Item2;
                    }
                }
                */

                GfxDevice.PresentFrame();
                if (shouldCallWFLPAGT != 0)
                    GfxDevice.WaitForLastPresentationAndGetTimestamp(shouldCallWFLPAGT);
            }
            catch (Exception e)
            {
                Core.Logger.Error("Exception while rendering: " + e);
                disabled = true;
            }
        }

        internal static void UpdateMainProgress(string text, float progress)
        {
            if (UI_StyleValues.ProgressBar == null)
                return;

            UI_StyleValues.ProgressBar.text.text = text;
            UI_StyleValues.ProgressBar.text.isDirty = true;
            UI_StyleValues.ProgressBar.progress = progress;
        }

        internal static void UpdateProgressFromLog(string msg)
        {
            if (UI_StyleValues.ProgressBar == null)
                return;

            UI_StyleValues.ProgressBar.progress = ProgressParser.GetProgressFromLog(msg, ref UI_StyleValues.ProgressBar.text.text, UI_StyleValues.ProgressBar.progress);
            UI_StyleValues.ProgressBar.text.isDirty = true;
        }

        internal static void UpdateProgressFromMod(MelonBase melon)
        {
            if (UI_StyleValues.ProgressBar == null)
                return;

            UI_StyleValues.ProgressBar.progress = ProgressParser.GetProgressFromMod(melon, ref UI_StyleValues.ProgressBar.text.text);
            UI_StyleValues.ProgressBar.text.isDirty = true;
        }

        internal static void UpdateProgressFromModAssembly(Assembly asm)
        {
            if (UI_StyleValues.ProgressBar == null)
                return;

            UI_StyleValues.ProgressBar.progress = ProgressParser.GetProgressFromModAssembly(asm, ref UI_StyleValues.ProgressBar.text.text);
            UI_StyleValues.ProgressBar.text.isDirty = true;
        }

        internal static void UpdateProgressState(ModLoadStep step)
        {
            if (UI_StyleValues.ProgressBar == null)
                return;

            if (ProgressParser.SetModState(step, ref UI_StyleValues.ProgressBar.text.text, out float generationPart))
            {
                UI_StyleValues.ProgressBar.progress = generationPart;
                UI_StyleValues.ProgressBar.text.isDirty = true;
            }
        }
    }
}
