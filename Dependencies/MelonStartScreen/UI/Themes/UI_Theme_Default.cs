namespace MelonLoader.MelonStartScreen.UI.Themes
{
    internal class UI_Theme_Default : UI_Theme
    {
        internal UI_Theme_Default() => Defaults();

        internal override byte[] GetLoadingImage()
            => StartScreenResources.MelonLoadingIcon;

        internal override byte[] GetLogoImage()
            => StartScreenResources.MelonLogo;
    }
}
