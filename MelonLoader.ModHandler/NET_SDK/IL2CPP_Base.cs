using System;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class IL2CPP_Base
    {
        public IntPtr Ptr { get; internal set; }
        internal IL2CPP_Base(IntPtr ptr)
            => Ptr = ptr;
    }
}