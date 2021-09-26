using System.Diagnostics;
using UnhollowerMini;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class AnimatedImage
    {
        private float imagedelayms;
        private Texture2D[] textures;
        private Stopwatch stopwatch = new Stopwatch();

        public AnimatedImage(byte[][] images, float imagedelayms)
        {
            this.imagedelayms = imagedelayms;
            this.textures = new Texture2D[images.Length];
            for (int i = 0; i < images.Length; ++i)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, images[i], false);
                this.textures[i] = tex;
            }
        }

        public void Render(int x, int y, int width, int height)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / imagedelayms) % textures.Length);

            Graphics.DrawTexture(new Rect(x, y, width, height), textures[image]);
        }
    }
}
