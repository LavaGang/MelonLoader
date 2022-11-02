namespace MelonLoader.MelonStartScreen.UI.Themes
{
    internal class UI_Theme_Default : UI_Theme
    {
        internal UI_Theme_Default() => Defaults();

        internal override byte[] GetLoadingImage()
            => Properties.Resources.Loading_Melon;

        internal override byte[] GetLogoImage()
            => Properties.Resources.Logo_Melon;
    }
}
