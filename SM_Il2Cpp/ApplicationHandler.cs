using System;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class ApplicationHandler
    {
#if __ANDROID__
        // internal static string _InternalGUID = new Guid().ToString();

        internal static string GetVersion()
        {
            return "0.0.0";
        }

        internal static string GetBuildGUID()
        {
            return "0000";
        }
#else
        private static MethodInfo Application_get_version = null;
        private static MethodInfo Application_get_buildGUID = null;

        static ApplicationHandler()
        {
            Type applicationType = typeof(Application);

            PropertyInfo versionProp = applicationType.GetProperty("version");
            if (versionProp != null)
                Application_get_version = versionProp.GetGetMethod();

            PropertyInfo buildGUIDProp = applicationType.GetProperty("buildGUID");
            if (buildGUIDProp != null)
                Application_get_buildGUID = buildGUIDProp.GetGetMethod();
        }

        internal static string GetVersion()
            => (Application_get_version != null)
                ? (string)Application_get_version.Invoke(null, new object[0])
                : null;

        internal static string GetBuildGUID()
            => (Application_get_buildGUID != null)
                ? (string)Application_get_buildGUID.Invoke(null, new object[0])
                : null;
#endif
    }
}
