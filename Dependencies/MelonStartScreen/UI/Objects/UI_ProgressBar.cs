using System;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal class UI_ProgressBar : UI_Object
    {
        private UI_Theme.cProgressBar config;
        internal float progress;
        internal UI_Text text;
        private Texture2D innerTexture;
        private Texture2D outerTexture;

        internal UI_ProgressBar(UI_Theme.cProgressBar progressBarSettings, UI_Theme.TextSettings textSettings)
        {
            config = progressBarSettings;
            text = new UI_Text(textSettings);

            innerTexture = UI_Utils.CreateColorTexture(config.InnerColor);
            innerTexture.hideFlags = HideFlags.HideAndDontSave;
            innerTexture.DontDestroyOnLoad();

            outerTexture = UI_Utils.CreateColorTexture(config.OuterColor);
            outerTexture.hideFlags = HideFlags.HideAndDontSave;
            outerTexture.DontDestroyOnLoad();

            AllElements.Add(this);
        }

        internal override void Render()
        {
            if (config.Enabled && (outerTexture != null) && (UI_Style.Background.solidTexture != null) && (innerTexture != null))
            {
                UI_Utils.AnchorToScreen(config.ScreenAnchor, config.Position.Item1, config.Position.Item2, out int anchor_x, out int anchor_y);
                UI_Utils.AnchorToObject(config.Anchor, anchor_x, anchor_y, config.Size.Item1, config.Size.Item2, out anchor_x, out anchor_y);

                Graphics.DrawTexture(new Rect(anchor_x, anchor_y, config.Size.Item1, config.Size.Item2), outerTexture);
                Graphics.DrawTexture(new Rect(anchor_x + 6, anchor_y + 6, config.Size.Item1 - 12, config.Size.Item2 - 12), UI_Style.Background.solidTexture);
                Graphics.DrawTexture(new Rect(anchor_x + 9, anchor_y + 9, (int)((config.Size.Item1 - 18) * Math.Min(1.0f, progress)), config.Size.Item2 - 18), innerTexture);
            }

            text?.Render();
        }

        internal override void Dispose()
        {
            if (innerTexture != null)
            {
                innerTexture.DestroyImmediate();
                innerTexture = null;
            }

            if (outerTexture != null)
            {
                outerTexture.DestroyImmediate();
                outerTexture = null;
            }

            if (text != null)
                text.Dispose();
        }
    }
}
