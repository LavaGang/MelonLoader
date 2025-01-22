using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonPlatformDomainAttribute(MelonPlatformDomainAttribute.CompatibleDomains domain = MelonPlatformDomainAttribute.CompatibleDomains.UNIVERSAL) : Attribute
{
    // <summary>Enum for Melon Platform Domain Compatibility.</summary>
    public enum CompatibleDomains
    {
        UNIVERSAL,
        MONO,
        IL2CPP
    };

    // <summary>Platform Domain Compatibility of the Melon.</summary>
    public CompatibleDomains Domain { get; internal set; } = domain;

    public bool IsCompatible(CompatibleDomains domain)
        => Domain == CompatibleDomains.UNIVERSAL || domain == CompatibleDomains.UNIVERSAL || Domain == domain;
}