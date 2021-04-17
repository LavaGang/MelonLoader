using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader.CompatibilityLayers;

namespace MelonLoader.Support
{
    internal static class CompatibilityLayer
    {
        private static void Setup(AppDomain domain)
        {
            IPA_CL.Setup(domain);
        }
    }
}