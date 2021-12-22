using System;
using System.IO;
using System.Linq;

namespace MelonLoader.MelonStartScreen.UI.Customization
{
    internal static class Images
    {
        private static AnimatedImage DefaultAnimatedImage()
        {
            MelonDebug.Msg("[UI.Customization.Images] Loading Default Animated Image...");

            try
            {
                return new AnimatedImage(33, 40, ImageDatas.FunnyImage.Select(data => Convert.FromBase64String(data)).ToArray());
            }
            catch (Exception ex)
            {
                MelonDebug.Error($"[UI.Customization.Images] Failed To Load Custom Animated Image: {ex}");
                return null;
            }
        }

        internal static AnimatedImage LoadAnimatedImage()
        {
            string filepath = ScanForFileInUserData("Loading.gif");
            if (string.IsNullOrEmpty(filepath))
                return DefaultAnimatedImage();

            MelonDebug.Msg("[UI.Customization.Images] Loading Custom Animated Image!");

            try
            {
                return new AnimatedImage(filepath);
            }
            catch (Exception ex)
            {
                MelonDebug.Error($"[UI.Customization.Images] Failed To Load Custom Animated Image: {ex}");
                return DefaultAnimatedImage();
            }
        }

        private static string ScanForFileInUserData(string filename)
        {
            string[] files = Directory.GetFiles(MelonUtils.UserDataDirectory);
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
