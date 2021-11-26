//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;

namespace MelonLoader.MelonStartScreen.UI
{
    public static class ImageFrameParser
    {
        public class ParsedInfo
        {
            public int Width;
            public int Height;
            public byte[][] FrameBuffer;
            //public ImageFormat FrameFormat;
        }

        /*
        public static ParsedInfo FromFile(string filepath, ImageFormat frameformat = null)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));

            Image image = Image.FromFile(filepath);
            if (image == null)
                return null;

            if (frameformat == null)
                frameformat = ImageFormat.Png;

            byte[][] framebuffer = image.ToFrameBuffer(frameformat);
            if (framebuffer == null)
                return null;

            ParsedInfo parsedInfo = new ParsedInfo()
            {
                Width = image.Width,
                Height = image.Height,
                FrameBuffer = framebuffer,
                FrameFormat = frameformat
            };
            image.Dispose();
            return parsedInfo;
        }

        public static ParsedInfo FromByteArray(byte[] filedata, ImageFormat frameformat = null)
        {
            if (filedata == null)
                throw new ArgumentNullException(nameof(filedata));

            Image image = null;
            using (MemoryStream ms = new MemoryStream(filedata))
            {
                image = Image.FromStream(ms);
                ms.Close();
            }
            if (image == null)
                return null;

            if (frameformat == null)
                frameformat = ImageFormat.Png;

            byte[][] framebuffer = image.ToFrameBuffer(frameformat);
            if (framebuffer == null)
                return null;

            ParsedInfo parsedInfo = new ParsedInfo()
            {
                Width = image.Width,
                Height = image.Height,
                FrameBuffer = framebuffer,
                FrameFormat = frameformat
            };
            image.Dispose();
            return parsedInfo;
        }

        public static ParsedInfo FromFrameBuffer(byte[][] framebuffer, ImageFormat frameformat = null)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            Image image = null;
            using (MemoryStream ms = new MemoryStream(framebuffer[0]))
            {
                image = Image.FromStream(ms);
                ms.Close();
            }
            if (image == null)
                return null;

            ParsedInfo parsedInfo = new ParsedInfo()
            {
                Width = image.Width,
                Height = image.Height,
                FrameBuffer = framebuffer,
                FrameFormat = frameformat
            };
            image.Dispose();
            return parsedInfo;
        }

        public static byte[][] ToFrameBuffer(this Image image, ImageFormat frame_format = null)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (frame_format == null)
                frame_format = ImageFormat.Png;

            FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(frameDimension);
            byte[][] framebuffer = new byte[frameCount][];
            for (int i = 0; i < frameCount; i++)
            {
                image.SelectActiveFrame(FrameDimension.Time, i);

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, frame_format);
                    framebuffer[i] = ms.ToArray();
                    ms.Close();
                }
            }
            image.SelectActiveFrame(FrameDimension.Time, 0);

            return framebuffer;
        }
        */
    }
}
