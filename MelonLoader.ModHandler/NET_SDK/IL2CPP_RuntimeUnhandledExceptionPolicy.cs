using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public enum IL2CPP_RuntimeUnhandledExceptionPolicy
    {
        IL2CPP_UNHANDLED_POLICY_LEGACY,
        IL2CPP_UNHANDLED_POLICY_CURRENT
    }
}
