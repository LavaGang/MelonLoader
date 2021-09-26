using System;

namespace MelonLoader.MelonStartScreen.NativeUtils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class NativeSignatureAttribute : Attribute
    {
        internal NativeSignatureFlags Flags { get; }
        internal string Signature { get; }
        internal string[] MinimalUnityVersions { get; }

        internal NativeSignatureAttribute(NativeSignatureFlags flags, string signature, params string[] minimalUnityVersions)
        {
            Flags = flags;
            Signature = signature;
            MinimalUnityVersions = minimalUnityVersions;
        }
    }
}
