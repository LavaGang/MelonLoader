using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class UIUtils
    {
        public static Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.SetPixels(new Color[] { color, color, color, color });
            texture.Apply();
            return texture;
        }
    }
}
