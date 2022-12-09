namespace MelonLoader.MelonStartScreen.UI.Themes
{
    internal class UI_Theme_Lemon : UI_Theme
    {
        internal UI_Theme_Lemon() => Defaults();

        internal override byte[] GetLoadingImage()
            => StartScreenResources.LemonLoadingIcon;

        internal override byte[] GetLogoImage()
            => StartScreenResources.LemonLogo;
    }
}
