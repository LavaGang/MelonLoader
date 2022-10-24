using System.IO;
using System.Linq;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UI_Utils
    {
        internal static Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.SetPixels(new Color[] { color, color, color, color });
            texture.Apply();
            return texture;
        }

        internal static Objects.UI_Image LoadImage(UI_Theme.ImageSettings imageSettings, string filename)
        {
            string filepath = ScanForFile(UI_Theme.ThemePath, filename, new string[] { ".gif", ".png", ".jpg", ".jpeg" });
            if (string.IsNullOrEmpty(filepath))
                return null;

            string fileext = Path.GetExtension(filepath).ToLowerInvariant();

            if (fileext.Equals(".gif"))
                return new Objects.UI_AnimatedImage(imageSettings, filepath);

            if (fileext.Equals(".png")
                || fileext.Equals(".jpg")
                || fileext.Equals(".jpeg"))
                return new Objects.UI_Image(imageSettings, filepath);

            return null;
        }

        internal static string ScanForFile(string folderPath, string filename, string[] fileExts)
        {
            string[] files = Directory.GetFiles(folderPath);
            if (files.Length <= 0)
                return null;

            string filename_lower = filename.ToLowerInvariant();
            return files.FirstOrDefault((string filepath) =>
            {
                string currentfilename = Path.GetFileNameWithoutExtension(filepath).ToLowerInvariant();
                if (!currentfilename.Equals(filename_lower))
                    return false;

                string fileext = Path.GetExtension(filepath).ToLowerInvariant();
                return fileExts.Contains(fileext);
            });
        }

        internal static string RandomFolder(string folderPath)
        {
            string[] files = Directory.GetDirectories(folderPath);
            if (files.Length <= 0)
                return null;
            return files[MelonUtils.RandomInt(0, files.Length)];
        }

        internal static void AnchorToScreen(UI_Anchor anchor, int x, int y, out int out_x, out int out_y)
        {
            int sw = Screen.width;
            int sh = Screen.height - 35;

            switch (anchor)
            {
                // Upper
                case UI_Anchor.UpperLeft:
                    y = sh - y;
                    goto default;
                case UI_Anchor.UpperCenter:
                    x = (sw / 2) + x;
                    y = sh - y;
                    goto default;
                case UI_Anchor.UpperRight:
                    x = sw - x;
                    y = sh - y;
                    goto default;

                // Middle
                case UI_Anchor.MiddleLeft:
                    y = (sh / 2) - y;
                    goto default;
                case UI_Anchor.MiddleCenter:
                    x = (sw / 2) + x;
                    y = (sh / 2) - y;
                    goto default;
                case UI_Anchor.MiddleRight:
                    x = sw - x;
                    y = (sh / 2) - y;
                    goto default;

                // Lower
                case UI_Anchor.LowerCenter:
                    x = (sw / 2) + x;
                    goto default;

                case UI_Anchor.LowerRight:
                    x = sw - x;
                    goto default;

                // End
                case UI_Anchor.LowerLeft:
                default:
                    out_x = x;
                    out_y = y;
                    break;
            }
        }

        internal static void AnchorToObject(UI_Anchor anchor, int x, int y, int width, int height, out int out_x, out int out_y)
        {
            switch (anchor)
            {
                // Upper
                case UI_Anchor.UpperCenter:
                    x -= (width / 2);
                    goto default;
                case UI_Anchor.UpperRight:
                    x -= width;
                    goto default;

                // Middle
                case UI_Anchor.MiddleLeft:
                    y -= (height / 2);
                    goto default;
                case UI_Anchor.MiddleCenter:
                    y -= (height / 2);
                    x -= (width / 2);
                    goto default;
                case UI_Anchor.MiddleRight:
                    y -= (height / 2);
                    x -= width;
                    goto default;

                // Lower
                case UI_Anchor.LowerLeft:
                    y -= height;
                    goto default;
                case UI_Anchor.LowerCenter:
                    y -= height;
                    x -= (width / 2);
                    goto default;
                case UI_Anchor.LowerRight:
                    y -= height;
                    x -= width;
                    goto default;

                // End
                case UI_Anchor.UpperLeft:
                default:
                    out_x = x;
                    out_y = y;
                    break;
            }
        }
    }
}
