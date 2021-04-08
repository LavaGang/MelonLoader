using System;

namespace MelonLoader.CompatibilityLayers
{
    class TinyJSON_CL
    {
        internal static void Setup(AppDomain domain) =>
            domain.AssemblyResolve += (sender, args) =>
                args.Name.StartsWith("TinyJSON, Version=")
                ? typeof(TinyJSON_CL).Assembly
                : null;
    }
}