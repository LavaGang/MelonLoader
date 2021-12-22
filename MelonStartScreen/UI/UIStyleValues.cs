using System;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        public static Font standardFont;

        public static Texture2D backgroundTexture;
        public static Texture2D progressbarInnerTexture;
        public static Texture2D progressbarOuterTexture;

        public static Texture2D melonloaderLogoTexture;

        public static AnimatedImage Animation;

        internal static void Init()
        {
            backgroundTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.Background.Value);
            progressbarInnerTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.ProgressBar.Value);
            progressbarOuterTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.ProgressBarOutline.Value);

            MelonDebug.Msg("[UIStyleValues] LoadImage Logo");
            melonloaderLogoTexture = new Texture2D(2, 2);
            bool imgLoaded = ImageConversion.LoadImage(melonloaderLogoTexture, Convert.FromBase64String(
                (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? ImageDatas.LemonLogo
                : ImageDatas.MelonLogo), false);
            MelonDebug.Msg("[UIStyleValues] LoadImage returned " + imgLoaded);

            // Load Animated Image
            Animation = Customization.Images.LoadAnimatedImage();

            // Load Font
            standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}