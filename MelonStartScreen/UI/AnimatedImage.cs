using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class AnimatedImage : Image
    {
        private Stopwatch stopwatch = new Stopwatch();
        private float frameDelayMS = 90f;
        internal Texture2D[] textures;

        internal AnimatedImage(string filepath, FilterMode filterMode = FilterMode.Bilinear) : this(File.ReadAllBytes(filepath), filterMode) { }
        internal AnimatedImage(byte[] filedata, FilterMode filterMode = FilterMode.Bilinear)
        {
            mgGif.Decoder decoder = new mgGif.Decoder(filedata);

            var img = decoder.NextImage();
            frameDelayMS = img.Delay;
            SetSize(img.Width, img.Height);

            List<Texture2D> images = new List<Texture2D>();
            while (img != null)
            {
                images.Add(img.CreateTexture(filterMode));
                img = decoder.NextImage();
            }

            textures = images.ToArray();
            MainTexture = textures[0];
        }

        internal AnimatedImage(byte[][] framebuffer, float framedelayms = 90f, FilterMode filterMode = FilterMode.Bilinear)
        {
            frameDelayMS = framedelayms;
            textures = new Texture2D[framebuffer.Length];

            for (int i = 0; i < framebuffer.Length; ++i)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.filterMode = filterMode;
                if (!ImageConversion.LoadImage(tex, framebuffer[i], false))
                    throw new Exception("ImageConversion.LoadImage returned false");
                textures[i] = tex;
            }

            MainTexture = textures[0];
            SetSize(MainTexture.width, MainTexture.height);
        }

        internal AnimatedImage(Texture2D[] frames, float framedelayms = 90f)
        {
            frameDelayMS = framedelayms;
            textures = frames;
            MainTexture = textures[0];
            SetSize(MainTexture.width, MainTexture.height);
        }

        internal override void Render(int x, int y, int width)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / frameDelayMS) % textures.Length);
            MainTexture = textures[image];
            base.Render(x, y, width);
        }
    }
}
