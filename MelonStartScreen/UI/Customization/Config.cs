using System.IO;
using UnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Customization
{
    internal static class Config
    {
        private static string FilePath;

        internal static void Load()
        {
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
    }
}
