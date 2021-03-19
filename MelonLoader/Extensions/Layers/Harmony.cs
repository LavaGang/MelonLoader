using System;

namespace MelonLoader.CompatibilityLayers
{
    class Harmony_CL
    {
        internal static void Setup(AppDomain domain) => 
            domain.AssemblyResolve += (sender, args) => 
                args.Name.StartsWith("0Harmony, Version=") 
                ? typeof(Harmony_CL).Assembly 
                : null;
    }
}