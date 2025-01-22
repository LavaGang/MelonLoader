using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonPlatformDomainAttribute : Attribute
    {
        public MelonPlatformDomainAttribute(CompatibleDomains domain = CompatibleDomains.UNIVERSAL) => Domain = domain;

        // <summary>Enum for Melon Platform Domain Compatibility.</summary>
        public enum CompatibleDomains
        {
            UNIVERSAL,
            MONO,
            IL2CPP
        };

        // <summary>Platform Domain Compatibility of the Melon.</summary>
        public CompatibleDomains Domain { get; internal set; }

        public bool IsCompatible(CompatibleDomains domain)
            => Domain == CompatibleDomains.UNIVERSAL || domain == CompatibleDomains.UNIVERSAL || Domain == domain;
    }
}