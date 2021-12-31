using System;
using System.IO;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class Image
    {
        internal int Width, Height;
        internal Texture2D MainTexture;
        internal float AspectRatio;

        internal Image() { }

        internal Image(string filepath, FilterMode filterMode = FilterMode.Bilinear)
        {
            byte[] filedata = File.ReadAllBytes(filepath);
            MainTexture = new Texture2D(2, 2);
            MainTexture.filterMode = filterMode;
            if (!ImageConversion.LoadImage(MainTexture, filedata, false))
                throw new Exception("ImageConversion.LoadImage Failed!");
            SetSize(MainTexture.width, MainTexture.height);
        }

        internal Image(byte[] filedata, FilterMode filterMode = FilterMode.Bilinear)
        {
            MainTexture = new Texture2D(2, 2);
            MainTexture.filterMode = filterMode;
            if (!ImageConversion.LoadImage(MainTexture, filedata, false))
                throw new Exception("ImageConversion.LoadImage Failed!");
            SetSize(MainTexture.width, MainTexture.height);
        }

        internal Image(Texture2D maintexture, FilterMode filterMode = FilterMode.Bilinear)
        {
            MainTexture = maintexture;
            MainTexture.filterMode = filterMode;
            SetSize(MainTexture.width, MainTexture.height);
        }

        internal void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            AspectRatio = width / (float)height;
        }

        internal virtual void Render(int x, int y, int width)
            => Graphics.DrawTexture(new Rect(x, y, width, -(int)(width / AspectRatio)), MainTexture);
        internal virtual void Render(int x, int y, int width, int height)
            => Graphics.DrawTexture(new Rect(x, y, width, height), MainTexture);
    }
}
