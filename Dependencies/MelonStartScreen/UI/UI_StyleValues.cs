using System.IO;
using System.Linq;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UI_StyleValues
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
            Background = new Objects.UI_Background(UI_Config.Background);
            VersionText = new Objects.UI_Text(UI_Config.VersionText);
            ProgressBar = new Objects.UI_ProgressBar(UI_Config.ProgressBar, UI_Config.ProgressText);

            if (UI_Config.LogoImage.ScanForCustomImage)
                LogoImage = UI_Utils.LoadImage(UI_Config.LogoImage, "Logo");
            if (LogoImage == null)
                LogoImage = new Objects.UI_Image(UI_Config.LogoImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Logo_Lemon
                        : Properties.Resources.Logo_Melon);

            if (UI_Config.LoadingImage.ScanForCustomImage)
                LoadingImage = UI_Utils.LoadImage(UI_Config.LoadingImage, "Loading");
            if (LoadingImage == null)
                LoadingImage = new Objects.UI_AnimatedImage(UI_Config.LoadingImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Loading_Lemon
                        : Properties.Resources.Loading_Melon);
        }
    }
}