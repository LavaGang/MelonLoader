using System;
using System.IO;
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
            bool imgLoaded = ImageConversion.LoadImage(melonloaderLogoTexture, Convert.FromBase64String(
                (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? ImageDatas.LemonLogo
                : ImageDatas.MelonLogo), false);
            MelonDebug.Msg("[UIStyleValues] LoadImage returned " + imgLoaded);

            bool load_default = true;
            string custom_image_path = Path.Combine(MelonUtils.UserDataDirectory, "Loading.gif");
            if (File.Exists(custom_image_path))
            {
                MelonDebug.Msg("[UIStyleValues] Found Custom Loading Screen Image!");
                try
                {
                    MelonDebug.Msg("[UIStyleValues] Loading AnimatedImage from Image...");
                    funnyAnimation = AnimatedImage.FromFile(custom_image_path);
                    if (funnyAnimation != null)
                    {
                        load_default = false;
                        MelonDebug.Msg("[UIStyleValues] Custom Loading Screen Image Loaded!");
                    }
                    else
                        MelonDebug.Error($"[UIStyleValues] Failed To Load AnimatedImage: AnimatedImage.FromFile returned null");
                }
                catch (Exception ex) { MelonDebug.Error($"[UIStyleValues] Failed To Load AnimatedImage: {ex}"); }
            }

            if (load_default)
            {
                MelonDebug.Msg("[UIStyleValues] Loading AnimatedImage from Default Loading Screen Image...");
                funnyAnimation = new AnimatedImage(ImageDatas.FunnyImage.Select(data => Convert.FromBase64String(data)).ToArray());
                MelonDebug.Msg("[UIStyleValues] Default Loading Screen Image Loaded!");
            }

            // Load default font
            standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}