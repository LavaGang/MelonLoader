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

            MelonDebug.Msg("[UIStyleValues] LoadImage Logo");
            melonloaderLogoTexture = new Texture2D(2, 2);
            bool imgLoaded = ImageConversion.LoadImage(melonloaderLogoTexture, Convert.FromBase64String(
                (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                ? ImageDatas.LemonLogo
                : ImageDatas.MelonLogo), false);
            MelonDebug.Msg("[UIStyleValues] LoadImage returned " + imgLoaded);

            /*
            string custom_image_path = ScanForCustomImage();
            if (!string.IsNullOrEmpty(custom_image_path))
            {
                MelonDebug.Msg("[UIStyleValues] Found Custom Loading Screen Image!");
                try
                {
                    MelonDebug.Msg("[UIStyleValues] Loading AnimatedImage from Image...");
                    funnyAnimation = AnimatedImage.FromFile(custom_image_path);
                    if (funnyAnimation != null)
                        MelonDebug.Msg("[UIStyleValues] Custom Loading Screen Image Loaded!");
                    else
                        MelonDebug.Error($"[UIStyleValues] Failed To Load AnimatedImage: AnimatedImage.FromFile returned null");
                }
                catch (Exception ex)
                {
                    funnyAnimation = null;
                    MelonDebug.Error($"[UIStyleValues] Failed To Load AnimatedImage: {ex}");
                }
            }
            */

            string customGif = ScanForCustomImage();
            if (!string.IsNullOrEmpty(customGif))
            {
                MelonDebug.Msg("[UIStyleValues] Loading AnimatedImage from Image...");
                var decoder = new GifDecoder(File.ReadAllBytes(customGif));

                System.Collections.Generic.List<Texture2D> images = new System.Collections.Generic.List<Texture2D>();
                var img = decoder.NextImage();

                int width = img.Width;
                int height = img.Height;

                while (img != null)
                {
                    images.Add(img.CreateTexture());
                    img = decoder.NextImage();
                }

                funnyAnimation = new AnimatedImage(width, height, images.ToArray());

                if (funnyAnimation != null)
                    MelonDebug.Msg("[UIStyleValues] Custom Loading Screen Image Loaded!");
                else
                    MelonDebug.Error($"[UIStyleValues] Failed To Load AnimatedImage: something returned null");
            }

            if (funnyAnimation == null)
            {
                MelonDebug.Msg("[UIStyleValues] Loading AnimatedImage from Start Screen Image...");
                funnyAnimation = new AnimatedImage(33, 40, ImageDatas.FunnyImage.Select(data => Convert.FromBase64String(data)).ToArray());
                MelonDebug.Msg("[UIStyleValues] Start Screen Image Loaded!");
            }

            // Load default font
            standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static string ScanForCustomImage()
        {
            string[] files = Directory.GetFiles(MelonUtils.UserDataDirectory);
            if (files.Length <= 0)
                return null;
            return files.FirstOrDefault(x => 
                Path.GetFileNameWithoutExtension(x).ToLowerInvariant().Equals("loading")
                && Path.GetExtension(x).ToLowerInvariant().Equals(".gif"));
        }
        
    }
}