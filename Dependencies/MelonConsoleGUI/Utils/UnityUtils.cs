using UnityEngine;

namespace MelonLoader.Console
{
    internal static class UnityUtils
    {
        public static Texture2D CreateColorTexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        public static Color ChangeAlpha(this Color color, float alpha)
            => new Color(color.r, color.g, color.b, alpha);
    }
}
