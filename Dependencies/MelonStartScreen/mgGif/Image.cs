using System;
using MelonUnityEngine;

namespace mgGif
{
    internal class Image : ICloneable
    {
        public int Width;
        public int Height;
        public int Delay; // milliseconds
        public Color32[] RawImage;

        public Image() { }

        public Image(Image img)
        {
            Width = img.Width;
            Height = img.Height;
            Delay = img.Delay;
            RawImage = img.RawImage != null ? (Color32[])img.RawImage.Clone() : null;
        }

        public object Clone() => new Image(this);

        public Texture2D CreateTexture(FilterMode filterMode = FilterMode.Bilinear)
        {
            var tex = new Texture2D(Width, Height);
            tex.filterMode = filterMode;

            Color[] colors = new Color[RawImage.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = RawImage[i];

            tex.SetPixels(colors);
            tex.Apply();

            return tex;
        }
    }
}
