using System.IO;
using UnityEngine;
using Tomlet;
using Tomlet.Models;
using MelonLoader;

namespace MelonLoader.MelonStartScreen
{
    internal static class UICustomization
    {
        private static string FilePath;

        internal static cGeneral General;
        internal static cLogo Logo;
        internal static cAnimation Animation;
        internal static cVersionText VersionText;
        internal static cProgressText ProgressText;
        internal static cProgressBar ProgressBar;

        internal static void Load()
        {
            FilePath = Path.Combine(Core.FolderPath, "Config.cfg");
            TomletMain.RegisterMapper(WriteColor, ReadColor);

            General = CreateCat<cGeneral>(nameof(General));
            Logo = CreateCat<cLogo>(nameof(Logo));
            Animation = CreateCat<cAnimation>(nameof(Animation));
            VersionText = CreateCat<cVersionText>(nameof(VersionText));
            ProgressText = CreateCat<cProgressText>(nameof(ProgressText));
            ProgressBar = CreateCat<cProgressBar>(nameof(ProgressBar));
        }

        private static T CreateCat<T>(string name) where T : new()
        {
            Preferences.MelonPreferences_ReflectiveCategory cat = MelonPreferences.CreateCategory<T>(name, name);
            cat.SetFilePath(FilePath, printmsg: false);
            cat.SaveToFile(false);
            return cat.GetValue<T>();
        }
        
        internal class cGeneral
        {
            internal bool UseStartScreen = true;
            internal Color BackgroundColor = new Color(0.08f, 0.09f, 0.10f);
        }

        internal class cLogo
        {
            internal bool Enabled = true;
        }

        internal class cAnimation
        {
            internal bool Enabled = true;
        }

        internal class cVersionText
        {
            internal bool Enabled = true;
            internal Color TextColor = new Color(1, 1, 1);
        }

        internal class cProgressText
        {
            internal bool Enabled = true;
            internal Color TextColor = new Color(1, 1, 1);
        }

        internal class cProgressBar
        {
            internal bool Enabled = true;
            internal Color InnerColor = new Color(1.00f, 0.23f, 0.42f);
            internal Color OuterColor = new Color(0.47f, 0.97f, 0.39f);
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
