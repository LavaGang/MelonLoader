using System.Collections.Generic;
using System.Diagnostics;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal class UI_AnimatedImage : UI_Image
    {
        private Stopwatch stopwatch = new Stopwatch();
        private float frameDelayMS = 90f;
        private Texture2D[] textures;

        internal UI_AnimatedImage(UI_Theme.ImageSettings imageSettings, string filepath) : base(imageSettings, filepath) { }
        internal UI_AnimatedImage(UI_Theme.ImageSettings imageSettings, byte[] filedata) : base(imageSettings, filedata) { }
        internal override void LoadImage(UI_Theme.ImageSettings imageSettings, byte[] filedata)
        {
            config = imageSettings;

            mgGif.Decoder decoder = new mgGif.Decoder(filedata);

            var img = decoder.NextImage();
            frameDelayMS = img.Delay;

            List<Texture2D> images = new List<Texture2D>();
            while (img != null)
            {
                Texture2D newtexture = img.CreateTexture(config.Filter);
                newtexture.hideFlags = HideFlags.HideAndDontSave;
                newtexture.DontDestroyOnLoad();

                images.Add(newtexture);
                img = decoder.NextImage();
            }

            textures = images.ToArray();
            MainTexture = textures[0];
            AspectRatio = MainTexture.width / (float)MainTexture.height;

            AllElements.Add(this);
        }

        internal override void Render()
        {
            if (!config.Enabled || (textures == null) || (textures.Length <= 0))
            {
                if (stopwatch.IsRunning)
                    stopwatch.Stop();
                return;
            }

            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / frameDelayMS) % textures.Length);
            MainTexture = textures[image];

            base.Render();
        }

        internal override void Render(int x, int y, int width, int height)
        {
            if (!config.Enabled)
            {
                if (stopwatch.IsRunning)
                    stopwatch.Stop();
                return;
            }

            if (!stopwatch.IsRunning)
                stopwatch.Start();

            int image = (int)((float)(stopwatch.ElapsedMilliseconds / frameDelayMS) % textures.Length);
            MainTexture = textures[image];

            base.Render(x, y, width, height);
        }

        internal override void Dispose()
        {
            if ((textures == null) || (textures.Length <= 0))
                return;

            foreach (Texture2D texture in textures)
                texture.DestroyImmediate();

            textures = null;
            MainTexture = null;
        }
    }
}
