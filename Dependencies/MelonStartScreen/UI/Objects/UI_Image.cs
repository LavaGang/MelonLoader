using System;
using System.IO;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal class UI_Image : UI_Object
    {
        internal UI_Theme.ImageSettings config;
        internal Texture2D MainTexture;
        internal float AspectRatio;

        internal UI_Image(UI_Theme.ImageSettings imageSettings, string filepath) => LoadImage(imageSettings, File.ReadAllBytes(filepath));
        internal UI_Image(UI_Theme.ImageSettings imageSettings, byte[] filedata) => LoadImage(imageSettings, filedata);

        internal virtual void LoadImage(UI_Theme.ImageSettings imageSettings, byte[] filedata)
        {
            config = imageSettings;

            MainTexture = new Texture2D(2, 2);
            MainTexture.filterMode = config.Filter;

            if (!ImageConversion.LoadImage(MainTexture, filedata, false))
                throw new Exception("ImageConversion.LoadImage Failed!");

            AspectRatio = MainTexture.width / (float)MainTexture.height;

            MainTexture.hideFlags = HideFlags.HideAndDontSave;
            MainTexture.DontDestroyOnLoad();

            AllElements.Add(this);
        }

        internal override void Render()
        {
            if (!config.Enabled)
                return;
            if (MainTexture == null)
                return;

            int aspectHeight = config.MaintainAspectRatio ? (int)(config.Size.Item1 / AspectRatio) : config.Size.Item2;

            UI_Utils.AnchorToScreen(config.ScreenAnchor, config.Position.Item1, config.Position.Item2, out int anchor_x, out int anchor_y);
            UI_Utils.AnchorToObject(config.Anchor, anchor_x, anchor_y, config.Size.Item1, -aspectHeight, out anchor_x, out anchor_y);

            Graphics.DrawTexture(new Rect(anchor_x, anchor_y, config.Size.Item1, -aspectHeight), MainTexture);
        }

        internal virtual void Render(int x, int y, int width, int height)
        {
            if (!config.Enabled)
                return;
            Graphics.DrawTexture(new Rect(x, height + y, width, -height), MainTexture);
        }

        internal override void Dispose()
        {
            if (MainTexture == null)
                return;

            MainTexture.DestroyImmediate();
            MainTexture = null;
        }
    }
}
