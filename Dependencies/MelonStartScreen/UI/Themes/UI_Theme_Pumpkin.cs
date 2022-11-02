using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Themes
{
    internal class UI_Theme_Pumpkin : UI_Theme
    {
        internal UI_Theme_Pumpkin()
        {
            Background = new hBackground();
            ProgressBar = new hProgressBar();
            VersionText = new hVersionTextSettings();
            Defaults();
        }

        internal override byte[] GetLoadingImage()
            => Properties.Resources.Logo_Halloween;

        internal override byte[] GetLogoImage()
            => Properties.Resources.Loading_Halloween;

        internal class hBackground : cBackground
        {
            public hBackground()
                => SolidColor = new Color(0.078f, 0f, 0.141f);
        }

        internal class hProgressBar : cProgressBar
        {
            public hProgressBar() : base()
            {
                OuterColor = new Color(0.478f, 0.169f, 0.749f);
                InnerColor = new Color(1f, 0.435f, 0f);
                Defaults();
            }
        }

        internal class hVersionTextSettings : VersionTextSettings
        {
            public hVersionTextSettings()
            {
                Text = "<loaderNameHalloween/> v<loaderVersion/> Open-Beta";
                Defaults();
            }
        }
    }
}
