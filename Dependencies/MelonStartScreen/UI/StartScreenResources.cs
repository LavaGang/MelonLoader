using System.IO;
using System.Reflection;
using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI
{
    internal static class StartScreenResources
    {
        public static byte[] HalloweenLoadingIcon => GetResource("Loading_Halloween.dat");
        public static byte[] MelonLoadingIcon => GetResource("Loading_Melon.dat");
        public static byte[] LemonLoadingIcon => GetResource("Loading_Lemon.dat");
        
        //Logos
        public static byte[] HalloweenLogo => GetResource("Logo_Halloween.dat");
        public static byte[] MelonLogo => GetResource("Logo_Melon.dat");
        public static byte[] LemonLogo => GetResource("Logo_Lemon.dat");
        
        private static byte[] GetResource(string name)
        {
            using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MelonLoader.MelonStartScreen.Resources.{name}");
            if (s == null)
                return null;
#if NET6_0
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
#else
            var ret = new byte[s.Length];
            s.Read(ret, 0, ret.Length);
            return ret;
#endif
        }
    }
}