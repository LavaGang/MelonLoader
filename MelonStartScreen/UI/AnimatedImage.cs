using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class AnimatedImage
    {
        private float frameDelayMS = 90f;
        private Texture2D[] textures;
        private Stopwatch stopwatch = new Stopwatch();

        public static AnimatedImage FromFile(string filepath, float framedelayms = 90f, ImageFormat frame_format = null)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));
            byte[][] framebuffer = ImageFrameParser.FileToFrameBuffer(filepath);
            if (framebuffer == null)
                return null;
            return new AnimatedImage(framebuffer, framedelayms);
        }

        public AnimatedImage(byte[][] framebuffer, float framedelayms = 90f)
        {
            frameDelayMS = framedelayms;
            textures = new Texture2D[framebuffer.Length];
            for (int i = 0; i < framebuffer.Length; ++i)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, framebuffer[i], false);
                textures[i] = tex;
            }
        }

        public void Render(int x, int y, int width, int height)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / frameDelayMS) % textures.Length);

            Graphics.DrawTexture(new Rect(x, y, width, height), textures[image]);
        }
    }
}
