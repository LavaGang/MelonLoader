﻿using System;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class IL2CPP_Event : IL2CPP_Base
    {
        internal IL2CPP_Event(IntPtr ptr) : base(ptr) => Ptr = ptr;
    }
}