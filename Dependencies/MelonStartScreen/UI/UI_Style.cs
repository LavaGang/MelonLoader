using MelonUnityEngine;
using System;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class UI_Style
    {
        internal static Objects.UI_Background Background;
        internal static Objects.UI_Image LogoImage;
        internal static Objects.UI_Image LoadingImage;
        internal static Objects.UI_Text VersionText;
        internal static Objects.UI_ProgressBar ProgressBar;

        internal static void Init()
        {
            Background = new Objects.UI_Background(UI_Theme.Instance.Background);
            VersionText = new Objects.UI_Text(UI_Theme.Instance.VersionText);
            ProgressBar = new Objects.UI_ProgressBar(UI_Theme.Instance.ProgressBar, UI_Theme.Instance.ProgressText);

            if (UI_Theme.Instance.LogoImage.ScanForCustomImage)
                LogoImage = UI_Utils.LoadImage(UI_Theme.Instance.LogoImage, "Logo");
            if (LogoImage == null)
                LogoImage = new Objects.UI_Image(UI_Theme.Instance.LogoImage, UI_Theme.Instance.GetLogoImage());

            if (UI_Theme.Instance.LoadingImage.ScanForCustomImage)
                LoadingImage = UI_Utils.LoadImage(UI_Theme.Instance.LoadingImage, "Loading");
            if (LoadingImage == null)
                LoadingImage = new Objects.UI_AnimatedImage(UI_Theme.Instance.LoadingImage, UI_Theme.Instance.GetLoadingImage());
        }


        internal static void Render()
        {
            Background.Render();
            LogoImage.Render();
            LoadingImage.Render();
            ProgressBar.Render();
            VersionText.Render();
        }
    }
}