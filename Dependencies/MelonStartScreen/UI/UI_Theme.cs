using System.IO;
using MelonUnityEngine;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Models;
using MelonLoader.MelonStartScreen.UI;
using System;

namespace MelonLoader.MelonStartScreen
{
    internal abstract class UI_Theme
    {
        internal static UI_Theme Instance;

        private static string[] includedThemeIDs =
        {
            "Default",
            "Random",
            "Pumpkin"
        };
        internal static bool IsIncludedThemeID() { foreach (var id in includedThemeIDs) if (id.Equals(ThemeID)) return true; return false; }
        internal static string ThemeID = includedThemeIDs[0];

        internal static string FilePath;
        internal static string ThemePath;
        internal static cGeneral General;
        internal static bool IsLemon;
        internal static bool IsPumpkin;

        internal cBackground Background;
        internal ImageSettings LogoImage;
        internal ImageSettings LoadingImage;
        internal TextSettings ProgressText;
        internal cProgressBar ProgressBar;
        internal VersionTextSettings VersionText;

        internal void Defaults()
        {
            if (Background == null)
                Background = CreateCat<cBackground>(nameof(Background), true);
            if (LogoImage == null)
                LogoImage = CreateCat<LogoImageSettings>(nameof(LogoImage), true);
            if (LoadingImage == null)
                LoadingImage = CreateCat<LoadingImageSettings>(nameof(LoadingImage), true);
            if (ProgressText == null)
                ProgressText = CreateCat<ProgressTextSettings>(nameof(ProgressText), true);
            if (ProgressBar == null)
                ProgressBar = CreateCat<cProgressBar>(nameof(ProgressBar), true);
            if (VersionText == null)
                VersionText = CreateCat<VersionTextSettings>(nameof(VersionText), true);
        }

        internal static void Load()
        {
            TomletMain.RegisterMapper(WriteColor, ReadColor);

            FilePath = Path.Combine(Core.FolderPath, "Config.cfg");
            General = CreateCat<cGeneral>(FilePath, nameof(General));

            if (!string.IsNullOrEmpty(General.Theme))
                ThemeID = General.Theme
                    .Replace("\\", "")
                    .Replace("/", "");

            if (ThemeID == "Halloween")
                General.Theme = ThemeID = includedThemeIDs[0];

            bool isIncludedID = IsIncludedThemeID();

            if (ThemeID.Equals(includedThemeIDs[1]))
                ThemePath = UI_Utils.RandomFolder(Core.ThemesFolderPath);
            else
                ThemePath = Path.Combine(Core.ThemesFolderPath, ThemeID);

            if (!isIncludedID && !Directory.Exists(ThemePath))
            {
                Core.Logger.Error($"Failed to find Start Screen Theme: {ThemeID}");

                isIncludedID = true;
                General.Theme = ThemeID = includedThemeIDs[0];
                ThemePath = Path.Combine(Core.ThemesFolderPath, ThemeID);
            }

            if (isIncludedID && !ThemeID.Equals(includedThemeIDs[1]))
            {
                // Lemon
                IsLemon = (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON);
                if (!IsLemon)
                {
                    // Pumpkin
                    IsPumpkin = ThemeID.Equals(includedThemeIDs[2]);
                    var nowTime = DateTime.Now;
                    if ((nowTime.Month == 10)
                        && (nowTime.Day == 31))
                        IsPumpkin = true;
                }

                ThemeID = "Default";
                ThemePath = Path.Combine(Core.ThemesFolderPath, ThemeID);
            }

            if (!Directory.Exists(ThemePath))
                Directory.CreateDirectory(ThemePath);

            if (isIncludedID)
                MelonPreferences.SaveCategory<cGeneral>(nameof(General), false);

            string themeName = (
                IsPumpkin ? "Pumpkin"
                : (IsLemon ? "Lemon" : ThemeID));

            Core.Logger.Msg($"Using Start Screen Theme: \"{themeName}\"");

            Instance =
                IsPumpkin ? new UI.Themes.UI_Theme_Pumpkin()
                : (IsLemon ? new UI.Themes.UI_Theme_Lemon() 
                : new UI.Themes.UI_Theme_Default());
        }

        internal abstract byte[] GetLogoImage();
        internal abstract byte[] GetLoadingImage();

        internal static T CreateCat<T>(string name, bool shouldRemoveOld = false) where T : new() => CreateCat<T>(Path.Combine(ThemePath, $"{name}.cfg"), name, shouldRemoveOld);
        internal static T CreateCat<T>(string filePath, string name, bool shouldRemoveOld = false) where T : new()
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
                Anchor = UI_Anchor.MiddleCenter;
                ScreenAnchor = UI_Anchor.MiddleCenter;
            }
        }

        internal class cProgressBar : ElementSettings
        {
            public cProgressBar() => Defaults();
            public void Defaults()
            {
                Position.Item2 = 56;
                Size.Item1 = 540;
                Size.Item2 = 36;

                Anchor = UI_Anchor.MiddleCenter;
                ScreenAnchor = UI_Anchor.MiddleCenter;
            }

            [TomlPrecedingComment("Inner RGBA Color of the Progress Bar")]
            internal Color InnerColor = new Color(1.00f, 0.23f, 0.42f);
            [TomlPrecedingComment("Outer RGBA Color of the Progress Bar")]
            internal Color OuterColor = new Color(0.47f, 0.97f, 0.39f);
        }

        internal class VersionTextSettings : TextSettings
        {
            internal bool Is_ALPHA_PreRelease = false;
            public VersionTextSettings() => Defaults();
            public void Defaults()
            {
                if (Text == null)
                    Text = $"<loaderName/> v<loaderVersion/> {(Is_ALPHA_PreRelease ? "ALPHA Pre-Release" : "Open-Beta")}";
                TextSize = 24;
                Anchor = UI_Anchor.MiddleCenter;
                ScreenAnchor = UI_Anchor.MiddleCenter;
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

                Anchor = UI_Anchor.MiddleCenter;
                ScreenAnchor = UI_Anchor.MiddleCenter;
            }
        }

        internal class LoadingImageSettings : ImageSettings
        {
            public LoadingImageSettings()
            {
                Position.Item2 = 35;
                Size.Item1 = 200;
                Size.Item2 = 132;

                Anchor = UI_Anchor.LowerRight;
                ScreenAnchor = UI_Anchor.LowerRight;
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
            internal UI_Anchor Anchor = UI_Anchor.UpperLeft;
            [TomlPrecedingComment("Anchor of the Text relative to the Screen  ( \"None\" | \"UpperLeft\" | \"UpperCenter\" | \"UpperRight\" | \"MiddleLeft\" | \"MiddleCenter\" | \"MiddleRight\" | \"LowerLeft\" | \"LowerCenter\" | \"LowerRight\" )")]
            internal UI_Anchor ScreenAnchor = UI_Anchor.UpperLeft;
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
            [TomlPrecedingComment("Font of the Text")]
            internal string Font = "Arial";
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
