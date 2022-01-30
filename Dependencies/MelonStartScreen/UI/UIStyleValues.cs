using System;
using System.IO;
using System.Linq;
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
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            BackgroundTexture = UIUtils.CreateColorTexture(UIConfig.Background.SolidColor);

            if (UIConfig.ProgressBar.Enabled)
            {
                ProgressBarInnerTexture = UIUtils.CreateColorTexture(UIConfig.ProgressBar.InnerColor);
                ProgressBarOuterTexture = UIUtils.CreateColorTexture(UIConfig.ProgressBar.OuterColor);
            }

            if (UIConfig.Background.ScanForCustomImage)
                BackgroundImage = LoadImage("Background", UIConfig.Background.Filter);

            if (UIConfig.LogoImage.Enabled)
            {
                if (UIConfig.LogoImage.ScanForCustomImage)
                    LogoImage = LoadImage("Logo", UIConfig.LogoImage.Filter);
                if (LogoImage == null)
                    LogoImage = new Image(
                    (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Logo_Lemon
                        : Properties.Resources.Logo_Melon, UIConfig.LogoImage.Filter);
            }

            if (UIConfig.LoadingImage.Enabled)
            {
                if (UIConfig.LoadingImage.ScanForCustomImage)
                    LoadingImage = LoadImage("Loading", UIConfig.LoadingImage.Filter);
                if (LoadingImage == null)
                    LoadingImage = new AnimatedImage(
                    (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Loading_Lemon
                        : Properties.Resources.Loading_Melon, UIConfig.LoadingImage.Filter);
            }
        }

        private static Image LoadImage(string filename, FilterMode filterMode = FilterMode.Bilinear)
        {
            string filepath = ScanForFile(filename);
            if (string.IsNullOrEmpty(filepath))
                return null;
            string fileext = Path.GetExtension(filepath).ToLowerInvariant();
            if (fileext.Equals(".gif"))
                return new AnimatedImage(filepath, filterMode);
            if (fileext.Equals(".png")
                || fileext.Equals(".jpg")
                || fileext.Equals(".jpeg"))
                return new Image(filepath, filterMode);
            return null;
        }

        private static string ScanForFile(string filename)
        {
            string[] files = Directory.GetFiles(Core.FolderPath);
            if (files.Length <= 0)
                return null;
            return files.FirstOrDefault(x =>
                Path.GetFileNameWithoutExtension(x)
                    .ToLowerInvariant()
                    .Equals(
                        filename
                        .ToLowerInvariant()));
        }
    }
}