using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        public static Font TextFont;

        public static Texture2D BackgroundTexture;
        public static Texture2D ProgressBarInnerTexture;
        public static Texture2D ProgressBarOuterTexture;

        public static Image BackgroundImage;
        public static Image LogoImage;
        public static Image LoadingImage;

        internal static void Init()
        {
            BackgroundTexture = UIUtils.CreateColorTexture(Customization.Config.General.BackgroundColor);
            BackgroundImage = Customization.Images.Background();

            if (Customization.Config.ProgressBar.Enabled)
            {
                ProgressBarInnerTexture = UIUtils.CreateColorTexture(Customization.Config.ProgressBar.InnerColor);
                ProgressBarOuterTexture = UIUtils.CreateColorTexture(Customization.Config.ProgressBar.OuterColor);
            }

            if (Customization.Config.Logo.Enabled)
                LogoImage = Customization.Images.Logo();

            // Load Loading Image
            if (Customization.Config.Animation.Enabled)
                LoadingImage = Customization.Images.Loading();

            // Load Font
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}