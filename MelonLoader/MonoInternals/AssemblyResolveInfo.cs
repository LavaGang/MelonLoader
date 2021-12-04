using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader.MonoInternals
{
    public class AssemblyResolveInfo
    {
        public Assembly Override = null;
        public Assembly Fallback = null;
        internal Dictionary<Version, Assembly> Versions = new Dictionary<Version, Assembly>();

        internal Assembly Resolve(Version requested_version)
        {
            // Check for Override
            if (Override != null)
                return Override;

            // Check for Requested Version
            if ((requested_version != null)
                && GetVersionSpecific(requested_version, out Assembly assembly))
                return assembly;

            // Check for Fallback
            if (Fallback != null)
                return Fallback;

            return null;
        }

        public void SetVersionSpecific(Version version, Assembly assembly = null)
        {
            lock (Versions)
                Versions[version] = assembly;
        }
        public Assembly GetVersionSpecific(Version version)
            => GetVersionSpecific(version, out Assembly assembly)
                ? assembly
                : null;
        public bool GetVersionSpecific(Version version, out Assembly assembly)
            => Versions.TryGetValue(version, out assembly) && assembly != null;
    }
}
