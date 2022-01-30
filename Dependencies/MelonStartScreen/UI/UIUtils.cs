using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIUtils
    {
        internal static Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.SetPixels(new Color[] { color, color, color, color });
            texture.Apply();
            return texture;
        }

        internal static void AnchorToScreen(UIAnchor anchor, int x, int y, out int out_x, out int out_y)
        {
            int sw = Screen.width;
            int sh = Screen.height - 35;

            switch (anchor)
            {
                // Upper
                case UIAnchor.UpperLeft:
                    y = sh - y;
                    goto default;
                case UIAnchor.UpperCenter:
                    x = (sw / 2) + x;
                    y = sh - y;
                    goto default;
                case UIAnchor.UpperRight:
                    x = sw - x;
                    y = sh - y;
                    goto default;

                // Middle
                case UIAnchor.MiddleLeft:
                    y = (sh / 2) - y;
                    goto default;
                case UIAnchor.MiddleCenter:
                    x = (sw / 2) + x;
                    y = (sh / 2) - y;
                    goto default;
                case UIAnchor.MiddleRight:
                    x = sw - x;
                    y = (sh / 2) - y;
                    goto default;

                // Lower
                case UIAnchor.LowerCenter:
                    x = (sw / 2) + x;
                    goto default;

                case UIAnchor.LowerRight:
                    x = sw - x;
                    goto default;

                // End
                case UIAnchor.LowerLeft:
                default:
                    out_x = x;
                    out_y = y;
                    break;
            }
        }

        internal static void AnchorToObject(UIAnchor anchor, int x, int y, int width, int height, out int out_x, out int out_y)
        {
            switch (anchor)
            {
                // Upper
                case UIAnchor.UpperCenter:
                    x -= (width / 2);
                    goto default;
                case UIAnchor.UpperRight:
                    x -= width;
                    goto default;

                // Middle
                case UIAnchor.MiddleLeft:
                    y -= (height / 2);
                    goto default;
                case UIAnchor.MiddleCenter:
                    y -= (height / 2);
                    x -= (width / 2);
                    goto default;
                case UIAnchor.MiddleRight:
                    y -= (height / 2);
                    x -= width;
                    goto default;

                // Lower
                case UIAnchor.LowerLeft:
                    y -= height;
                    goto default;
                case UIAnchor.LowerCenter:
                    y -= height;
                    x -= (width / 2);
                    goto default;
                case UIAnchor.LowerRight:
                    y -= height;
                    x -= width;
                    goto default;

                // End
                case UIAnchor.UpperLeft:
                default:
                    out_x = x;
                    out_y = y;
                    break;
            }
        }
    }
}
