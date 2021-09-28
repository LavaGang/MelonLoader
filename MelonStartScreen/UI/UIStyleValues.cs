using System;
using System.Linq;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        public static Font standardFont;

        public static Texture2D backgroundTexture;
        public static Texture2D progressbarOuterTexture;
        public static Texture2D progressbarInnerTexture;

        public static Texture2D melonloaderLogoTexture;

        public static AnimatedImage funnyAnimation;

        internal static void Init()
        {
            backgroundTexture = UIUtils.CreateColorTexture(new Color(0.08f, 0.09f, 0.10f));
            progressbarOuterTexture = UIUtils.CreateColorTexture(new Color(0.47f, 0.97f, 0.39f));
            progressbarInnerTexture = UIUtils.CreateColorTexture(new Color(1.00f, 0.23f, 0.42f));

            melonloaderLogoTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(melonloaderLogoTexture, Convert.FromBase64String(
                (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? ImageDatas.LemonLogo
                : ImageDatas.MelonLogo), false);

            funnyAnimation = new AnimatedImage(ImageDatas.FunnyImage.Select(data => Convert.FromBase64String(data)).ToArray(), 90);

            // Load default font
            standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
