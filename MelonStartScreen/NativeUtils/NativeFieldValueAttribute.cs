using System;

namespace MelonLoader.MelonStartScreen.NativeUtils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    class NativeFieldValueAttribute : Attribute
    {
        internal uint LookupIndex { get; }
        internal NativeSignatureFlags Flags { get; }
        internal object Value { get; }
        internal string[] MinimalUnityVersions { get; }

        public NativeFieldValueAttribute(uint lookupIndex, NativeSignatureFlags flags, object value, params string[] minimalUnityVersions)
        {
            LookupIndex = lookupIndex;
            Flags = flags;
            Value = value;
            MinimalUnityVersions = minimalUnityVersions;
        }
    }
}
