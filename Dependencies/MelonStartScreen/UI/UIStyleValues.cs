using System;
using System.IO;
using System.Reflection;
using MelonUnityEngine;

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

        private static byte[] GetResource(string name)
        {
            using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            if (s == null)
                return null;
#if NET6_0
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
#else
            var ret = new byte[s.Length];
            s.Read(ret, 0, ret.Length);
            return ret;
#endif
        }

        internal static void Init()
        {
            TextFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            Background = new Objects.UI_Background(UIConfig.Background);
            VersionText = new Objects.UI_Text(UIConfig.VersionText);
            ProgressBar = new Objects.UI_ProgressBar(UIConfig.ProgressBar, UIConfig.ProgressText);

            if (UIConfig.LogoImage.ScanForCustomImage)
                LogoImage = UIUtils.LoadImage(UIConfig.LogoImage, "Logo");
            if (LogoImage == null)
                LogoImage = new Objects.UI_Image(UIConfig.LogoImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                    ? GetResource("Logo_Lemon.dat")
                    : GetResource("Logo_Melon.dat"));

            if (UIConfig.LoadingImage.ScanForCustomImage)
                LoadingImage = UIUtils.LoadImage(UIConfig.LoadingImage, "Loading");
            if (LoadingImage == null)
                LoadingImage = new Objects.UI_AnimatedImage(UIConfig.LoadingImage, (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                    ? GetResource("Loading_Lemon.dat")
                    : GetResource("Loading_Melon.dat"));
        }
    }
}