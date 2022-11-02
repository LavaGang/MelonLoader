namespace MelonLoader.MelonStartScreen.UI.Themes
{
    internal class UI_Theme_Lemon : UI_Theme
    {
        internal UI_Theme_Lemon() => Defaults();

        internal override byte[] GetLoadingImage()
            => Properties.Resources.Loading_Lemon;

        internal override byte[] GetLogoImage()
            => Properties.Resources.Logo_Lemon;
    }
}
