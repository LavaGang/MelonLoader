using System.IO;
using UnityEngine;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Models;
using MelonLoader;
using MelonLoader.MelonStartScreen.UI;
using System;

namespace MelonLoader.MelonStartScreen
{
    internal static class UIConfig
    {
        private static string FilePath;
        internal static string ThemePath;

        internal static cGeneral General;
        internal static cBackground Background;
        internal static ImageSettings LogoImage;
        internal static ImageSettings LoadingImage;
        internal static TextSettings VersionText;
        internal static TextSettings ProgressText;
        internal static cProgressBar ProgressBar;

        internal static void Load()
        {
            TomletMain.RegisterMapper(WriteColor, ReadColor);

            FilePath = Path.Combine(Core.FolderPath, "Config.cfg");
            General = CreateCat<cGeneral>(FilePath, nameof(General));

            bool UseDefault = true;
            if (!string.IsNullOrEmpty(General.Theme) && !General.Theme.Equals("Default"))
            {
                try
                {
                    // To-Do: Sanatize themeName
                    General.Theme = General.Theme
                        .Replace("\\", "")
                        .Replace("/", "");

                    ThemePath = Path.Combine(Core.ThemesFolderPath, General.Theme);
                    if (Directory.Exists(ThemePath))
                        UseDefault = false;
                    else
                        throw new DirectoryNotFoundException(ThemePath);
                }
                catch (Exception ex) { Core.Logger.Error($"Failed to find Start Screen Theme: {ex}"); }
            }

            if (UseDefault)
            {
                General.Theme = "Default";
                ThemePath = Path.Combine(Core.ThemesFolderPath, General.Theme);
                if (!Directory.Exists(ThemePath))
                    Directory.CreateDirectory(ThemePath);
            }

            Core.Logger.Msg($"Using Start Screen Theme: \"{General.Theme}\"");

            Background = CreateCat<cBackground>(nameof(Background), true);
            LogoImage = CreateCat<LogoImageSettings>(nameof(LogoImage), true);
            LoadingImage = CreateCat<LoadingImageSettings>(nameof(LoadingImage), true);
            VersionText = CreateCat<VersionTextSettings>(nameof(VersionText), true);
            ProgressText = CreateCat<ProgressTextSettings>(nameof(ProgressText), true);
            ProgressBar = CreateCat<cProgressBar>(nameof(ProgressBar), true);

            MelonPreferences.SaveCategory<cGeneral>(nameof(General), false);
        }

        private static T CreateCat<T>(string name, bool shouldRemoveOld = false) where T : new() => CreateCat<T>(Path.Combine(ThemePath, $"{name}.cfg"), name, shouldRemoveOld);
        private static T CreateCat<T>(string filePath, string name, bool shouldRemoveOld = false) where T : new()
        {
            if (shouldRemoveOld)
                MelonPreferences.RemoveCategoryFromFile(FilePath, name);
            Preferences.MelonPreferences_ReflectiveCategory cat = MelonPreferences.CreateCategory<T>(name, name);
            cat.SetFilePath(filePath, true, false);
            cat.SaveToFile(false);
            cat.DestroyFileWatcher();
            return cat.GetValue<T>();
        }
        
        internal class cGeneral
        {
            [TomlPrecedingComment("Toggles the Entire Start Screen  ( true | false )")]
            internal bool Enabled = true;
            [TomlPrecedingComment("Current Theme of the Start Screen")]
            internal string Theme = "Default";
        }

        internal class cBackground : ImageSettings
        {
            [TomlPrecedingComment("Solid RGBA Color of the Background")]
            internal Color SolidColor = new Color(0.08f, 0.09f, 0.10f);
            [TomlPrecedingComment("If it should stretch the Background to the Full Window Size")]
            internal bool StretchToScreen = true;
        }

        internal class ProgressTextSettings : TextSettings
        {
            public ProgressTextSettings()
            {
                Position.Item2 = 56;
                Anchor = UIAnchor.MiddleCenter;
                ScreenAnchor = UIAnchor.MiddleCenter;
            }
        }

        internal class cProgressBar : ElementSettings
        {
            public cProgressBar()
            {
                Position.Item2 = 56;
                Size.Item1 = 540;
                Size.Item2 = 36;

                Anchor = UIAnchor.MiddleCenter;
                ScreenAnchor = UIAnchor.MiddleCenter;
            }

            [TomlPrecedingComment("Inner RGBA Color of the Progress Bar")]
            internal Color InnerColor = new Color(1.00f, 0.23f, 0.42f);
            [TomlPrecedingComment("Outer RGBA Color of the Progress Bar")]
            internal Color OuterColor = new Color(0.47f, 0.97f, 0.39f);
        }

        internal class VersionTextSettings : TextSettings
        {
            public VersionTextSettings()
            {
                Text = "<loaderName/> v<loaderVersion/> Open-Beta";
                TextSize = 24;
                Anchor = UIAnchor.MiddleCenter;
                ScreenAnchor = UIAnchor.MiddleCenter;
                Position.Item2 = 16;
            }
        }

        internal class LogoImageSettings : ImageSettings
        {
            public LogoImageSettings()
            {
                Size.Item1 = 262;
                Size.Item2 = 212;
                Position.Item2 = -(Size.Item2 / 2);

                Anchor = UIAnchor.MiddleCenter;
                ScreenAnchor = UIAnchor.MiddleCenter;
            }
        }

        internal class LoadingImageSettings : ImageSettings
        {
            public LoadingImageSettings()
            {
                Position.Item2 = 35;
                Size.Item1 = 200;
                Size.Item2 = 132;

                Anchor = UIAnchor.LowerRight;
                ScreenAnchor = UIAnchor.LowerRight;
            }
        }

        internal class ElementSettings
        {
            [TomlPrecedingComment("Toggles the Element  ( true | false )")]
            internal bool Enabled = true;

            [TomlPrecedingComment("Position of the Element")]
            internal LemonTuple<int, int> Position = new LemonTuple<int, int>();
            [TomlPrecedingComment("Size of the Element")]
            internal LemonTuple<int, int> Size = new LemonTuple<int, int>();

            [TomlPrecedingComment("Anchor of the Text relative to Itself  ( \"None\" | \"UpperLeft\" | \"UpperCenter\" | \"UpperRight\" | \"MiddleLeft\" | \"MiddleCenter\" | \"MiddleRight\" | \"LowerLeft\" | \"LowerCenter\" | \"LowerRight\" )")]
            internal UIAnchor Anchor = UIAnchor.UpperLeft;
            [TomlPrecedingComment("Anchor of the Text relative to the Screen  ( \"None\" | \"UpperLeft\" | \"UpperCenter\" | \"UpperRight\" | \"MiddleLeft\" | \"MiddleCenter\" | \"MiddleRight\" | \"LowerLeft\" | \"LowerCenter\" | \"LowerRight\" )")]
            internal UIAnchor ScreenAnchor = UIAnchor.UpperLeft;
        }

        internal class TextSettings : ElementSettings
        {
            [TomlPrecedingComment("UnityEngine.FontStyle of the Text  ( \"Normal\" | \"Bold\" | \"Italic\" | \"BoldAndItalic\" )")]
            internal FontStyle Style = FontStyle.Bold;
            [TomlPrecedingComment("Is this Rich Text  ( true | false )")]
            internal bool RichText = true;
            [TomlPrecedingComment("Size of the Text")]
            internal int TextSize = 16;
            [TomlPrecedingComment("Scale of the Text")]
            internal float Scale = 1f;
            [TomlPrecedingComment("Scale of Line Spacing of the Text")]
            internal float LineSpacing = 1f;
            [TomlPrecedingComment("RGBA Color of the Text")]
            internal Color TextColor = new Color(1, 1, 1);
            [TomlPrecedingComment("Text to be Displayed")]
            internal string Text;
        }

        internal class ImageSettings : ElementSettings
        {
            [TomlPrecedingComment("If should Load Custom Image  ( true | false )")]
            internal bool ScanForCustomImage = true;

            [TomlPrecedingComment("UnityEngine.FilterMode of the Image  ( \"Point\" | \"Bilinear\" | \"Trilinear\" )")]
            internal FilterMode Filter = FilterMode.Bilinear;

            [TomlPrecedingComment("If the Image should attempt to Maintain it's Aspect Ratio")]
            internal bool MaintainAspectRatio = false;
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
