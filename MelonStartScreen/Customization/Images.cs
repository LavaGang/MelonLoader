using System;
using System.IO;
using System.Linq;
using MelonLoader.MelonStartScreen.UI;

namespace MelonLoader.MelonStartScreen.Customization
{
    internal static class Images
    {
        internal static Image Loading()
        {

            Image returnval = LoadImage("Loading.gif");
            if (returnval == null)
                returnval = LoadImage("Loading.png");
            if (returnval != null)
            {
                MelonDebug.Msg("[UI.Customization.Images] Found Custom Loading Image!");
                return returnval;
            }

            MelonDebug.Msg("[UI.Customization.Images] Using Default Loading Image...");
            try { return new AnimatedImage(ImageDatas.FunnyImage.Select(data => Convert.FromBase64String(data)).ToArray()); }
            catch (Exception ex)
            {
                MelonLogger.Error($"[UI.Customization.Images] Failed To Load Default Loading Image: {ex}");
                return null;
            }
        }

        internal static Image Logo()
        {
            Image returnval = LoadImage("Logo.gif");
            if (returnval == null)
                returnval = LoadImage("Logo.png");
            if (returnval != null)
            {
                MelonDebug.Msg("[UI.Customization.Images] Found Custom Logo Image!");
                return returnval;
            }

            MelonDebug.Msg("[UI.Customization.Images] Using Default Logo Image...");
            try
            {
                return new Image(
                    (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON)
                        ? Properties.Resources.Logo_Lemon
                        : Properties.Resources.Logo_Melon);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[UI.Customization.Images] Failed To Load Default Logo Image: {ex}");
                return null;
            }
        }

        internal static Image Background()
        {
            Image returnval = LoadImage("Background.gif");
            if (returnval == null)
                returnval = LoadImage("Background.png");
            if (returnval != null)
            {
                MelonDebug.Msg("[UI.Customization.Images] Found Custom Background Image!");
                return returnval;
            }

            MelonDebug.Msg("[UI.Customization.Images] Using Solid Color Background...");
            returnval = new Image();
            returnval.MainTexture = UIStyleValues.BackgroundTexture;
            return returnval;
        }


        private static Image LoadImage(string filename)
        {
            string filepath = ScanForFile(filename);
            if (string.IsNullOrEmpty(filepath))
                return null;

            if (Path.GetExtension(filepath).ToLowerInvariant().Equals(".gif"))
            {
                try { return new AnimatedImage(filepath); }
                catch (Exception ex) { MelonLogger.Error($"[UI.Customization.Images] Failed To Load AnimatedImage {filepath}: {ex}"); }
            }

            try { return new Image(filepath); }
            catch (Exception ex) { MelonLogger.Error($"[UI.Customization.Images] Failed To Load Image {filepath}: {ex}"); }

            return null;
        }

        private static string ScanForFile(string filename)
        {
            string[] files = Directory.GetFiles(Core.FolderPath);
            if (files.Length <= 0)
                return null;
            return files.FirstOrDefault(x =>
                Path.GetFileName(x)
                    .ToLowerInvariant()
                    .Equals(
                        filename
                        .ToLowerInvariant()));
        }
    }
}
