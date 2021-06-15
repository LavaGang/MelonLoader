using System;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class ApplicationHandler
    {
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
    }
}
