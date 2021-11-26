//using System;
using System.Diagnostics;
//using System.Drawing.Imaging;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal class AnimatedImage
    {
        public readonly int width, height;

        private float frameDelayMS = 90f;
        private Texture2D[] textures;
        private float aspectRatio;

        private Stopwatch stopwatch = new Stopwatch();

        /*
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

        public static AnimatedImage FromFrameBuffer(byte[][] frameBuffer, float framedelayms = 90f, ImageFormat frame_format = null)
        {
            if (frameBuffer == null)
                throw new ArgumentNullException(nameof(frameBuffer));
            ImageFrameParser.ParsedInfo parsedInfo = ImageFrameParser.FromFrameBuffer(frameBuffer);
            if (parsedInfo == null)
                return null;
            return new AnimatedImage(parsedInfo, framedelayms);
        }
        */

        public AnimatedImage(ImageFrameParser.ParsedInfo parsedInfo, float framedelayms = 90f)
            : this(parsedInfo.Width, parsedInfo.Height, parsedInfo.FrameBuffer, framedelayms) { }
        public AnimatedImage(int width, int height, byte[][] framebuffer, float framedelayms = 90f)
        {
            frameDelayMS = framedelayms;
            textures = new Texture2D[framebuffer.Length];
            this.width = width;
            this.height = height;
            aspectRatio = width / (float)height;
            for (int i = 0; i < framebuffer.Length; ++i)
            {
                Texture2D tex = new Texture2D(width, height);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, framebuffer[i], false);
                textures[i] = tex;
            }
        }

        public void Render(int x, int y, int width)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / frameDelayMS) % textures.Length);

            Graphics.DrawTexture(new Rect(x, y, width, -(int)(width / aspectRatio)), textures[image]);
        }
    }
}
