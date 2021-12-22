using System;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        public static Font TextFont;

        public static Texture2D BackgroundTexture;
        public static Texture2D ProgressBarTexture;
        public static Texture2D ProgressBarOutlineTexture;
        public static Texture2D LogoTexture;

        public static AnimatedImage Animation;

        internal static void Init()
        {
            BackgroundTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.Background.Value);
            ProgressBarTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.ProgressBar.Value);
            ProgressBarOutlineTexture = UIUtils.CreateColorTexture(Customization.Config.Colors.ProgressBarOutline.Value);

            MelonDebug.Msg("[UIStyleValues] LoadImage Logo");
            LogoTexture = new Texture2D(2, 2);
            bool imgLoaded = ImageConversion.LoadImage(LogoTexture, Convert.FromBase64String(
                (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? ImageDatas.LemonLogo
                : ImageDatas.MelonLogo), false);
            MelonDebug.Msg("[UIStyleValues] LoadImage returned " + imgLoaded);

            // Load Animated Image
            Animation = Customization.Images.LoadAnimatedImage();

            // Load Font
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}