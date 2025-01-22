using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader.Resolver;

public class AssemblyResolveInfo
{
    public Assembly Override = null;
    public Assembly Fallback = null;
    internal Dictionary<Version, Assembly> Versions = [];

    internal Assembly Resolve(Version requested_version)
    {
        // Check for Override
        if (Override != null)
            return Override;

        // Check for Requested Version
        if (requested_version != null
            && GetVersionSpecific(requested_version, out var assembly))
            return assembly;

        // Check for Fallback
        return Fallback ?? null;
    }

    public void SetVersionSpecific(Version version, Assembly assembly = null)
    {
        lock (Versions)
            Versions[version] = assembly;
    }
    public Assembly GetVersionSpecific(Version version)
        => GetVersionSpecific(version, out var assembly)
            ? assembly
            : null;
    public bool GetVersionSpecific(Version version, out Assembly assembly)
        => Versions.TryGetValue(version, out assembly) && assembly != null;
}
