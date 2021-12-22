using System.IO;
using UnityEngine;
using Tomlet;
using Tomlet.Models;
using MelonLoader;

namespace MelonLoader.MelonStartScreen.UI.Customization
{
    internal static class Config
    {
        private static string FilePath;

        internal static void Load()
        {
            TomletMain.RegisterMapper(WriteColor, ReadColor);

            FilePath = Path.Combine(MelonUtils.UserDataDirectory, "StartScreen.cfg");

            Colors.Setup();

            if (!File.Exists(FilePath))
                Colors.Category.SaveToFile(false);
        }

        internal static class Colors
        {
            internal static MelonPreferences_Category Category;
            internal static MelonPreferences_Entry<Color> Background;
            internal static MelonPreferences_Entry<Color> ProgressBar;
            internal static MelonPreferences_Entry<Color> ProgressBarOutline;
            internal static MelonPreferences_Entry<Color> Text;

            internal static void Setup()
            {
                MelonDebug.Msg("[UI.Customization.Config.Colors] Initializing...");

                Category = MelonPreferences.CreateCategory("Colors", "Colors");
                Category.SetFilePath(FilePath, printmsg: false);

                Background = Category.CreateEntry(nameof(Background), new Color(0.08f, 0.09f, 0.10f), nameof(Background));
                ProgressBar = Category.CreateEntry(nameof(ProgressBar), new Color(0.47f, 0.97f, 0.39f), nameof(ProgressBar));
                ProgressBarOutline = Category.CreateEntry(nameof(ProgressBarOutline), new Color(1.00f, 0.23f, 0.42f), nameof(ProgressBarOutline));
                Text = Category.CreateEntry(nameof(Text), new Color(1, 1, 1), nameof(Text));
            }
        }

        private static Color ReadColor(TomlValue value)
        {
            float[] floats = MelonPreferences.Mapper.ReadArray<float>(value);
            if (floats == null || floats.Length != 4)
                return default;
            return new Color(floats[0] / 255f, floats[1] / 255f, floats[2] / 255f, floats[3] / 255f);
        }

        private static TomlValue WriteColor(Color value)
        {
            float[] floats = new[] { value.r * 255, value.g * 255, value.b * 255, value.a * 255 };
            return MelonPreferences.Mapper.WriteArray(floats);
        }
    }
}
