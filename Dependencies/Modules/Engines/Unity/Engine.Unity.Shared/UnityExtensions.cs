using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace MelonLoader.Engine.Unity
{
    public static class UnityExtensions
    {
        #region AssetsManager

        public static ClassPackageFile LoadIncludedClassPackage(this AssetsManager assetsManager)
        {
			var asm = typeof(UnityExtensions).Assembly;
            var names = asm.GetManifestResourceNames();
            string resourceName = null;
            foreach (var name in names)
                if (name.Contains("classdata"))
                {
                    resourceName = name;
                    break;
                }
            if (string.IsNullOrEmpty(resourceName))
                return null;

            ClassPackageFile classPackage = null;
            using (var stream = asm.GetManifestResourceStream(resourceName))
                classPackage = assetsManager.LoadClassPackage(stream);
            return classPackage;
        }

        #endregion
    }
}
