using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class GifParser
    { 
        internal static byte[][] GifToFrameBuffer(string filepath)
        {
            Image image = Image.FromFile(filepath);
            if (image == null)
                return null;
            FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(frameDimension);
            byte[][] output = new byte[frameCount][];
            for (int i = 0; i < frameCount; i++)
            {
                image.SelectActiveFrame(FrameDimension.Time, i);
                Image frame = new Bitmap(image);
                using (MemoryStream ms = new MemoryStream())
                {
                    frame.Save(ms, ImageFormat.Png);
                    output[i] = ms.ToArray();
                    ms.Close();
                }
                frame.Dispose();
            }
            image.Dispose();
            return output;
        }
    }
}
