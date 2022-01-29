using System;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class ProgressBar
    {
        internal int x, y;
        internal int width, height;
        internal Font font;
        private Texture2D innerTexture;
        private Texture2D outerTexture;

        public float progress;
        public string text = "";

        private string textCached = null;
        private Mesh textmesh;

        internal ProgressBar(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            font = UIStyleValues.TextFont;
            innerTexture = UIStyleValues.ProgressBarInnerTexture;
            outerTexture = UIStyleValues.ProgressBarOuterTexture;
        }

        internal void Render()
        {
            if (UICustomization.ProgressText.Enabled)
                RefreshTextmesh();

            if (UICustomization.ProgressBar.Enabled)
            {
                Graphics.DrawTexture(new Rect(x, y, width, height), outerTexture);
                Graphics.DrawTexture(new Rect(x + 6, y + 6, width - 12, height - 12), UIStyleValues.BackgroundTexture);
                Graphics.DrawTexture(new Rect(x + 9, y + 9, (int)((width - 18) * Math.Min(1.0f, progress)), height - 18), innerTexture);
            }

            if (UICustomization.ProgressText.Enabled)
            {
                font.material.SetPass(0);
                Graphics.DrawMeshNow(textmesh, new Vector3(x + width / 2, y + height / 2 + 1, 0), Quaternion.identity);
            }
        }

        private void RefreshTextmesh()
        {
            if (textCached == text)
                return;

            textCached = text;

            TextGenerationSettings settings2 = new TextGenerationSettings();
            settings2.textAnchor = TextAnchor.MiddleCenter;
            settings2.color = UICustomization.ProgressText.TextColor;
            settings2.generationExtents = new Vector2(540, 47.5f);
            settings2.richText = true;
            settings2.font = font;
            settings2.pivot = new Vector2(0.5f, 0.5f);
            settings2.fontSize = 16;
            settings2.fontStyle = FontStyle.Bold;
            settings2.verticalOverflow = VerticalWrapMode.Overflow;
            settings2.scaleFactor = 1f;
            settings2.lineSpacing = 1f;
            textmesh = TextMeshGenerator.Generate(text, settings2);
        }

        internal void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
