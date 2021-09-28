using MelonLoader.MelonStartScreen.NativeUtils;
using MelonLoader.MelonStartScreen.UI;
using System;
using UnityEngine;
using UnityPlayer;

namespace MelonLoader.MelonStartScreen
{
    internal static class ScreenRenderer
    {
        private const float logoRatio = 1.2353f;

        private delegate void SetupPixelCorrectCoordinates(bool param_1); 

#pragma warning disable 0649
        #region m_SetupPixelCorrectCoordinates Signatures
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? 8b d8 e8 ?? ?? ?? ?? ff 75 08 8d 4d f0", "2019.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? ff 75 08 8b d8 8d 45 f0 50 e8", "2017.3.0", "2018.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 56 e8 ?? ?? ?? ?? 8b f0 8b 45 08 50 8d 4d f0 51 e8", "2017.1.0")]
        [NativeSignature(NativeSignatureFlags.X64, "48 89 5c 24 08 57 48 81 ec a0 00 00 00 8b d9 e8 ?? ?? ?? ?? 48 8b f8 e8", "2017.1.0")]
        #endregion
        private static SetupPixelCorrectCoordinates m_SetupPixelCorrectCoordinates;
#pragma warning restore 0649

        public static bool disabled = false;

        private static Mesh melonloaderversionTextmesh;
        private static ProgressBar progressBar;

        internal static void Init()
        {
            if (disabled)
                return;

            UIStyleValues.Init();

            TextGenerationSettings settings = new TextGenerationSettings();
            settings.textAnchor = TextAnchor.MiddleCenter;
            settings.color = new Color(1, 1, 1);
            settings.generationExtents = new Vector2(540, 47.5f);
            settings.richText = true;
            settings.font = UIStyleValues.standardFont;
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.fontSize = 24;
            settings.fontStyle = FontStyle.Bold;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.scaleFactor = 1f;
            settings.lineSpacing = 1f;

            string melonloaderText = (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? "<color=#FFCC4D>LemonLoader</color>"
                : "<color=#78f764>Melon</color><color=#ff3c6a>Loader</color>"; 
            melonloaderversionTextmesh = TextMeshGenerator.Generate($"{melonloaderText} {BuildInfo.Version} Open-Beta", settings);

            progressBar = new ProgressBar(width: 540, height: 36);
        }


        internal static unsafe void Render()
        {
            if (disabled)
                return;

            try
            {
                m_SetupPixelCorrectCoordinates(false);

                int sw = Screen.width;
                int sh = Screen.height;

                int logoHeight = (int)(sh * 0.4f);
                int logoWidth = (int)(logoHeight * logoRatio);

                Graphics.DrawTexture(new Rect(0, 0, sw, sh), UIStyleValues.backgroundTexture);
                Graphics.DrawTexture(new Rect((sw - logoWidth) / 2, sh - ((sh - logoHeight) / 2 - 46), logoWidth, -logoHeight), UIStyleValues.melonloaderLogoTexture);

                // Animated image
                UIStyleValues.funnyAnimation.Render(sw - 200, 200, 132, -160);

                UIStyleValues.standardFont.material.SetPass(0);
                Graphics.DrawMeshNow(melonloaderversionTextmesh, new Vector3(sw / 2, sh - (sh / 2 + (logoHeight / 2) - 35), 0), Quaternion.identity);

                progressBar.SetPosition(
                    (sw - 540) / 2,
                    sh - ((sh - 36) / 2 + (logoHeight / 2) + 50));
                progressBar.Render();

                GfxDevice.PresentFrame();
            }
            catch (Exception e)
            {
                MelonLogger.Error("Exception while rendering: " + e);
            }
        }

        internal static void UpdateMainProgress(string text, float progress)
        {
            if (progressBar == null)
                return;

            progressBar.text = text;
            progressBar.progress = progress;
        }

        internal static void UpdateProgressFromLog(string msg)
        {
            if (progressBar == null)
                return;

            progressBar.progress = ProgressParser.GetProgressFromLog(msg, ref progressBar.text, progressBar.progress);
        }

        internal static void UpdateProgressFromMod(string modname)
        {
            if (progressBar == null)
                return;

            progressBar.progress = ProgressParser.GetProgressFromMod(modname, ref progressBar.text);
        }

        internal static void UpdateProgressState(ModLoadStep step)
        {
            if (progressBar == null)
                return;

            progressBar.progress = ProgressParser.SetModState(step, ref progressBar.text);
        }
    }
}
