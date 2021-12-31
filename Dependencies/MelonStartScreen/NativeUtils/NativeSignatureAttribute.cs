using System;

namespace MelonLoader.MelonStartScreen.NativeUtils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class NativeSignatureAttribute : Attribute
    {
        internal uint LookupIndex { get; }
        internal NativeSignatureFlags Flags { get; }
        internal string Signature { get; }
        internal string[] MinimalUnityVersions { get; }

        internal NativeSignatureAttribute(uint lookupIndex, NativeSignatureFlags flags, string signature, params string[] minimalUnityVersions)
        {
            LookupIndex = lookupIndex;
            Flags = flags;
            Signature = signature;
            MinimalUnityVersions = minimalUnityVersions;
        }
    }
}
