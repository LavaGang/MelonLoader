using System.IO;
using UnityEngine;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Models;
using MelonLoader;

namespace MelonLoader.MelonStartScreen
{
    internal static class UIConfig
    {
        private static string FilePath;

        internal static cGeneral General;
        internal static cBackground Background;
        internal static ExtraImageSettings LogoImage;
        internal static ExtraImageSettings LoadingImage;
        internal static ExtraTextSettings VersionText;
        internal static TextSettings ProgressText;
        internal static cProgressBar ProgressBar;

        internal static void Load()
        {
            FilePath = Path.Combine(Core.FolderPath, "Config.cfg");
            TomletMain.RegisterMapper(WriteColor, ReadColor);

            General = CreateCat<cGeneral>(nameof(General));
            Background = CreateCat<cBackground>(nameof(Background));
            LogoImage = CreateCat<ExtraImageSettings>(nameof(LogoImage));
            LoadingImage = CreateCat<ExtraImageSettings>(nameof(LoadingImage));
            VersionText = CreateCat<ExtraTextSettings>(nameof(VersionText));
            ProgressText = CreateCat<TextSettings>(nameof(ProgressText));
            ProgressBar = CreateCat<cProgressBar>(nameof(ProgressBar));
        }

        private static T CreateCat<T>(string name) where T : new()
        {
            Preferences.MelonPreferences_ReflectiveCategory cat = MelonPreferences.CreateCategory<T>(name, name);
            cat.SetFilePath(FilePath, printmsg: false);
            cat.SaveToFile(false);
            cat.DestroyFileWatcher();
            return cat.GetValue<T>();
        }
        
        internal class cGeneral
        {
            [TomlPrecedingComment("Toggles the Entire Start Screen  ( true | false )")]
            internal bool UseStartScreen = true;
        }

        internal class cBackground : ImageSettings
        {
            [TomlPrecedingComment("Solid RGBA Color of the Background")]
            internal Color SolidColor = new Color(0.08f, 0.09f, 0.10f);
        }

        internal class cProgressBar
        {
            [TomlPrecedingComment("Toggles the Progress Bar  ( true | false )")]
            internal bool Enabled = true;
            [TomlPrecedingComment("Inner RGBA Color of the Progress Bar")]
            internal Color InnerColor = new Color(1.00f, 0.23f, 0.42f);
            [TomlPrecedingComment("Outer RGBA Color of the Progress Bar")]
            internal Color OuterColor = new Color(0.47f, 0.97f, 0.39f);

            [TomlPrecedingComment("Toggles the Position Auto-Alignment of the Progress Bar  ( true | false )")]
            internal bool AutoAlign = true;
            [TomlPrecedingComment("Custom Position of the Progress Bar")]
            internal LemonTuple<int, int> CustomPosition = new LemonTuple<int, int>();
        }

        internal class TextSettings
        {
            [TomlPrecedingComment("Toggles the Text  ( true | false )")]
            internal bool Enabled = true;
            [TomlPrecedingComment("RGBA Color of the Text")]
            internal Color TextColor = new Color(1, 1, 1);
        }

        internal class ExtraTextSettings : TextSettings
        {
            [TomlPrecedingComment("Toggles the Position Auto-Alignment of the Text")]
            internal bool AutoAlign = true;
            [TomlPrecedingComment("Custom Position of the Text")]
            internal LemonTuple<int, int> CustomPosition = new LemonTuple<int, int>();

            // To-Do: Move to TextSettings
            [TomlPrecedingComment("UnityEngine.TextAnchor of the Text  ( \"UpperLeft\" | \"UpperCenter\" | \"UpperRight\" | \"MiddleLeft\" | \"MiddleCenter\" | \"MiddleRight\" | \"LowerLeft\" | \"LowerCenter\" | \"LowerRight\" )")]
            internal TextAnchor Anchor = TextAnchor.MiddleCenter;
            [TomlPrecedingComment("UnityEngine.FontStyle of the Version Text  ( \"Normal\" | \"Bold\" | \"Italic\" | \"BoldAndItalic\" )")]
            internal FontStyle Style = FontStyle.Bold;
            [TomlPrecedingComment("Is the Version Text Rich Text  ( true | false )")]
            internal bool RichText = true;
            [TomlPrecedingComment("Font Size of the Version Text")]
            internal int FontSize = 24;
            [TomlPrecedingComment("Scale of the Version Text")]
            internal float Scale = 1f;
            [TomlPrecedingComment("Scale of Line Spacing of the Version Text")]
            internal float LineSpacing = 1f;
            [TomlPrecedingComment("Version Text to be Displayed")]
            internal string Text = "<loaderName/> v<loaderVersion/> Open-Beta";
        }

        internal class ImageSettings
        {
            [TomlPrecedingComment("If should Load Custom Image  ( true | false )")]
            internal bool ScanForCustomImage = true;
            [TomlPrecedingComment("UnityEngine.FilterMode of the Image  ( \"Point\" | \"Bilinear\" | \"Trilinear\" )")]
            internal FilterMode Filter = FilterMode.Bilinear;
        }

        internal class ExtraImageSettings : ImageSettings
        {
            // To-Do: Move to TextSettings
            [TomlPrecedingComment("Toggles the Image  ( true | false )")]
            internal bool Enabled = true;

            [TomlPrecedingComment("Toggles the Position Auto-Alignment of the Image  ( true | false )")]
            internal bool AutoAlign = true;
            [TomlPrecedingComment("Custom Position of the Image")]
            internal LemonTuple<int, int> CustomPosition = new LemonTuple<int, int>();
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
