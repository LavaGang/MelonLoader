using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal class UI_Background : UI_Object
    {
        private UI_Theme.cBackground config;
        private UI_Image image;
        internal Texture2D solidTexture;

        internal UI_Background(UI_Theme.cBackground backgroundSettings)
        {
            config = backgroundSettings;
            image = UI_Utils.LoadImage(config, "Background");

            solidTexture = UI_Utils.CreateColorTexture(config.SolidColor);
            solidTexture.hideFlags = HideFlags.HideAndDontSave;
            solidTexture.DontDestroyOnLoad();
            AllElements.Add(this);
        }

        internal override void Render()
        {
            int sw = Screen.width;
            int sh = Screen.height;

            if (solidTexture != null)
                Graphics.DrawTexture(new Rect(0, 0, sw, sh), solidTexture);

            if (image != null)
            {
                if (config.StretchToScreen)
                    image.Render(0, 0, sw, sh);
                else
                    image.Render();
            }
        }

        internal override void Dispose()
        {
            if (solidTexture == null)
                return;

            solidTexture.DestroyImmediate();
            solidTexture = null;
        }
    }
}
