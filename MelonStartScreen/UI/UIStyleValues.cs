using System;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        public static Font TextFont;

        public static Texture2D BackgroundTexture;
        public static Texture2D ProgressBarInnerTexture;
        public static Texture2D ProgressBarOuterTexture;
        public static Texture2D LogoTexture;

        public static AnimatedImage Animation;

        internal static void Init()
        {
            BackgroundTexture = UIUtils.CreateColorTexture(Customization.Config.General.BackgroundColor);

            if (Customization.Config.ProgressBar.Enabled)
            {
                ProgressBarInnerTexture = UIUtils.CreateColorTexture(Customization.Config.ProgressBar.InnerColor);
                ProgressBarOuterTexture = UIUtils.CreateColorTexture(Customization.Config.ProgressBar.OuterColor);
            }

            if (Customization.Config.Logo.Enabled)
            {
                MelonDebug.Msg("[UIStyleValues] LoadImage Logo");
                LogoTexture = new Texture2D(2, 2);
                bool imgLoaded = ImageConversion.LoadImage(LogoTexture, Convert.FromBase64String(
                    (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                    ? ImageDatas.LemonLogo
                    : ImageDatas.MelonLogo), false);
                MelonDebug.Msg("[UIStyleValues] LoadImage returned " + imgLoaded);
            }

            // Load Animated Image
            if (Customization.Config.Animation.Enabled)
                Animation = Customization.Images.LoadAnimatedImage();

            // Load Font
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}