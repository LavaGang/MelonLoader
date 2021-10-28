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
            ImageFrameParser.ParsedInfo parsedInfo = ImageFrameParser.FromFile(filepath);
            if (parsedInfo == null)
                return null;
            return new AnimatedImage(parsedInfo, framedelayms);
        }

        public static AnimatedImage FromByteArray(byte[] filedata, float framedelayms = 90f, ImageFormat frame_format = null)
        {
            if (filedata == null)
                throw new ArgumentNullException(nameof(filedata));
            ImageFrameParser.ParsedInfo parsedInfo = ImageFrameParser.FromByteArray(filedata);
            if (parsedInfo == null)
                return null;
            return new AnimatedImage(parsedInfo, framedelayms);
        }

        public AnimatedImage(int width, int height, byte[][] framebuffer, float framedelayms = 90f)
        {
            frameDelayMS = framedelayms;
            textures = new Texture2D[framebuffer.Length];
            for (int i = 0; i < framebuffer.Length; ++i)
            {
                Texture2D tex = new Texture2D(width, height);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, framebuffer[i], false);
                textures[i] = tex;
            }
        }

        public AnimatedImage(ImageFrameParser.ParsedInfo parsedInfo, float framedelayms = 90f)
        {
            frameDelayMS = framedelayms;
            textures = new Texture2D[parsedInfo.FrameBuffer.Length];
            for (int i = 0; i < parsedInfo.FrameBuffer.Length; ++i)
            {
                Texture2D tex = new Texture2D(parsedInfo.Width, parsedInfo.Height);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, parsedInfo.FrameBuffer[i], false);
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
