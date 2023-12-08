using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace MelonLoader.Unity.Utils
{
    public static class UnityExtensions
    {
        #region AssetsManager

        public static ClassPackageFile LoadIncludedClassPackage(this AssetsManager assetsManager)
        {
            ClassPackageFile classPackage = null;
            using (var stream = typeof(UnityExtensions).Assembly.GetManifestResourceStream("MelonLoader.Unity.Resources.classdata.tpk"))
                classPackage = assetsManager.LoadClassPackage(stream);
            return classPackage;
        }

        #endregion
    }
}
