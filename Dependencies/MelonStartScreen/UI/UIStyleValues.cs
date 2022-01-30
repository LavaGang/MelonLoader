using System.IO;
using System.Linq;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIStyleValues
    {
        internal static Font TextFont;

        internal static Objects.UI_Background Background;
        internal static Objects.UI_Image LogoImage;
        internal static Objects.UI_Image LoadingImage;
        internal static Objects.UI_Text VersionText;
        internal static Objects.UI_ProgressBar ProgressBar;

        internal static void Init()
        {
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            Background = new Objects.UI_Background(UIConfig.Background);
            VersionText = new Objects.UI_Text(UIConfig.VersionText);
            ProgressBar = new Objects.UI_ProgressBar(UIConfig.ProgressBar, UIConfig.ProgressText);

            if (UIConfig.LogoImage.ScanForCustomImage)
                LogoImage = LoadImage(UIConfig.LogoImage, "Logo");
            if (LogoImage == null)
                LogoImage = new Objects.UI_Image(UIConfig.LogoImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Logo_Lemon
                        : Properties.Resources.Logo_Melon);

            if (UIConfig.LoadingImage.ScanForCustomImage)
                LoadingImage = LoadImage(UIConfig.LoadingImage, "Loading");
            if (LoadingImage == null)
                LoadingImage = new Objects.UI_AnimatedImage(UIConfig.LoadingImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Loading_Lemon
                        : Properties.Resources.Loading_Melon);
        }

        internal static Objects.UI_Image LoadImage(UIConfig.ImageSettings imageSettings, string filename)
        {
            string filepath = ScanForFile(Core.FolderPath, filename);
            if (string.IsNullOrEmpty(filepath))
                filepath = ScanForFile(Core.ElementsFolderPath, filename);
            if (string.IsNullOrEmpty(filepath))
                return null;
            string fileext = Path.GetExtension(filepath).ToLowerInvariant();
            if (fileext.Equals(".gif"))
                return new Objects.UI_AnimatedImage(imageSettings, filepath);
            if (fileext.Equals(".png")
                || fileext.Equals(".jpg")
                || fileext.Equals(".jpeg"))
                return new Objects.UI_Image(imageSettings, filepath);
            return null;
        }

        private static string ScanForFile(string folderPath, string filename)
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